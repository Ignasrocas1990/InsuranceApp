using System;
using System.Linq;
using Insurance_app.Models;
using Realms;
using Realms.Exceptions;
using Realms.Sync;
using Realms.Sync.Exceptions;

namespace Insurance_app
{
    public class MongoSetup
    {
        public User user;
        public Customer customer;
        private Realm realm;
        public SyncConfiguration Config { get; set; }
        public MongoSetup()
        {

            Setup();
        }

        public void CreateCustomer()
        {
            try
            {
                customer =  new Customer()
                {
                    Username = "test.com",
                    Password = "test1234",
                    Name = "test",
                    Age = 12,

                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }


        public async void Setup()
        {

            try
            {
                string MyRealmAppId = "application-0-bvutx";

                var app = Realms.Sync.App.Create(MyRealmAppId);
                user = await app.LogInAsync(
                    Credentials.EmailPassword("test@gmail.com", "test@gmail.com"));


                Config = new SyncConfiguration("_partition", user, "x2.realm");
                //Config.IsDynamic=true;
                

                realm = await Realm.GetInstanceAsync(Config);
                Console.WriteLine("all ok");
                /*
                var existingCustomer = realm.All<Customer>()
                    .FirstOrDefault(c => c.Username.Equals("test.com") && c.Password.Equals("test@gmail.com"));
                if (existingCustomer is null)
                {
                    CreateCustomer(realm);
                }
                */

                Console.WriteLine($"user longed in? {customer != null}");
                Console.WriteLine($"connected to realm ?{realm != null}");
            }
            catch (RealmFileAccessErrorException ex)
            {
                Console.WriteLine($@"Error creating or opening the realm file. {ex.Message}");
            }
            catch (RealmException e)
            {
                Console.WriteLine($"Other exception {e}");
                Console.WriteLine(e.InnerException);
            }
            

        }

        public void CreateCustomer(Realm realm)
        {
            realm.Write(() =>
            {
                realm.Add(customer);
            });
        }
    }
}