using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Policy : RealmObject
    {
        [PrimaryKey] [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public bool? DelFlag { get; set; } = false;

        public float? Price { get; set; }

        public float? PayedPrice { get; set; }
        public string Cover { get; set; }
        public int? HospitalFee { get; set; }
        public string Hospitals { get; set; }
        public string Plan { get; set; }
        public int? Smoker { get; set; }

        public DateTimeOffset? ExpiryDate { get; set; }
        public bool? UnderReview { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }

        public string Owner { get; set; }

        [MapTo("_partition")] public string Partition { get; set; } = "CustomerPartition";

    }
   
    
}