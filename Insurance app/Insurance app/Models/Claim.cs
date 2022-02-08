using System;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Claim : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        [MapTo("_partition")] [Required] public string Partition { get; set; } = App.RealmApp.CurrentUser.Id;
        public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.Now;
        public bool OpenStatus { get; set; } = false;
        public bool? DelFlag { get; set; } = false;
        
        public DateTimeOffset? CloseDate { get; set; }
        public string HospitalPostCode { get; set; }
        public string PatientNr { get; set; }
        public string Type { get; set; }
        



    }
}