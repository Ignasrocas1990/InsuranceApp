using System;
using System.Collections.Generic;
using Realms;
using watch.Services;

namespace watch.Models
{
    public class Customer : RealmObject
    {
        public Customer() { }
        [PrimaryKey] [MapTo("_id")] [Required] public string Id { get; set; } = RealmDb.RealmApp.CurrentUser.Id;
        public Address Address { get; set; }
        
        public DateTimeOffset? Dob { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string PhoneNr { get;set; }
        public string Email { get; set; }
        
        public IList<Policy> Policy { get; }
        public IList<watch.Models.Reward> Reward { get; }
        public IList<watch.Models.Claim> Claim { get; }

        public bool DataSendSwitch { get; set; } = false;
        public bool? DelFlag { get; set; } = false;
        [MapTo("_partition")] public string Partition { get; set; } = "CustomerPartition";
    }
    public class Address  : EmbeddedObject
    {
        public string City { get; set; } = "";
        public string Country { get; set; } = "";
        public string County { get; set; } = "";
        public int? HouseN { get; set; } = 0;
        public string PostCode { get; set; } = "";
        public string Street { get; set; } = "";
    }
}