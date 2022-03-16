using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class PolicyManager : IDisposable
    {
        public List<Policy> PreviousPolicies;
        private RealmDb realmDb;

        public PolicyManager()
        {
            PreviousPolicies = new List<Policy>();
            realmDb = RealmDb.GetInstancePerPage();
        }
        public Policy CreatePolicy(float price,float payedPrice, string cover, int fee, string hospitals, string plan, int smoker, bool underReview, 
            DateTimeOffset expiryDate,DateTimeOffset updateDate,string owner)
        {
           
            return new Policy()
            {
                Price = price,PayedPrice = payedPrice, Cover = cover, HospitalFee = fee,
                Hospitals = hospitals, Plan = plan, Smoker = smoker,
                UnderReview = underReview,Owner = owner,ExpiryDate = expiryDate,UpdateDate = updateDate
            };
        }

        public async Task AddPolicy(string customerId,User user,Policy newPolicy)
        {
            await realmDb.UpdatePolicy(customerId,user, newPolicy);
        }

        public async Task<Tuple<bool,Policy>> FindPolicy(string customerId,User user)
        {
            return await realmDb.FindPolicy(customerId,user);
        }

        public Policy RegisterPolicy(float price,float payedPrice, string cover, int fee, string hospitals, string plan, int smoker, bool underReview, 
            DateTimeOffset expiryDate,string owner)
        {
            return new Policy()
            {
                Price = price,PayedPrice = payedPrice, Cover = cover, HospitalFee = fee,
                Hospitals = hospitals, Plan = plan, Smoker = smoker,
                UnderReview = underReview,Owner = owner,ExpiryDate = expiryDate
            };
        }

        public async Task GetPreviousPolicies(string customerId, User user)
        {
           PreviousPolicies = await realmDb.GetPreviousPolicies(customerId, user);
        }

        public async Task<Customer> AllowUpdate(string customerId, User user,bool allowUpdate)
        {
         return await realmDb.ResolvePolicyUpdate(customerId,user,allowUpdate);
        }

        public void RemoveIfContains(Policy policy)
        {
            try
            {
                if (PreviousPolicies.Contains(policy)) PreviousPolicies.Remove(policy);
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"RemoveIfContains error : {e}");
            }
        }
        public void Dispose()
        {
            realmDb.Dispose();
            PreviousPolicies = null;
        }

        public Task<IEnumerable<Policy>> GetAllUpdatedPolicies(User user)
        {
          return realmDb.GetAllUpdatedPolicies(user);
        }

        public Task<Customer> UpdatePolicyPrice(User user, string customerId, double price)
        {
           return realmDb.UpdatePolicyPrice(user, customerId, price);
        }
    }
}