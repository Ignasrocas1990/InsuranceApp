using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;
using watch.Services;


namespace watch.Models
{
    public class Reward : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public bool IsFinish { get; set; }
        public float? Cost { get; set; }
        public DateTimeOffset? FinDate { get; set; } = null;
        public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.Now.DateTime;
        public bool? DelFlag { get; set; } = false;
        public IList<MovData> MovData { get; }

        public string Owner { get; set; } = RealmDb.RealmApp.CurrentUser.Id;

        [MapTo("_partition")] public string Partition { get; set; } = "CustomerPartition";
        
    }
}