using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Policy: RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        
        [MapTo("_partition")]
        public string Partition { get; set; }
        
        [MapTo("delFlag")]
        public bool? DelFlag { get; set; }
        
        [MapTo("price")]
        public double? Price { get; set; }
        
        [MapTo("startDate")]
        public DateTimeOffset StartDate { get; set; }
        
        [MapTo("status")]
        public bool? Status { get; set; }
        
        [MapTo("subscribe")]
        public bool? Subscribe { get; set; }
    }
}