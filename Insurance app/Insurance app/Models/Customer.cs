using System;
using System.Collections.Generic;
using System.Security.Claims;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Customer : RealmObject
    {
        [PrimaryKey][Required]
        [MapTo("_id")]
        public string Id { get; set; }
        
        [MapTo("_partition")] public string Partition { get; set; }
        
        [MapTo("email")] public string Email { get; set; }
        
        [MapTo("password")] public string Password { get; set; }
        
        [MapTo("age")] public int? Age { get; set; }
        
        [MapTo("name")] public string Name { get; set; }

        [MapTo("address")] public Address Address { get; set; }
        
        [MapTo("claim")] public Claim Claim { get; set; }
        
        [MapTo("policy")] public Policy Policy { get; set; }

        [MapTo("movData")] public IList<MovData> MovData { get; } = new List<MovData>();
        

        [MapTo("rewards")] public IList<Reward> Rewards { get; } = new List<Reward>();

        
        [MapTo("delFlag")] public bool? DelFlag { get; set; } = false;


    }
    public class Address  : EmbeddedObject
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        [MapTo("House_n")] public int? HouseN { get; set; }
        [MapTo("Post_Code")] public string PostCode { get; set; }
        public string Street { get; set; }
    }
}