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
        public List<Policy> previousPolicies;

        public PolicyManager()
        {
            previousPolicies = new List<Policy>();
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
            await RealmDb.GetInstance().UpdatePolicy(customerId,user, newPolicy);
        }

        public async Task<Tuple<bool,Policy>> FindPolicy(string customerId,User user)
        {
            return await RealmDb.GetInstance().FindPolicy(customerId,user);
        }
        

        public void Dispose()
        {
            RealmDb.GetInstance().Dispose();
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
           previousPolicies = await RealmDb.GetInstance().GetPreviousPolicies(customerId, user);
        }

        public async Task<Customer> AllowUpdate(string customerId, User user,bool allowUpdate)
        {
         return await RealmDb.GetInstance().ResolvePolicyUpdate(customerId,user,allowUpdate);
        }

        public void RemoveIfContains(Policy policy)
        {
            try
            {
                if (previousPolicies.Contains(policy)) previousPolicies.Remove(policy);
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"RemoveIfContains error : {e}");
            }
        }
    }
}