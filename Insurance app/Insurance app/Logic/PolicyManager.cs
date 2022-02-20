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
        public PersonalPolicy CreatePolicy(string price, int cover, int fee, int hospitals, int plan, int smoker, bool underReview, 
            DateTimeOffset date,DateTimeOffset? dt,string owner)
        {
            
            return new PersonalPolicy()
            {
                Price = Converter.GetPrice(price), Cover = cover, HospitalFee = fee, UpdateDate = dt,
                Hospitals = hospitals, Plan = plan, Smoker = smoker,
                UnderReview = underReview, StartDate = date,Owner = owner
            };
        }

        public async Task AddPolicy(User user,PersonalPolicy newPolicy)
        {
            await RealmDb.GetInstance().UpdatePolicy(user, newPolicy);
        }

        public async Task<Dictionary<int,PersonalPolicy>> FindPolicy(User user)
        {
            return await RealmDb.GetInstance().FindPolicy(user);
        }
        

        public void Dispose()
        {
            RealmDb.GetInstance().Dispose();
        }
    }
}