using MongoDB.Bson;
using RobotsIntelect_WebApi.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RobotsIntelect_WebApi.Models
{
    public abstract class Document : IDocument
    {
        public ObjectId Id { get; set; }


        public DateTime CreatedAt => Id.CreationTime;
    }
}
