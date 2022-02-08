using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Realms;
using Realms.Sync;

namespace Insurance_app.Service
{
    public class RealmDb
    {
        private Realm _realm = null;
        public RealmDb() {}
//------------------------- app Access Methods ---------------------------------------
        public async Task<string> Register(String email, String password)
        {
            try
            {
              await App.RealmApp.EmailPasswordAuth.RegisterUserAsync(email, password);
              return "success";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
        }
//------------------------------------- Customer methods ---------------------
        
        public async Task AddCustomer(Customer c,User user)
        {
            await GetRealm(user);
            try
            {
                if (_realm is null)
                {
                    Console.WriteLine("Couldn't get realm");
                    return;
                }
                _realm.Write(() =>
                {
                    _realm.Add(c);
                    
                });
                Console.WriteLine("customer added");
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem adding customer : \n {e}");
            }
        }
        
        public async Task<Customer> FindCustomer(User user)
        {
            try
            {
                Customer c = null;
                await GetRealm(user);
                if (_realm != null)
                {
                    _realm.Write(() =>
                    {
                        c = _realm.All<Customer>().FirstOrDefault(u => u.Id == user.Id);
                        //c = realm.Find<Customer>(user.Id);

                    });
                }
                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong : \n {e}");
                return null;

            }
        }
        
// --------------------------- Mov Data  methods --------------------------------     
        public async Task AddMovData(ConcurrentQueue<MovData> movList,User user)
        {
             await GetRealm(user);
            if (_realm is null)
            {
                Console.WriteLine("AddMovData2 realm is null");
                return;
            }
            try
            {
                _realm.Write(() =>
                {
                    //var c = _realm.Find<Customer>(user.Id);
                   // if (c != null)
                   // {
                        
                        //var reward = c.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                        var reward = _realm.All<Reward>()
                            .Where(r => r.Partition == user.Id && r.FinDate == null)
                            .FirstOrDefault();
                        if (reward !=null)
                        {
                            foreach (var mov in movList)
                            {
                                reward.MovData.Add(mov);
                            }
                        }
                  //  }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<Dictionary<string,int>> GetWeeksMovData(User user)
        {
            //int[] chartEntries = {0,0,0,0,0,0,0};
            Dictionary<string, int> chartEntries = new Dictionary<string, int>();
            try
            {
                var hourDif = 24;
                var now = DateTime.Now;
                var prev = DateTime.Now.AddHours(-hourDif);
                
                await GetRealm(user);
                _realm.Write(() =>
                {
                    int count = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        var prev1 = prev;
                        var now1 = now;
                        count = _realm
                            .All<MovData>().Count(m => m.Partition == user.Id && m.DateTimeStamp <= now1 && 
                                                       m.DateTimeStamp > prev1);
                        chartEntries.Add(now.DayOfWeek.ToString(),count);
                        now = prev;
                        prev = prev.AddHours(-hourDif);
                    }
                });
                return chartEntries;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            
        }

        public async Task DelAllMovData(User user)
        {
            try
            {
                await GetRealm(user);
                _realm.Write(() =>
                {
                    var remList = _realm.All<MovData>().Where(m => m.Partition == user.Id);
                    _realm.RemoveRange(remList);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           

        }

//------------------------------------------------   reward methods ----------------------
        public async Task<Reward> AddNewReward(User user)
        {
            await GetRealm(user);
            if (_realm is null) return null;
            try
            {
                Reward reward = null;
                _realm.Write(()=>
                {
                    
                    var c = _realm.Find<Customer>(user.Id);
                    if (c!=null)
                    {
                        var r = _realm.Add(new Reward()
                        {
                            Partition = user.Id,
                            Cost = (c.Policy.Price/100)
                        });
                   
                        c.Reward.Add(r);
                        reward = _realm.Find<Reward>(r.Id);
                    }
                });
                return reward;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
// -------------------------------Get Instance ----------------------------
        private async Task GetRealm(User user)
        {
            try
            {
                _realm = null;

                var config = new SyncConfiguration(user.Id,user);
                    _realm = await Realm.GetInstanceAsync(config);
            }
            catch (Exception e)
            {
                Console.WriteLine($"realm instance error return null {e.Message}");
                Console.WriteLine($" inner exception : {e.InnerException}");
                _realm = null;
            }
        }
        public void StopSync()
        {
            if (_realm!=null)
                _realm.SyncSession.Stop();
            if (_realm != null) _realm.Dispose();
        }


        public async Task AddClaim(User user,Claim claim)
        {
            try
            {
                await GetRealm(user);
                _realm.Write(() =>
                {
                    var c = _realm.Find<Customer>(user.Id);
                    if (c is null) return;
                    c.Claim.Add(_realm.Add(claim));

                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}