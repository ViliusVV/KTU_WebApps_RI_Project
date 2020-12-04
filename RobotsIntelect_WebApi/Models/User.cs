using RobotsIntelect_WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Models
{
    [BsonCollection("users")]
    public class User: Document
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }
        public string PasswordHash { get; set; }
        public ContactInfo ContactInfo { get; set; }

        [IgnoreDataMember]
        public List<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
