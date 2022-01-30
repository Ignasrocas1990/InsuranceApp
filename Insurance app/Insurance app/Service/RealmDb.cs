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
        private Realm Realm { set; get; }
        private static User _user;
        private static string _partition;
        private static RealmDb _realmDb = null;
        private RealmDb() {}

        public static RealmDb GetInstance()
        {
            if (_realmDb is null)
            {
                _realmDb = new RealmDb();
            }

            return _realmDb;
        }

        public void SetUser(User user)
        {
            _user = user;
            _partition = user.Id;
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
        
        public async Task<Customer> AddCustomer(Customer c)
        {
            try
            {
                await GetRealm();
                if (Realm is null)
                {
                    Console.WriteLine("Couldn't get realm");
                    return null;
                }
                Realm.Write(() =>
                {
                    Realm.Add(c);
                    
                });
                Console.WriteLine("customer added");
                return  Realm.Find<Customer>(c.Id);
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem adding customer : \n {e}");
                return null;
            }
           
        }
        
        public async Task<Customer> FindCustomer()
        {
            try
            {
                await GetRealm();
                if (Realm != null)
                {
                    return Realm.Find<Customer>(_user.Id);
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong : \n {e}");
            }
            return null;
        }
        
// --------------------------- Mov Data  methods --------------------------------     
        public async Task<List<MovData>> AddMovData(List<MovData> movList)
        {
            await GetRealm();
            if (Realm is null) return null;
            try
            {
                 await Realm.WriteAsync(realm =>
                 {
                     var customer = realm.Find<Customer>(_user.Id);
                     if (customer !=null)
                     {
                         var reward = customer.Reward.Where(r => !r.IsFinish).FirstOrDefault();
                         if (reward !=null)
                         {
                             foreach (var mov in movList)
                             {
                                 reward.MovData.Add(mov);
                             }
                             return reward;
                         }
                     }
                     return null;

                 });
                 return null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Realm = null;
                return null;
            }
        }
//------------------------------------------------   reward methods ----------------------
        public async Task<Reward> AddNewReward(Customer c)
        {
            await GetRealm();
            if (Realm is null) return null;
            try
            {
                Realm.Write(()=>
                {
                    var reward = Realm.Add(new Reward()
                    {
                        Partition = _partition,
                        Cost = (c.Policy.Price/100)
                    });
                   
                    c.Reward.Add(reward);
                    return reward;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            return null;
        }
// -------------------------------Get Instance ----------------------------
        private async Task GetRealm()
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
                if (Realm==null)
                {
                    var config = new SyncConfiguration(_partition,_user);
                    Realm =await Realm.GetInstanceAsync(config);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"realm instance error return null {e.Message}");
                Console.WriteLine($" inner exception : {e.InnerException}");
                Realm = null;
            }
           
        }


   
    }
}