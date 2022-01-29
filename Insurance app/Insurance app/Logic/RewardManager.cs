using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class RewardManager
    {
        private readonly RealmDb realmDb;

        public RewardManager()
        {
            realmDb=RealmDb.GetInstance();
        }

        public async Task<Reward> AddReward(Customer c)
        {
            return await realmDb.AddNewReward(c);
        }

        public void GetReward( Customer c)
        {
            
        }
    }
}