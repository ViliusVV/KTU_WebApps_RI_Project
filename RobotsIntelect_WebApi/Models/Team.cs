using MongoDB.Driver;
using RobotsIntelect_WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Models
{
    [BsonCollection("teams")]
    public class Team: Document
    {
        public string Name { get; set; }
        public string City { get; set; }
        public string Represents { get; set; }
        public List<string> TeamMembers { get; set; } = new List<string>(); // IDs only
        public List<string> Robots { get; set; } = new List<string>(); // IDs only
    }
}
