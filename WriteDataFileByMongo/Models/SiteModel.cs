using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteDataFileByMongo.Models
{
    [BsonIgnoreExtraElements]
    public class SiteModel
    {
        public ObjectId Id { get; set; }
        public string SiteId { get; set; }
        public string Location { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public string DisplayGroup { get; set; }
        public string LoggerId { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public string Available { get; set; }

    }
}
