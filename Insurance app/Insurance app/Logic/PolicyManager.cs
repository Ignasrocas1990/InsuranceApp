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

        public PolicyManager()
        {
            
        }
        public Policy CreatePolicy(float price,float payedPrice, int cover, int fee, int hospitals, int plan, int smoker, bool underReview, 
            DateTimeOffset expiryDate,DateTimeOffset updateDate,string owner)
        {
           
            return new Policy()
            {
                Price = price,PayedPrice = payedPrice, Cover = cover, HospitalFee = fee,
                Hospitals = hospitals, Plan = plan, Smoker = smoker,
                UnderReview = underReview,Owner = owner,ExpiryDate = expiryDate,UpdateDate = updateDate
            };
        }

        public async Task AddPolicy(User user,Policy newPolicy)
        {
            await RealmDb.GetInstance().UpdatePolicy(user, newPolicy);
        }

        public async Task<Dictionary<int,Policy>> FindPolicy(User user)
        {
            return await RealmDb.GetInstance().FindPolicy(user);
        }
        

        public void Dispose()
        {
            RealmDb.GetInstance().Dispose();
        }

        public Policy RegisterPolicy(float price,float payedPrice, int cover, int fee, int hospitals, int plan, int smoker, bool underReview, 
            DateTimeOffset expiryDate,string owner)
        {
            return new Policy()
            {
                Price = price,PayedPrice = payedPrice, Cover = cover, HospitalFee = fee,
                Hospitals = hospitals, Plan = plan, Smoker = smoker,
                UnderReview = underReview,Owner = owner,ExpiryDate = expiryDate
            };
        }
    }
}