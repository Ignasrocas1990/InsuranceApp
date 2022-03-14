using System;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Claim : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        [MapTo("_partition")] public string Partition { get; set; } = "CustomerPartition";
        public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.Now.DateTime;
        public bool Accepted { get; set; }

        public string ExtraInfo { get; set; }

        public bool? DelFlag { get; set; } = false;

        public DateTimeOffset? CloseDate { get; set; } = null;
        public string HospitalPostCode { get; set; }
        public string PatientNr { get; set; }
        public string Type { get; set; }
        
        public string Owner { get; set; }
    }
}