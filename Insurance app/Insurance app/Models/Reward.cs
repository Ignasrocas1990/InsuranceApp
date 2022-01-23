using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Reward : RealmObject
    {
        [PrimaryKey] [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public bool IsFinish { get; set; }
        public float Amount { get; set; }
        public DateTimeOffset? FinDate { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public bool? DelFlag { get; set; }
        public IList<MovData> MovData { get; }

        [MapTo("_partition")]
        public string Partition { get; set; }
        
        //var movData = Reward.MovData.Where(md => md.Id == Id).ToList();

    }
}