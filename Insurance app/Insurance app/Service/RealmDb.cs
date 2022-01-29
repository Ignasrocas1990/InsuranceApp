using System;
using System.Threading.Tasks;
using Insurance_app.Models;
using Realms;
using Realms.Sync;

namespace Insurance_app.Service
{
    public class RealmDb
    {
        private Realm Realm { set; get; }
        public User User { set; get; }
        private static RealmDb realmDb = null;
        private RealmDb() {}

        public static RealmDb GetInstance()
        {
            if (realmDb is null)
            {
                return new RealmDb();
            }

            return realmDb;
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
                await GetRealm($"Customer ={c.Id}");
                if (Realm is null)
                {
                    Console.WriteLine("Couldn't get realm");
                    return null;
                }
                Realm.Write(() =>
                {
                    Realm.Add(c);
                    
                });
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
                await GetRealm($"Customer ={User.Id}");
                if (Realm != null)
                {
                    return Realm.Find<Customer>(User.Id);
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong : \n {e}");
            }
            return null;
        }
        
// --------------------------- Mov Data  methods --------------------------------     
        public async Task<MovData> GetMovData(Customer c)
        {
            await GetRealm($"Customer ={c.Id}");
            return null;
        }
//------------------------------------------------   reward methods ----------------------
        public async Task<Reward> AddNewReward(Customer c)
        {
            await GetRealm($"partition={User.Id}");
            if (Realm is null) return null;
            try
            {
                Realm.Write(()=>
                {
                    var reward = Realm.Add(new Reward()
                    {
                        Partition = $"reward={c.Id}",
                        Amount = (c.Policy.Price/100)
                    });
                    //Customer copy = realm.Find<Customer>(c.Id);
                    //realm.Refresh();
                    //var copy2 = realm.All<Customer>().Where(x => x.Id == c.Id).FirstOrDefault();
                    //if (copy2 != null) copy2.Reward.Add(reward);
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
        private async Task GetRealm(String partition)
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
                    var config = new SyncConfiguration(partition,User);
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