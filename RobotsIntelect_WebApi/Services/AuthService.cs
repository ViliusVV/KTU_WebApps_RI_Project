using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using RobotsIntelect_WebApi.Models;
using RobotsIntelect_WebApi.Helpers;
using RobotsIntelect_WebApi.Models.Auth;
using RobotsIntelect_WebApi.Repository.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace RobotsIntelect_WebApi.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMongoRepository<User> _userRepository;
        private readonly AppSettings _appSettings;

        public AuthService(IMongoRepository<User> userRepository, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            // TODO: Implement hash
            string hashedPassword = model.Password;
            var usrs = _userRepository.AsQueryable().ToList();
            var user =  _userRepository.AsQueryable().SingleOrDefault(x => x.Username == model.Username && x.PasswordHash == hashedPassword);

            // return null if user not found
            if (user == null) return null;

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken(ipAddress);

            // save refresh token

            // TODO: Make shure this is correct
            user.RefreshTokens.Add(refreshToken);

            _userRepository.ReplaceOne(user);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var user = _userRepository.AsQueryable().SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            var usrs = _userRepository.AsQueryable().ToList();
            // return null if no user found with token
            if (user == null) return null;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return null if token is no longer active
            if (!refreshToken.IsActive) return null;

            // replace old refresh token with a new one and save
            var newRefreshToken = generateRefreshToken(ipAddress);
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.ReplacedByToken = newRefreshToken.Token;
            user.RefreshTokens.Add(newRefreshToken);

            _userRepository.ReplaceOne(user);

            // generate new jwt
            var jwtToken = generateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _userRepository.AsQueryable().SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            
            // return false if no user found with token
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            // return false if token is not active
            if (!refreshToken.IsActive) return false;

            // revoke token and save
            refreshToken.Revoked = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;

            _userRepository.ReplaceOne(user);

            return true;
        }

        public IEnumerable<User> GetAll()
        {
            return _userRepository.AsQueryable();
        }

        public User GetById(string id)
        {
            return _userRepository.FindById(id);
        }

        private string generateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private RefreshToken generateRefreshToken(string ipAddress)
        {
            using(var rngCryptoServiceProvider = new RNGCryptoServiceProvider())
            {
                var randomBytes = new byte[64];
                rngCryptoServiceProvider.GetBytes(randomBytes);
                return new RefreshToken
                {
                    Token = Convert.ToBase64String(randomBytes),
                    Expires = DateTime.UtcNow.AddDays(7),
                    Created = DateTime.UtcNow,
                    CreatedByIp = ipAddress
                };
            }
        }
    }
}