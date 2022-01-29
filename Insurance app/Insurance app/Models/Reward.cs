using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Service;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Reward : RealmObject
    {
        [PrimaryKey] [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public bool IsFinish { get; set; }
        public float? Cost { get; set; }
        public DateTimeOffset? FinDate { get; set; }
        public DateTimeOffset? StartDate { get; set; } =DateTime.Now;
        public bool? DelFlag { get; set; } = false;
        public IList<MovData> MovData { get; }

        [MapTo("_partition")]
        public string Partition { get; set; }

        public async Task AddMovData(List<MovData> movList)
        {
            var realm = RealmDb.GetInstance();
             await realm.AddMovData(movList, this);
        }
        

    }
}