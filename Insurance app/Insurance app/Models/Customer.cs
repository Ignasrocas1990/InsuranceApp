using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Customer : RealmObject
    {
        [PrimaryKey]
        [MapTo("_id")]
        public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        
        [MapTo("_partition")]
        public string Partition { get; set; }
        
        [MapTo("name")]
        public string Name { get; set; }
        
        [MapTo("password")]
        public string Password { get; set; }
        
        [MapTo("username")]
        public string Username { get; set; }
        
        [MapTo("age")]
        public int? Age { get; set; }
        
        [MapTo("claim")] public Claim MyClaim { get; set; } = null;

        [MapTo("address")] public Customer_address Address { get; set; } = null;

        [MapTo("movData")] public IList<MovData> MovData { get;} = new List<MovData>(10);

        [MapTo("policy")] public Policy MyPolicy { get; set; } = null;

        [MapTo("rewards")] public IList<Reward> Rewards { get; } = null;

        [MapTo("delFlag")] public bool? DelFlag { get; set; } = false;

    }
    public class Customer_address  : EmbeddedObject
    {
        public string City { get; set; }
        
        public string Country { get; set; }
        
        public string County { get; set; }
        
        [MapTo("House_n")]
        public int? HouseN { get; set; }
        
        [MapTo("Post_Code")]
        public string PostCode { get; set; }
        
        public string Street { get; set; }
    }
}