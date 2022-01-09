using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class MovData : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();


        [MapTo("_partition")] public string Partition { get; set; }
        
        [MapTo("movType")] public string MovType { get; set; }

        [MapTo("timeStamp")] 
        public long? TimeStamp { get; set; } = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds();
        
        public Acc Acc { get; set; }
        public Gyro Gyro { get; set; }
        
        [MapTo("delFlag")] public bool? DelFlag { get; set; } = false;

    }

    public class Acc : EmbeddedObject
    {
        [MapTo("x")] public double? X { get; set; }
        [MapTo("y")] public double? Y { get; set; }
        [MapTo("z")] public double? Z { get; set; }
    }

    public class Gyro : EmbeddedObject
    {
        [MapTo("x")] public double? X { get; set; }
        [MapTo("y")] public double? Y { get; set; }
        [MapTo("z")] public double? Z { get; set; }
    }
}