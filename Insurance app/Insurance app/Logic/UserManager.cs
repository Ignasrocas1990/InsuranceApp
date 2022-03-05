using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms;
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
        public async Task<Customer> GetCustomer(User user,string id)
        {
            return await RealmDb.GetInstance().FindCustomer(user,id);
           // return currentCustomer;
        }

        public Customer EditCustomer()
        {
            return new Customer();
        }

        public Customer CreateCustomer(DateTimeOffset dob, string fName, string lName, string phoneNr, string email,Address address,bool direct)
        {
            try
            {
                return new Customer()
                {
                
                    Dob = dob,DirectDebitSwitch = direct,
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
            string phoneNr,Address address, User user,string customerId)
        {
           await RealmDb.GetInstance().UpdateCustomer(name, lastName,
                phoneNr,  address, user,customerId);
        }

        public async Task<bool> CreateClient(User user, string email, string fname, string lname, string code)
        {
            return await RealmDb.GetInstance().CreateClient(user, email, fname, lname, code);
        }

        public async Task<string> FindTypeUser(User user)
        {
            try
            {
                var now = DateTimeOffset.Now;
                var customer = await RealmDb.GetInstance().FindCustomer(user,user.Id);

                if (customer!=null)
                {
                    // expired
                    var currentPolicy = customer.Policy
                        ?.Where(p=> p.DelFlag == false && p.ExpiryDate< now).OrderByDescending(z => z.ExpiryDate).First();
                
                    if (currentPolicy!=null)
                    {
                        if (!customer.DirectDebitSwitch)
                        {
                            return "ExpiredCustomer";
                        }
                        await UpdatePolicy(now,customer,user,currentPolicy);
                    }
                    return "Customer";
                }
                if (await RealmDb.GetInstance().IsClient(user.Id, user))
                {
                    return "Client";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "";
        }
        private async Task UpdatePolicy(DateTimeOffset now, Customer customer, User user, Policy currentPolicy)
        {
            try
            {
                float totalCost = 0;
                var newDate = ChangeDate(currentPolicy.ExpiryDate.Value, now);
                if (customer.AutoRewardUse)
                {
                    var rewards = customer.Reward.Where(r => r.FinDate != null && r.DelFlag != false).ToList();
                    totalCost = GetTotalRewardCost(rewards);
                }
                await RealmDb.GetInstance().UpdatePolicyDate(newDate, currentPolicy,totalCost,user);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        

        private float GetTotalRewardCost(IEnumerable<Reward> rewards)
        {
            return rewards.Where(reward => reward.Cost != null).Sum(reward => (float) reward.Cost);
        }
        private DateTimeOffset ChangeDate(DateTimeOffset policyDate,DateTimeOffset now)
        {
            while (policyDate<now)
            {
                policyDate = policyDate.AddMonths(1);
            }

            return policyDate;
        }

        public async Task<List<Customer>>GetAllCustomer(User user)
        {
           return await RealmDb.GetInstance().GetAllCustomer(user);
        }

        public async Task<DateTimeOffset> GetCustomersDob(string customerId,User user)
        {
            return await RealmDb.GetInstance().GetCustomersDob(customerId,user);
        }

        public async Task UpdateCustomerSwitch(User user, bool switchState)
        { 
         await  RealmDb.GetInstance().UpdateCustomerSwitch(user, switchState);
        }

        public void UpdateAccountSettings(User user,string userId,bool directDebit, bool useRewards)
        {
            Task.FromResult(RealmDb.GetInstance().UpdateAccountSettings(user,userId,directDebit,useRewards));
        }
    }
}