using System;
using System.Diagnostics;
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

        public RealmDb(){}
//------------------------- app Access Methods ---------------------------------------
        public async Task<string> Register(String Email, String Password)
        {
            try
            {
              await App.RealmApp.EmailPasswordAuth.RegisterUserAsync(Email, Password);
              return "success";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
        }
        public Task<User> Login(String Email, String Password)
        {
            try
            {
                return App.RealmApp.LogInAsync(
                    Credentials.EmailPassword(Email, Password));
            }
            catch (Exception e)
            {
                Console.WriteLine("could not get");
                return null;
            }
        }
//------------------------------------- Customer methods ---------------------
        
        public async Task AddCustomer(Customer c)
        {
            try
            {
                var realm = await GetRealm($"Customer ={c.Id}");
                if (realm is null)
                {
                    Console.WriteLine("Couldn't get realm");
                    return;
                }
                realm.Write(() =>
                {
                    realm.Add(c);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem adding customer : \n {e}");
            }
           
        }
        
        public async Task<Customer> FindCustomer(string customerId)
        {
            try
            {
                var realm = await GetRealm($"Customer ={customerId}");
                if (realm != null) 
                    return realm.Find<Customer>(customerId);
               
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
            var realm = await GetRealm($"Customer ={c.Id}");
            return null;
        }


// -------------------------------Get Instance ----------------------------
        private async Task<Realm> GetRealm(String partition)
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