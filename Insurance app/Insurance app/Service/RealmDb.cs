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
        private Realm realm = null;
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
                if (realm is null)
                {
                    Console.WriteLine("Couldn't get realm");
                    return;
                }
                realm.Write(() =>
                {
                    realm.Add(c,true);
                    
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
                if (realm != null)
                {
                    realm.Write(() =>
                    {
                        c = realm.All<Customer>().FirstOrDefault(u => u.Id == user.Id);
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
            if (realm is null)
            {
                Console.WriteLine("AddMovData2 realm is null");
                return;
            }
            try
            {
                realm.Write(() =>
                {
                    //var c = _realm.Find<Customer>(user.Id);
                   // if (c != null)
                   // {
                        
                        //var reward = c.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                        var reward = realm.All<Reward>()
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
                realm.Write(() =>
                {
                    int count = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        var prev1 = prev;
                        var now1 = now;
                        count = realm
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



//------------------------------------------------   reward methods ----------------------
        public async Task<Reward> AddNewReward(User user)
        {
            await GetRealm(user);
            if (realm is null) return null;
            try
            {
                Reward reward = null;
                realm.Write(()=>
                {
                    
                    var c = realm.Find<Customer>(user.Id);
                    if (c!=null)
                    {
                        var r = realm.Add(new Reward()
                        {
                            Partition = user.Id,
                            Cost = (c.Policy.Price/100)
                        });
                   
                        c.Reward.Add(r);
                        reward = realm.Find<Reward>(r.Id);
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

// ---------------------------- Claim methods --------------------------

        public async Task AddClaim( string hospitalCode,string patientNr,string type,bool openStatus,User user)
        {
            try
            {
                await GetRealm(user);
                realm.Write(() =>
                {
                    var customer = realm.Find<Customer>(user.Id);
                    if (customer != null)
                    {
                        customer.Claim.Add(realm.Add(new Claim()
                        {
                            HospitalPostCode = hospitalCode,
                            PatientNr = patientNr,
                            Type = type,
                            OpenStatus = openStatus

                        },true));
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        public async Task<List<Claim>> GetClaims(User user)
        {
            await GetRealm(user);
            List<Claim> claims = new List<Claim>();
            try
            {
                if (realm !=null)
                {
                    realm.Write(() =>
                    {
                        claims = realm.All<Claim>().Where(c => c.Partition == user.Id).ToList();
                    });
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return claims;
        }
// -------------------------------- support methods ---------------------------------        
        public async Task CleanDatabase(User user)//TODO remove this when submitting
        {
            try
            {
                await GetRealm(user);
                realm.Write(() =>
                {
                    realm.RemoveAll<Claim>();
                    realm.RemoveAll<Customer>();
                    realm.RemoveAll<MovData>();
                    realm.RemoveAll<PersonalPolicy>();
                    realm.RemoveAll<Reward>();
                    //var remList = _realm.All<MovData>().Where(m => m.Partition == user.Id);
                    //_realm.RemoveRange(remList);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           

        }
        private async Task GetRealm(User user)
        {
            try
            {
                realm = null;

                var config = new SyncConfiguration(user.Id,user);
                realm = await Realm.GetInstanceAsync(config);
            }
            catch (Exception e)
            {
                Console.WriteLine($"realm instance error return null {e.Message}");
                Console.WriteLine($" inner exception : {e.InnerException}");
                realm = null;
            }
        }

        public void Dispose()
        {
            try
            {
                if (realm is null) return;
                realm.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

    }
}