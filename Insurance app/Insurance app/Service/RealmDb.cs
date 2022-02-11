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
    public class RealmDb : IDisposable
    {
        private Realm realm = null;
        readonly Thread curThread = Thread.CurrentThread;
        private static RealmDb db = null;

        private RealmDb()
        {
        }

        public static RealmDb GetInstance()
        {
            return db ?? (db = new RealmDb());
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
                realm?.Write(() =>
                {
                    c = realm.All<Customer>().FirstOrDefault(u => u.Id == user.Id);
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
               realm.Write(() =>
               {
                   var customer = realm.All<Customer>().FirstOrDefault(c => c.Id == user.Id);
                   if (customer!=null)
                   {
                       customer.Age = age;
                       customer.Name = name;
                       customer.LastName = lastName;
                       customer.PhoneNr = phoneNr;
                       customer.Email = email;
                       customer.Address = address;
                   }
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
                    var reward = realm.All<Reward>().FirstOrDefault(r => r.Partition == user.Id && r.FinDate == null);
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

                    var customer = realm.All<Customer>().FirstOrDefault(c => c.Id == user.Id);
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
        private async Task GetRealm(User user)//ne
        {
           
            try
            {
                realm = null;
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
                Console.WriteLine($"GetRealm,realm error : \n {e.Message}");
                Console.WriteLine($"GetRealm, inner exception : {e.InnerException}");
                //await GetDifferentRealm(user);

            }
        }

        private async Task GetDifferentRealm(User user)
        {
            try
            {
                realm = null;
                var config = new PartitionSyncConfiguration(user.Id, user);
                realm = await Realm.GetInstanceAsync(config);
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetDifferentRealm, error \n {e.Message}");
                Console.WriteLine($"GetDifferentRealm, inner exception : \n {e.InnerException}");
                realm = null;
            }
        }
        

        public void Dispose()
        {
            try
            {
                if (realm is null) return;
                realm.Dispose();
                realm = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        
    }
}