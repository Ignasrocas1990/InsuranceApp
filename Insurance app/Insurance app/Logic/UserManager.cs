using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class UserManager
    {
        public RealmDb realmDb;
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

        public Customer CreateCustomer(string userId, int age, string fName, string lName, string phoneNr, string email)
        {
            return new Customer()
            {
                Id = userId, Age = age,
                Name = fName, LastName = lName, PhoneNr = phoneNr, Email=email, Partition = userId
            };
        }

        public async Task AddCustomer(Customer customer,User user)
        {
            await realmDb.AddCustomer(customer,user);
        }
        
        public void StopSync()
        {
            realmDb.StopSync();
        }
    }
}