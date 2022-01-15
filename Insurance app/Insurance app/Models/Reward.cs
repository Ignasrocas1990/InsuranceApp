using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Reward : RealmObject
    {
        [PrimaryKey] [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        
        public int? Count { get; set; }
        public bool? DelFlag { get; set; }
        public DateTimeOffset? FinDate { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        
        [MapTo("_partition")]
        public string Partition { get; set; }

    }
}