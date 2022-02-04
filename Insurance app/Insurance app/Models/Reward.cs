using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Service;
using MongoDB.Bson;
using Realms;
using Realms.Sync;

namespace Insurance_app.Models
{
    public class Reward : RealmObject
    {
        [PrimaryKey] [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public bool IsFinish { get; set; }
        public float? Cost { get; set; }
        public DateTimeOffset? FinDate { get; set; } = null;
        public DateTimeOffset? StartDate { get; set; } =DateTime.Now;
        public bool? DelFlag { get; set; } = false;
        public IList<MovData> MovData { get; }

        [MapTo("_partition")][Required] public string Partition { get; set; }

        public async Task AddMovData(ConcurrentQueue<MovData> newMovDataList,User user)
        {
            RealmDb db = new RealmDb();
            await db.AddMovData2(newMovDataList,user);
        }
    }
}