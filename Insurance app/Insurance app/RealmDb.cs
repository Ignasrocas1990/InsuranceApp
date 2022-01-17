using System;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Realms;
using Realms.Exceptions;
using Realms.Sync;

namespace Insurance_app
{
    public class RealmDb
    {

        //TODO Register method

        public RealmDb()
        {
            
        }

        public async Task<MovData> GetMovData(Customer c)
        {
            var realm = await GetRealm($"Customer ={c.Id}");
            return null;
        }

        public async Task AddCustomer(string email,string password,string customerId)
        {
            Console.WriteLine("adding new customer ...");
            var realm = await GetRealm($"Customer ={customerId}");
            realm.Write(() =>
            {
                try
                {
                    realm.Add(new Customer()
                    {
                        Email = email,Password = password,Id = customerId
                    });
                }
                catch (Exception e)
                {
                    Console.WriteLine($"customer not added : {e}");
                }
               
            });
            
        }
        public async Task<Customer> FindCustomer(string customerId)
        {
            try
            {
                var realm = await GetRealm($"Customer ={customerId}");
                return realm.Find<Customer>(customerId);
               
            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong : {e}");
                return null;

            }
        }
        
        
        private async Task<Realm> GetRealm(String partition)
        {
            try
            {
                var localConfig = new RealmConfiguration("RealmDBFile")
                {
                    SchemaVersion = 5
                    //ShouldDeleteIfMigrationNeeded = true
                };
                var config = new SyncConfiguration(partition,App.RealmApp.CurrentUser);

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