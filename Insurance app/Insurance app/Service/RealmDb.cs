using System;
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
        
        public async Task<Customer> AddCustomer(Customer c,User user)
        {
            try
            {
                 var realm = await GetRealm(user);
                if (realm is null)
                {
                    Console.WriteLine("Couldn't get realm");
                    return null;
                }
                realm.Write(() =>
                {
                    realm.Add(c);
                    
                });
                Console.WriteLine("customer added");
                return  realm.Find<Customer>(c.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem adding customer : \n {e}");
                return null;
            }
           
        }
        
        public async Task<Customer> FindCustomer(User user)
        {
            try
            {
                var realm = await GetRealm(user);
                if (realm != null)
                {
                   var t = realm.Find<Customer>(user.Id);
                   return t;
                }


            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong : \n {e}");
            }
            return null;
        }
        
// --------------------------- Mov Data  methods --------------------------------     
        public async Task<List<MovData>> AddMovData(List<MovData> movList,Reward r,User user)
        {
            var realm = await GetRealm(user);
            if (realm is null) return null;
            try
            {
                 await realm.WriteAsync(realmTemp =>
                 {
                     var reward = realmTemp.Find<Reward>(r.Id);
                     if (reward !=null)
                         {
                             foreach (var mov in movList)
                             {
                                 reward.MovData.Add(mov);
                             }

                             return reward.MovData;
                         }
                     return null;

                 });
                 movList.Clear();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            return null;
        }
        public async Task<List<MovData>> AddMovData2(List<MovData> movList,User user)
        {
            var realm = await GetRealm(user);
            if (realm is null) return null;
            try
            {
                realm.Write(() =>
                {
                    var c = realm.Find<Customer>(user.Id);
                    if (c != null)
                    {
                        var reward = c.Reward.Where(r => r.IsFinish = false).FirstOrDefault();
                        if (reward !=null)
                        {
                            foreach (var mov in movList)
                            {
                                reward.MovData.Add(mov);
                            }

                            return reward.MovData;
                        }
                    }
                    
                    return null;

                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

            return null;
        }
//------------------------------------------------   reward methods ----------------------
        public async Task<Reward> AddNewReward(User user)
        {
           var realm =await GetRealm(user);
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
// -------------------------------Get Instance ----------------------------
        private async Task<Realm> GetRealm(User user)
        {
            try
            {
                /*
                var localConfig = new RealmConfiguration("RealmDBFile")
                {
                    SchemaVersion = 5,
                    ShouldDeleteIfMigrationNeeded = true
                };
                */
                    var config = new SyncConfiguration(user.Id,user);
                    return await Realm.GetInstanceAsync(config);
            }
            catch (Exception e)
            {
                Console.WriteLine($"realm instance error return null {e.Message}");
                Console.WriteLine($" inner exception : {e.InnerException}");
                return null;
            }
           
        }


   
    }
}