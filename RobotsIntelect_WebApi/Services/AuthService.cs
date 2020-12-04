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
        private readonly IMongoRepository<LapSensor> _sensorRepository;
        private readonly AppSettings _appSettings;

        public AuthService(IMongoRepository<User> userRepository, IMongoRepository<LapSensor> sensorRepository, IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _sensorRepository = sensorRepository;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user =  _userRepository.AsQueryable().SingleOrDefault(x => x.Username == model.Username);

            if (user == null) return null;

            bool verified = BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash);

            if (!verified) return null;


            // Generate tokens
            var jwtToken = generateJwtToken(user);
            var refreshToken = generateRefreshToken(ipAddress);

            // Save refresh token

            user.RefreshTokens.Add(refreshToken);
            _userRepository.ReplaceOne(user);

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var user = _userRepository.AsQueryable().SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
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


        public AuthenticateResponse AuthenticateSensor(string token, string ipAddress)
        {
            var sensor = _sensorRepository.AsQueryable().SingleOrDefault(s => s.ApiKey.Token == token);

            if (sensor == null) return null;


            var fakeUser = new User() { Name = sensor.SensorName, Role = Role.Sensor, Id = sensor.Id};
            var jwtToken = generateJwtToken(fakeUser);

            return new AuthenticateResponse(fakeUser, jwtToken, token);
        }

        public bool RevokeToken(string token, string ipAddress)
        {
            var user = _userRepository.AsQueryable().SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
            
            if (user == null) return false;

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive) return false;

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

        public RefreshToken generateRefreshToken(string ipAddress)
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