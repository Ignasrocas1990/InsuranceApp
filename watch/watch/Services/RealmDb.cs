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

    public class RealmDb
    {
        private static string MyRealmAppId = "application-1-luybv";
        //private Realm realm = null;
        private static RealmDb _db = null;
        private readonly string partition = "CustomerPartition";
        public static App RealmApp;
        private const string TAG = "mono-stdout";
        public event EventHandler LoggedInCompleted = delegate{ };
        public event EventHandler StopDataGathering = delegate{ };
        private static Func<String,float>toFloat =  x => float.Parse(x, CultureInfo.InvariantCulture.NumberFormat);

        private RealmDb()
        {
            try
            {
                RealmApp ??= App.Create(MyRealmAppId);
            }
            catch (Exception e)
            {
                Log.Verbose(TAG, $"Create(RealmApp) error : \n {e.Message}");
            }
        }

        public static RealmDb GetInstance()
        {
            if (_db is null)
            { 
                _db = new RealmDb();
            }
            return _db;
        }
        public async Task LogIn(string email, string password)
        {
            try
            {
                if (RealmApp.CurrentUser == null)
                {
                    var user =  await RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    if (user is null)
                    {
                        Log.Verbose(TAG, "fail to log in realm");
                        return;
                    }
                }
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LoggedInCompleted.Invoke(this, EventArgs.Empty);
                });
            }
            catch (Exception e)
            {
                Log.Verbose(TAG,$"LogIn,realm error : \n {e.Message}");
            }
        }
         public async Task AddMovData(List<string> dataToBeSaved)
        {
            try
            {
                //if (RealmApp is null) return;
                //if (RealmApp.CurrentUser is null) return;
                
                var  otherRealm =  await GetRealm();
                if (otherRealm is null) throw new Exception("AddMvData ::: Realm is null");
                otherRealm.Write( ()=>
                {
                    var movDataList = 
                        dataToBeSaved.Select(dataFullString => 
                            dataFullString.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                            .Select(sd => 
                                new MovData() 
                                    {AccData = new Acc() 
                                        {X = toFloat(sd[0]), Y = toFloat(sd[1]), Z = toFloat(sd[2])}, 
                                        Type = "step"}).ToList();
                    Log.Verbose(TAG,$"data date stamp is : {movDataList.First().DateTimeStamp}");//TODO Remove ===========
                    var customer = otherRealm.Find<Customer>(RealmApp.CurrentUser.Id);
                    if (customer is null) throw new Exception("AddMvData ::: Customer is null");
                    if (customer.DataSendSwitch is false)
                    {
                        StopDataGathering.Invoke(this, EventArgs.Empty);
                    }
                    otherRealm.Add(movDataList);
                    
                    var currentDate = DateTimeOffset.Now;
                    var rewardCount = customer.Reward.Count(r => r.FinDate != null
                                                                 && r.FinDate.Value.Month == currentDate.Month
                                                                 && r.FinDate.Value.Year == currentDate.Year);

                    var currentReward = customer.Reward.FirstOrDefault(r => r.FinDate == null && 
                                                                            r.DelFlag == false);
                    
                    if (currentReward != null && currentReward.MovData.Count <= 10000)
                    {
                        foreach (var d in movDataList)
                            currentReward.MovData.Add(d);
                        if (currentReward.MovData.Count >= 10000)
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
                    Log.Verbose(TAG, "Saved Data to Realm");
                });
            }
            catch (Exception e)
            {
                 // in-case connection loss ignore
                 Log.Verbose(TAG, $"Data is not saved {e.Message}");
            }
        }
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
                     switchOn = customer.DataSendSwitch;
                 });
             }
             catch (Exception e)
             {
                 Log.Verbose(TAG,e.Message);
             }

             return switchOn;
         }
         public async Task UpdateSwitch()
         {
             try
             {
                 var  otherRealm =  await GetRealm();
                 if (otherRealm is null) throw new Exception("AddMvData ::: Realm is null");
                 otherRealm.Write(() =>
                 {
                    var c = otherRealm.Find<Customer>(RealmApp.CurrentUser.Id);
                    c.DataSendSwitch = false;
                 });
             }
             catch (Exception e)
             {
                 Log.Verbose(TAG,e.Message);
             }
         }

        private async Task<Realm> GetRealm()
        {
            try
            {
               // if (!NetConnection()) return realm;
                
                return await Realm.GetInstanceAsync(
                    new PartitionSyncConfiguration(partition,RealmApp.CurrentUser,"RealmDb"));
            }
            catch (Exception e)
            {
                Log.Verbose(TAG,$"GetRealm,realm error : \n {e.Message}");
                Log.Verbose(TAG,$"GetRealm, inner exception : {e.InnerException}");
            }
            return null;
        }

        private static bool NetConnection()
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