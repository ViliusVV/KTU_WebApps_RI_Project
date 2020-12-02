using RobotsIntelect_WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Models
{
    [BsonCollection("participants")]
    public class Participant: Document
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public ContactInfo ContactInfo { get; set; }
    }
}
