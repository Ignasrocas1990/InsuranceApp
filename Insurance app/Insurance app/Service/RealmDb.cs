using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Insurance_app.Models;
using Realms;
using Realms.Exceptions;
using Realms.Sync;
using Xamarin.Essentials;

namespace Insurance_app.Service
{
    public class RealmDb
    {
        private Realm realm = null;
        //readonly Thread curThread = Thread.CurrentThread;
        private static RealmDb _db = null;

        private RealmDb() {}

        public static RealmDb GetInstance()
        {
            return _db ?? (_db = new RealmDb());
        }
        
        
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
            try
            {
                await GetRealm(user);
                if (realm is null) throw new Exception("AddCustomer, real is null");
                
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
            Customer c = null;
            try
            {
                await GetRealm(user);
                if (realm is null) throw new Exception("FindCustomer, real is null");
                realm.Write(() =>
                {
                    c = realm.All<Customer>().FirstOrDefault(u => u.Id == user.Id && u.DelFlag == false);
                });
                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong : \n {e}");
                return null;

            }
        }
        public async Task UpdateCustomer(int age, string name, string lastName, 
            string phoneNr, string email, Address address, User user)
        {
            try
            {
               await GetRealm(user);
               if (realm is null) throw new Exception("UpdateCustomer, real is null");
               realm.Write(() =>
               {
                   var customer = realm.All<Customer>().FirstOrDefault(c => c.Id == user.Id);
                   if (customer == null) return;
                   customer.Age = age;
                   customer.Name = name;
                   customer.LastName = lastName;
                   customer.PhoneNr = phoneNr;
                   customer.Email = email;
                   customer.Address = address;
               });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error when updating customer \n{e}");
            }
            
        }
        
// --------------------------- Mov Data  methods --------------------------------     
        public async Task AddMovData(ConcurrentQueue<MovData> movList,User user)
        {
            try
            {
                await GetRealm(user);
                if (realm is null)
                {
                    Console.WriteLine("AddMovData2 realm is null");
                    return;
                }
                realm.Write(() =>
                {
                    var reward = realm.All<Reward>().FirstOrDefault(
                        r => r.Partition == user.Id && r.FinDate == null && r.DelFlag == false);
                    if (reward != null)
                    {
                        foreach (var mov in movList)
                        {
                            reward.MovData.Add(mov);
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<Dictionary<string,int>> GetWeeksMovData(User user)
        {
            Dictionary<string, int> chartEntries = new Dictionary<string, int>();
            try
            {
                var hourDif = 24;
                var now = DateTime.Now;
                var prev = DateTime.Now.AddHours(-hourDif);
                
                await GetRealm(user);
                if (realm is null)
                    throw new Exception("real null");
                realm.Write(() =>
                {
                    int count = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        var prev1 = prev;
                        var now1 = now;
                        count = realm
                            .All<MovData>().Count(m => m.Partition == user.Id && m.DateTimeStamp <= now1 && 
                                                       m.DateTimeStamp > prev1 && m.DelFlag == false);
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
            Reward reward = null;
            try
            {
                await GetRealm(user);
                if (realm is null)
                    throw new Exception("real, AddNewReward, null");
                realm.Write(()=>
                {

                    var customer = realm.All<Customer>().FirstOrDefault(c => c.Id == user.Id && c.DelFlag == false);
                    if (customer!=null)
                    {
                        var cost = customer.Policy.Price / 100;
                        var r = realm.Add(new Reward()
                        {
                            Partition = user.Id,
                            Cost = cost
                        });
                   
                        customer.Reward.Add(r);
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
        public async Task<float> GetTotalRewards(User user)
        {
            float totalEarnings = 0;
            try
            {
                await GetRealm(user);
                if (realm is null) throw new Exception("getTotalRewards realm null my exception");
                realm.Write(() =>
                {
                    var rewards = realm.All<Reward>().Where(r => r.Partition == user.Id &&
                                                                 r.FinDate != null && r.DelFlag == false).ToList();
                    totalEarnings += rewards.Where(r => r.Cost != null).Sum(r =>
                    {
                        if (r.Cost != null) return (float) r.Cost;
                        return 0;
                    });
                });
                return totalEarnings;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return totalEarnings;
            }
        }

// ---------------------------- Claim methods --------------------------

        public async Task AddClaim( string hospitalCode,string patientNr,string type,bool openStatus,User user)
        {
            try
            {
                await GetRealm(user);
                if (realm is null)
                    throw new Exception(" AddClaim realm null");
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
            List<Claim> claims = new List<Claim>();
            try
            {
                await GetRealm(user);
                if (realm is null)
                    throw new Exception(" GetClaims realm null");
                

                realm.Write(() =>
                {
                    claims = realm.All<Claim>().Where(c => c.Partition == user.Id && c.DelFlag == false).ToList();
                });
                
                
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
                if (realm is null)
                    throw new Exception(" CleanDatabase realm null");
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
        private async Task GetRealm(User user)//ne
        {
           
            try
            {
                var config = new PartitionSyncConfiguration(user.Id, user);
                realm = await Realm.GetInstanceAsync(config);
                /*
                if (!curThread.Equals(Thread.CurrentThread))
                {
                    realm = null;
                    Console.WriteLine("different thread");
                }
                */
                
            }
            catch (Exception e)
            {
                realm = null;
                Console.WriteLine($"GetRealm,realm error : \n {e.Message}");
                Console.WriteLine($"GetRealm, inner exception : {e.InnerException}");
            }
        }
        public void Dispose()
        {
            try
            {
                if (realm is null) return;
                realm.SyncSession.Stop();
               
                if (!realm.IsClosed)
                {
                    realm.Dispose();
                }
                realm = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



    }
}