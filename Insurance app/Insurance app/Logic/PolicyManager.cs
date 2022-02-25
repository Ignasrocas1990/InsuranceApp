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
        public List<Policy> previousPolicies { get; set; }

        public PolicyManager()
        {
            
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

        public async Task<Dictionary<int,Policy>> FindPolicy(string customerId,User user)
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

        public async Task<bool> GetPreviousPolicies(string customerId, User user)
        {
           previousPolicies = await RealmDb.GetInstance().GetPreviousPolicies(customerId, user);
           return previousPolicies.Count>0;
        }

        public async Task AllowUpdate(string customerId, User user,bool allowUpdate)
        {
           await RealmDb.GetInstance().ResolvePolicyUpdate(customerId,user,allowUpdate);
        }
    }
}