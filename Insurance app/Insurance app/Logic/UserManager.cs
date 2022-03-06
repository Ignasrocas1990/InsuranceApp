using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms;
using Realms.Sync;
using Xamarin.Forms;

namespace Insurance_app.Logic
{
    public class UserManager : IDisposable
    {
        private RealmDb realmDb;

        public UserManager()
        {
            realmDb = RealmDb.GetInstancePerPage();

        }
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
        public async Task<Customer> GetCustomer(User user,string id)
        {
            return await realmDb.FindCustomer(user,id);
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
            await realmDb.AddCustomer(customer,user);
        }


        public async Task updateCustomer(string name, string lastName, 
            string phoneNr,Address address, User user,string customerId)
        {
           await realmDb.UpdateCustomer(name, lastName,
                phoneNr,  address, user,customerId);
        }

        public async Task<bool> CreateClient(User user, string email, string fname, string lname, string code)
        {
            return await realmDb.CreateClient(user, email, fname, lname, code);
        }

        public async Task<string> FindTypeUser(User user)
        {
            try
            {
                var now = DateTimeOffset.Now;
                var customer = await realmDb.FindCustomer(user,user.Id);

                if (customer!=null)
                {
                    // expired
                    var currentPolicy= FindLatestPolicy(customer);
                
                    if (currentPolicy != null && currentPolicy.ExpiryDate < now)
                    {
                        if (!customer.DirectDebitSwitch)
                        {
                            return currentPolicy.Id.ToString();
                        }
                        await UpdatePolicy(now,customer,user,currentPolicy);
                    }
                    return "Customer";
                }
                if (await realmDb.IsClient(user))
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

        private Policy FindLatestPolicy(Customer customer)
        {
            try
            {
                return customer.Policy
                    ?.Where(p=> p.DelFlag == false).OrderByDescending(z => z.ExpiryDate).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
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
                await realmDb.UpdatePolicyDate(newDate, currentPolicy,totalCost,user);
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
            
            List<Customer> list = new List<Customer>();
            var now = DateTimeOffset.Now;
           foreach (var customer in await realmDb.GetAllCustomer(user))
           {
              var policy= FindLatestPolicy(customer);
              if (policy != null && policy.ExpiryDate > now)
              {
                  list.Add(customer);
              }
           }
           return list;
        }

        public async Task<DateTimeOffset> GetCustomersDob(string customerId,User user)
        {
            return await realmDb.GetCustomersDob(customerId,user);
        }

        public async Task UpdateCustomerSwitch(User user, bool switchState)
        { 
         await  realmDb.UpdateCustomerSwitch(user, switchState);
        }

        public void UpdateAccountSettings(User user,string userId,bool directDebit, bool useRewards)
        {
            Task.FromResult(realmDb.UpdateAccountSettings(user,userId,directDebit,useRewards));
        }
        public async Task ResetPassword(string email,string name,HttpService api)
        {
            try
            {
             
                var tempPass = StaticOpt.TempPassGenerator();
                await App.RealmApp.EmailPasswordAuth.CallResetPasswordFunctionAsync(email,tempPass);
                api.ResetPasswordEmail(email,name, DateTime.Now, tempPass);
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void Dispose()
        {
            realmDb.Dispose();
        }
    }
}