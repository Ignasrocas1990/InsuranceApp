using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Reward : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        [MapTo("_partition")] public string Partition { get; set; }

        [MapTo("count")] public int? Count { get; set; }

        

        [MapTo("finDate")] public DateTimeOffset? FinDate { get; set; }

        [MapTo("startDate")] public DateTimeOffset? StartDate { get; set; }
        
        [MapTo("delFlag")] public bool? DelFlag { get; set; } = false;
    }
}