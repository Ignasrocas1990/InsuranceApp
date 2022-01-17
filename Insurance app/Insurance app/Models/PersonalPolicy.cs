using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class PersonalPolicy : RealmObject
    {
        [PrimaryKey] [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public float? Price { get; set; }
        public bool? DelFlag { get; set; }

        public int? Cover { get; set; }
        public int? HospitalFee { get; set; }
        public int? Hospitals { get; set; }
        public int? Plan { get; set; }
        public int? Smoker { get; set; }
        
        public DateTimeOffset? StartDate { get; set; }
        public bool? Status { get; set; }
        
        [MapTo("_partition")]
        public string Partition { get; set; }
        
    }
   
    
}