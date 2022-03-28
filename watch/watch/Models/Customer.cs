/*
    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

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
        
        public DataSendSwitch DataSendSwitch { get; set; }
        public IList<Policy> Policy { get; }
        public IList<Reward> Reward { get; }
        public IList<Claim> Claim { get; }

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
    public class DataSendSwitch  : EmbeddedObject
    {
        public bool Switch { get; set; } = false;
        public DateTimeOffset changeDate { get; set; } = DateTimeOffset.Now;
    }
}