using RobotsIntelect_WebApi.Models;
using RobotsIntelect_WebApi.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Services
{
    public interface IAuthService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse AuthenticateSensor(string token, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        RefreshToken generateRefreshToken(string ipAddress);
        bool RevokeToken(string token, string ipAddress);
        IEnumerable<User> GetAll();
        User GetById(string id);
    }

}
