using System;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class PolicyManager
    {
        private readonly RealmDb realmDb;

        public PolicyManager()
        {
            realmDb = new RealmDb();
        }
        
        public PersonalPolicy CreatePolicy(string price, int cover, int fee, int hospitals, int plan, int smoker, bool status, DateTime utcNow,string partition)
        {
            
            return new PersonalPolicy()
            {
                Price = Converter.GetPrice(price), Cover = cover, HospitalFee = fee, 
                Hospitals = hospitals, Plan = plan, Smoker = smoker,
                Status = status, StartDate = utcNow,Partition = partition
            };
        }
    }
}