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
using System.Threading.Tasks;
using System.Timers;
using Insurance_app.Models;
using MongoDB.Bson;
using Realms;
using Realms.Sync;

namespace Insurance_app.Service
{
    /// <summary>
    /// Background service Singleton that every 7 seconds checks
    /// Database and notifies the Hove View Model so the steps can be updated
    /// </summary>
    public class WatchService
    {
        private static bool _set;
        private static readonly Timer Timer = new Timer {Interval = 7000, AutoReset = true};
        public ObjectId CurrentRewardId { get; set; }
        public static bool State;
        public event EventHandler<StepArgs> StepCheckedEvent = delegate {  };
        private static WatchService _service;
        
        private WatchService()
        {
            if (_set)
            {
                _set = true;
                return;
            }
            Timer.Elapsed += (async (s,e) =>
            {
                await CheckDatabase();
            });
        }
        public static WatchService GetInstance()
        {
            return _service ??= new WatchService();
        }
        /// <summary>
        /// Toggles the recurring timer start/stop
        /// </summary>
        public static void ToggleListener()
        {
            if (State)
            {
                State = false;
                Timer.Stop();
            }
            else
            {
                State = true;
                Timer.Start();
            }
        }
        
        public static void StopListener() => Timer.Stop();
        public static void StartListener() => Timer.Start();

        /// <summary>
        /// Initializes a realm and checks for updated mov data
        /// </summary>
        private async Task CheckDatabase()
        {
            var steps = 0;
            try
            {
                var config = new SyncConfiguration(await GetPartition(),App.RealmApp.CurrentUser);
                var realm= await Realm.GetInstanceAsync(config);
                if (realm is null) throw new Exception("Realm is null in WatchService");
                realm.Write(() =>
                {
                    steps = realm.Find<Reward>(CurrentRewardId).MovData.Count;
                });
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            Console.WriteLine("steps invoked");
            StepCheckedEvent.Invoke(this,new StepArgs(){Steps=steps});
        }
        private static async Task<string> GetPartition() => (await App.RealmApp.CurrentUser.Functions.CallAsync("getPartition")).AsString;
    }
    public class StepArgs : EventArgs { internal int Steps { get; set; }}
}