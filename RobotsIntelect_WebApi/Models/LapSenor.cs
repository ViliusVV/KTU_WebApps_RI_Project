using RobotsIntelect_WebApi.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Models
{
    [BsonCollection("lapSensors")]
    public class LapSensor : Document
    {
        public string SensorName { get; set; }
        public RefreshToken ApiKey { get; set; }
    }
}
