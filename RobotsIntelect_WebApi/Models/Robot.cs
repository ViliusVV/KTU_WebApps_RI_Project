using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using RobotsIntelect_WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Models
{
    [BsonCollection("robots")]
    public class Robot: Document
    {
        public string Name { get; set; }
        public string Type { get; set; }

        public List<LapTime> LapTimes { get; set; }

    }
}
