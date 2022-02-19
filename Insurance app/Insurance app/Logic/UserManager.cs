using System;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class UserManager : IDisposable
    {
        public UserManager()
        {
            
        }
        public Task<string> Register(string email, string password)
        {
            
            return RealmDb.GetInstance().Register(email, password);
        }
        public void DelCustomer()
        {
            
        }
        public async Task<Customer> GetCustomer(User user)
        {
            return await RealmDb.GetInstance().FindCustomer(user);
           // return currentCustomer;
        }

        public Customer EditCustomer()
        {
            return new Customer();
        }

        public Customer CreateCustomer(DateTimeOffset dob, string fName, string lName, string phoneNr, string email,Address address)
        {
            try
            {
                return new Customer()
                {
                
                    Dob = dob,
                    Name = fName, LastName = lName, PhoneNr = phoneNr, Email=email,
                    Address = new Address()
                    {
                        HouseN = address.HouseN,
                        City = address.City,
                        Country = address.Country,
                        County = address.County,
                        PostCode = address.PostCode,
                        Street = address.Street
                    }
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
            await RealmDb.GetInstance().AddCustomer(customer,user);
        }
        public void Dispose()
        {
            if (RealmDb.GetInstance()!=null)
            {
                RealmDb.GetInstance().Dispose();
                
            }
        }

        public async Task updateCustomer(string name, string lastName, 
            string phoneNr,Address address, User user)
        {
           await RealmDb.GetInstance().UpdateCustomer(name, lastName,
                phoneNr,  address, user);
        }

        public async Task<bool> CreateClient(User user, string email, string fname, string lname, string code)
        {
            return await RealmDb.GetInstance().CreateClient(user, email, fname, lname, code);
        }
    }
}