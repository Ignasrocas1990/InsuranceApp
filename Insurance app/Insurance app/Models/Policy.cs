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
using MongoDB.Bson;
using Realms;

namespace Insurance_app.Models
{
    /// <summary>
    /// Class representation of an object schema that is stored on Mongo/Realm
    /// </summary>
    public class Policy : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        public bool? DelFlag { get; set; } = false;
        public float? Price { get; set; }
        public float? PayedPrice { get; set; }
        public string Cover { get; set; }
        public int? HospitalFee { get; set; }
        public string Hospitals { get; set; }
        public string Plan { get; set; }
        public int? Smoker { get; set; }
        public DateTimeOffset? ExpiryDate { get; set; }
        public bool? UnderReview { get; set; }
        public DateTimeOffset? UpdateDate { get; set; }
        public string Owner { get; set; }
        [MapTo("_partition")] public string Partition { get; set; } = "CustomerPartition";
    }
   
    
}