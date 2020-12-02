using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Models.Interfaces
{
    public interface IMongoDbSettings
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
    }
}
