using System;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class UserManager : IDisposable
    {
        private RealmDb realmDb;
        public UserManager()
        {
            realmDb= new RealmDb();
            
        }

        public Task<string> Register(string email, string password)
        {
            return realmDb.Register(email, password);
        }
        public void DelCustomer()
        {
            
        }
        public Task<Customer> GetCustomer(User user)
        {
            return realmDb.FindCustomer(user);
        }

        public Customer EditCustomer()
        {
            return new Customer();
        }

        public Customer CreateCustomer(int age, string fName, string lName, string phoneNr, string email,Address address)
        {
            try
            {
                return new Customer()
                {
                
                    Age = age,
                    Name = fName, LastName = lName, PhoneNr = phoneNr, Email=email, 
                    Address = address

                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"customer creation error :\n {e}");
                return null;
            }

        }

        public async Task AddCustomer(Customer customer,User user)
        {
            await realmDb.AddCustomer(customer,user);
        }
        public void Dispose()
        {
            if (realmDb!=null)
            {
                realmDb.Dispose();
                
            }
        }
    }
}