﻿/*
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

using Realms;

namespace Insurance_app.Models
{
    /// <summary>
    /// Class representation of an object schema that is stored on Mongo/Realm
    /// </summary>
    public class Client : RealmObject
    {
        public Client() { }
        [PrimaryKey] [MapTo("_id")] public string Id { get; set; } = App.RealmApp.CurrentUser.Id;
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyCode { get; set; }
        public bool DelFlag { get; set; } = false;
        

    }
}