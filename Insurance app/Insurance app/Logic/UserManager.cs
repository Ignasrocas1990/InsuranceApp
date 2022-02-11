using System;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class UserManager : IDisposable
    {
        //private Customer currentCustomer;
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
            await RealmDb.GetInstance().AddCustomer(customer,user);
        }
        public void Dispose()
        {
            if (RealmDb.GetInstance()!=null)
            {
                RealmDb.GetInstance().Dispose();
                
            }
        }

        public async Task updateCustomer(int age, string name, string lastName, 
            string phoneNr, string email, Address address, User user)
        {
           await RealmDb.GetInstance().UpdateCustomer(age, name, lastName,
                phoneNr, email, address, user);
        }
    }
}