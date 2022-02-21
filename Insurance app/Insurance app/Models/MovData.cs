using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class MovData : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        [MapTo("_partition")] public string Partition { get; set; } = "CustomerPartition";
        public DateTimeOffset? DateTimeStamp { get; set; } = DateTimeOffset.Now.DateTime;
        public bool? DelFlag { get; set; } = false;

        [Indexed]
        public string Owner { get; set; } = App.RealmApp.CurrentUser.Id;

        public Acc AccData { get; set; }
        public string Type { get; set; }


    }

    public class Acc : EmbeddedObject
    {
        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Z { get; set; }
    }
}