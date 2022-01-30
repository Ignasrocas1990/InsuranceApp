using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Claim : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public DateTimeOffset? CloseDate { get; set; }
        public bool? DelFlag { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        [MapTo("_partition")][Required]
        public string Partition { get; set; }

    }
}