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
    public class MovData : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();
        [MapTo("_partition")] public string Partition { get; set; } = "CustomerPartition";
        public DateTimeOffset? DateTimeStamp { get; set; } = DateTimeOffset.Now;
        public bool? DelFlag { get; set; } = false;

        [Indexed]
        public string Owner { get; set; } = App.RealmApp.CurrentUser.Id;

        public Acc AccData { get; set; }
        public string Type { get; set; }


    }

    public class Acc : EmbeddedObject
    {
        public float? X { get; set; }
        public float? Y { get; set; }
        public float? Z { get; set; }
    }
}