using System;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class ClientActivity : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public string Type { get; set; }
        public string ActivityOwnerId { get; set; }
        public DateTimeOffset Date { get; set; } = DateTimeOffset.Now;
        public bool DelFlag { get; set; } = false;
    }
}