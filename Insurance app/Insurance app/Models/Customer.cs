using System;
using System.Collections.Generic;
using System.Security.Claims;
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    public class Customer : RealmObject
    {
        public Customer() { }
        [PrimaryKey][MapTo("_id")][Required]
        public string Id { get; set; }
        public Address Address { get; set; }
        public int? Age { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNr { get;set; }
        public string Email { get; set; }
        
        public PersonalPolicy Policy { get; set; }
        public IList<Reward> Reward { get; }
        public Claim Claim { get; set; }

        public bool? DelFlag { get; set; } = false;
        [MapTo("_partition")]
        public string Partition { get; set; }
    }
    public class Address  : EmbeddedObject
    {
        public string City { get; set; }
        public string Country { get; set; }
        public string County { get; set; }
        public int? HouseN { get; set; }
        public string PostCode { get; set; }
        public string Street { get; set; }
    }
}