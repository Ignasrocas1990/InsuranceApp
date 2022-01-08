using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class MovData : RealmObject
    {
        
        [PrimaryKey] [MapTo("_id")] 
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        
        public MovData_Acc Acc { get; set; }
        
        public MovData_Gyro Gyro { get; set; }
        
        [MapTo("_partition")] 
        public string Partition { get; set; }

        [MapTo("delFlag")] public bool? DelFlag { get; set; } = false;
        
        [MapTo("movType")] 
        public string MovType { get; set; }
        
        [MapTo("timeStamp")]
        public long TimeStamp { get; set; }
    }
    public class MovData_Acc : EmbeddedObject
    {
        [MapTo("x")]
        public double? X { get; set; }
        [MapTo("y")]
        public double? Y { get; set; }
        [MapTo("z")]
        public double? Z { get; set; }
    }
    public class MovData_Gyro : EmbeddedObject
    {
        [MapTo("x")]
        public double? X { get; set; }
        [MapTo("y")]
        public double? Y { get; set; }
        [MapTo("z")]
        public double? Z { get; set; }
    }
}