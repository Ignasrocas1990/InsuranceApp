﻿using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class PersonalPolicy : RealmObject
    {
        [PrimaryKey] [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public bool? DelFlag { get; set; } = false;

        public float? Price { get; set; }
        public int? Cover { get; set; }
        public int? HospitalFee { get; set; }
        public int? Hospitals { get; set; }
        public int? Plan { get; set; }
        public int? Smoker { get; set; }

        public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.Now.DateTime;
        public bool? Updating { get; set; }

        [MapTo("_partition")] [Required] public string Partition { get; set; } = App.RealmApp.CurrentUser.Id;

    }
   
    
}