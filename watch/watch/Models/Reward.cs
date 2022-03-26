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
using MongoDB.Bson;
using Realms;
using watch.Services;

namespace watch.Models
{
    public class Reward : RealmObject
    {
        [PrimaryKey] [MapTo("_id")] public ObjectId Id { get; set; } = ObjectId.GenerateNewId();

        public bool IsFinish { get; set; }
        public float? Cost { get; set; }
        public DateTimeOffset? FinDate { get; set; } = null;
        public DateTimeOffset? StartDate { get; set; } = DateTimeOffset.Now.DateTime;
        public bool? DelFlag { get; set; } = false;
        public IList<MovData> MovData { get; }

        public string Owner { get; set; } = RealmDb.RealmApp.CurrentUser.Id;

        [MapTo("_partition")] public string Partition { get; set; } = "CustomerPartition";
        
    }
}