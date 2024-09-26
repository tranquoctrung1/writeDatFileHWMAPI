﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WriteDataFileByMongo.Models
{
    [BsonIgnoreExtraElements]
    public class DataLoggerModel
    {
        public ObjectId Id { get; set; }
        public Nullable<DateTime> TimeStamp { get; set; }
        public Nullable<double> Value { get; set; }
    }
}
