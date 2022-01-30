using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class UserManager
    {
        private RealmDb realmDb;
        public UserManager()
        {
            realmDb=RealmDb.GetInstance();
        }
        public void SetUser(User user)
        {
            realmDb.SetUser(user);
        }

        public Task<string> Register(string email, string password)
        {
            return realmDb.Register(email, password);
        }
        public void DelCustomer()
        {
            
        }
        public Task<Customer> GetCustomer()
        {
            return realmDb.FindCustomer();
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

        public Task<Customer> AddCustomer(Customer customer)
        {
            return realmDb.AddCustomer(customer);
        }
    }
}