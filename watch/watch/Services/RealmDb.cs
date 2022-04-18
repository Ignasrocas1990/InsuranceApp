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
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Android.Util;
using Realms;
using Realms.Sync;
using watch.Models;
using Xamarin.Essentials;

namespace watch.Services
{
    /// <summary>
    /// User to connect to Mongo cloud database
    /// and save accelerometer readings
    /// </summary>
    public class RealmDb
    {
        private const string MyRealmAppId = "application-1-luybv";
        private static RealmDb _db = null;
        private int stepsNeeded = 5;
        private const string Partition = "CustomerPartition";
        public static App RealmApp;
        private const string Tag = "mono-stdout";
        public  EventHandler LoggedInCompleted = delegate{ };
        public  EventHandler StopDataGathering = delegate{ };
        private static readonly Func<string,float>ToFloat =  x => float.Parse(x, CultureInfo.InvariantCulture.NumberFormat);
        private string email = "";
        private string pass = "";

        private RealmDb()
        {
            try
            {
                RealmApp ??= App.Create(MyRealmAppId);
            }
            catch (Exception e)
            {
                Log.Verbose(Tag, $"Create(RealmApp) error : \n {e.Message}");
            }
        }

        public static RealmDb GetInstance()
        {
            return _db ??= new RealmDb();
        }
        /// <summary>
        /// Login's to Realm database
        /// </summary>
        /// <param name="email">email passed via bluetooth or from SQL database</param>
        /// <param name="password">password passed via bluetooth or from SQL database</param>
        public async Task LogIn(string email, string password)
        {
            try
            {
                this.email = email;
                pass = password;
                if (RealmApp.CurrentUser == null)
                {
                    var user =  await RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    if (user is null)
                    {
                        Log.Verbose(Tag, "fail to log in realm");
                        return;
                    }
                }
                await MainThread.InvokeOnMainThreadAsync(() =>
                {
                    LoggedInCompleted.Invoke(this, EventArgs.Empty);
                });
            }
            catch (Exception e)
            {
                Log.Verbose(Tag,$"LogIn,realm error : \n {e.Message}");
            }
        }
        /// <summary>
        /// Adds movement data to Mongo, cloud database
        /// </summary>
        /// <param name="dataToBeSaved"></param>
         public async Task AddMovData(List<string> dataToBeSaved)
        {
            try
            {
                Log.Verbose(Tag,$"RealmApp.CurrentUser is null ?={RealmApp.CurrentUser is null}");
                var  otherRealm =  await GetRealm();
                /*
                if (otherRealm is null)
                {
                  await LogIn(email,pass);
                }
                */
                otherRealm.Write( ()=>
                {
                    var movDataList = 
                        dataToBeSaved.Select(dataFullString => 
                            dataFullString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                            .Select(sd => 
                                new MovData() 
                                    {AccData = new Acc() 
                                        {X = ToFloat(sd[0]), Y = ToFloat(sd[1]), Z = ToFloat(sd[2])}, 
                                        Type = "step"}).ToList();
                    Log.Verbose(Tag,$"data date stamp is : {movDataList.First().DateTimeStamp}");//TODO Remove ===========
                    
                    var customer = otherRealm.Find<Customer>(RealmApp.CurrentUser.Id);
                    if (customer is null) throw new Exception("AddMvData ::: Customer is null");
                    
                     
                    if (!GetTimeDifference(customer.DataSendSwitch))
                    {
                        StopDataGathering.Invoke(this, EventArgs.Empty);
                        return;
                    }
                    otherRealm.Add(movDataList);
                    
                    var currentDate = DateTimeOffset.Now;
                    var rewardCount = customer.Reward.Count(r => r.FinDate != null
                                                                 && r.FinDate.Value.Month == currentDate.Month
                                                                 && r.FinDate.Value.Year == currentDate.Year);

                    var currentReward = customer.Reward.FirstOrDefault(r => r.FinDate == null && 
                                                                            r.DelFlag == false);
                    
                    if (currentReward != null && currentReward.MovData.Count <= stepsNeeded)
                    {
                        foreach (var d in movDataList)
                            currentReward.MovData.Add(d);
                        if (currentReward.MovData.Count >= stepsNeeded)
                        {
                            currentReward.FinDate = currentDate;
                        }
                    }
                    else if (rewardCount < 25)
                    {
                        var cost = customer.Policy.Where(p => p.DelFlag == false)
                            .OrderByDescending(p => p.ExpiryDate).First().Price / 100;

                        var reward = otherRealm.Add(new Reward()
                        {
                            Owner = RealmApp.CurrentUser.Id,
                            Cost = cost
                        });
                        foreach (var d in movDataList)
                            reward.MovData.Add(d);
                        customer.Reward.Add(reward);
                    }
                    Log.Verbose(Tag, "Saved Data to Realm");
                    otherRealm.Refresh();
                });
            }
            catch (Exception e)
            {
                 // in-case connection loss ignore
                 Log.Verbose(Tag, $"Data is not saved {e.Message}");
            }
        }
        /// <summary>
        /// Checks the switch has been updated 10 min ago to false(stop monitoring data)
        /// </summary>
        /// <param name="dataSendSwitch">customer DataSendSwitch instance </param>
        /// <returns>on/off boolean</returns>
        private bool GetTimeDifference(DataSendSwitch dataSendSwitch)
        {
            if (dataSendSwitch.Switch) return true;

            var changeDate = dataSendSwitch.changeDate.ToUnixTimeSeconds();
            var now = DateTimeOffset.Now.ToUnixTimeSeconds();
            return now - changeDate >= 600;
        }

        /// <summary>
        /// Checks if customer still monitoring
        /// </summary>
        /// <returns>true if monitoring</returns>
         public async Task<bool> CheckIfMonitoring()
         {
             bool switchOn=false;
             try
             {
                 var  otherRealm =  await GetRealm();
                 if (otherRealm is null) throw new Exception("AddMvData ::: Realm is null");
                 otherRealm.Write(() =>
                 {
                   var customer =  otherRealm.Find<Customer>(RealmApp.CurrentUser.Id);
                   if (customer==null)
                   {
                       throw new Exception("No customer found = Switch is false");
                   }
                   var currentPolicy = customer?.Policy
                       ?.Where(p=> p.DelFlag == false && p.ExpiryDate > DateTimeOffset.Now)
                       .OrderByDescending(z => z.ExpiryDate).FirstOrDefault();
                   if (currentPolicy is null)
                   {
                       throw new Exception("No policy found (expired or not created) = Switch is false");
                   }
                     switchOn = customer.DataSendSwitch.Switch;
                 });
             }
             catch (Exception e)
             {
                 Log.Verbose(Tag,e.Message);
             }

             return switchOn;
         }
        /// <summary>
        /// updates customer monitoring switch
        /// </summary>
         public async Task UpdateSwitch(bool state)
         {
             try
             {
                 var  otherRealm =  await GetRealm();
                 if (otherRealm is null) throw new Exception("UpdateSwitch ::: Realm is null");
                 otherRealm.Write(() =>
                 {
                    var c = otherRealm.Find<Customer>(RealmApp.CurrentUser.Id);
                    
                    if (c is null) throw new Exception("UpdateSwitch ::: Customer is null");
                    var mySwitch = c.DataSendSwitch;
                    if (mySwitch is null) throw new Exception("UpdateSwitch ::: mySwitch is null");
                    
                    mySwitch.Switch = state;
                     mySwitch.changeDate = DateTimeOffset.Now;
                 });
             }
             catch (Exception e)
             {
                 Log.Verbose(Tag,e.Message);
             }
         }
        /// <summary>
        /// Gets an instance of Realm using partition and current user
        /// </summary>
        /// <returns>Realm instance</returns>
        private async Task<Realm> GetRealm()
        {
            try
            {
                var config = new SyncConfiguration(Partition,RealmApp.CurrentUser);
                return await Realm.GetInstanceAsync(config);
            }
            catch (Exception e)
            {
                Log.Verbose(Tag,$"GetRealm,realm error : {e.Message}");
                Log.Verbose(Tag,$"GetRealm, inner exception : {e.InnerException}");
                if (!NetConnection() || RealmApp.CurrentUser == null)
                {
                    Log.Verbose(Tag, $"network connection is not on{!NetConnection()}");
                    Log.Verbose(Tag, $"Realm is null ? {RealmApp.CurrentUser == null}");
                }
                Log.Verbose(Tag,$"Internet connection available/resting the user...");
                return await ResetLog();
            }
        }
        /// <summary>
        /// Fixing a Error: Websocket: Expected HTTP response 101 Switching Protocols
        /// Which logs out the user, logins the user and gets a new instance of realm
        /// </summary>
        /// <returns>Realm instance or Null</returns>
        private async Task<Realm> ResetLog()
        {
            try
            {
                 await RealmApp.CurrentUser.LogOutAsync();
                 await RealmApp.LogInAsync(Credentials.EmailPassword(email, pass));
                 return await Realm.GetInstanceAsync(
                     new PartitionSyncConfiguration(Partition,RealmApp.CurrentUser));
            }
            catch (Exception e)
            {
                Log.Verbose(Tag,$"ResetLog Failed with error ={e.Message}");
                Log.Verbose(Tag,$"ResetLog Failed with error ={e.InnerException}");
                return null;
            }
        }
        /// <summary>
        /// Checks if device has wifi/cellular internet connection
        /// </summary>
        /// <returns>boolean value true when internet connection found</returns>
        public static bool NetConnection()
        {
            var profiles = Connectivity.ConnectionProfiles;
            var connectionProfiles = profiles as ConnectionProfile[] ?? profiles.ToArray();
            if (connectionProfiles.Contains(ConnectionProfile.WiFi) || connectionProfiles.Contains(ConnectionProfile.Cellular))
            {
                return (Connectivity.NetworkAccess == NetworkAccess.Internet);
            }

            return false;
        }
    }
}