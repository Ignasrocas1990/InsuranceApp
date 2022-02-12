using System;
using System.Threading.Tasks;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class RewardManager : IDisposable
    {
        public RewardManager()
        {
        }

        public async Task CreateReward(User user)
        {
           await RealmDb.GetInstance().AddNewReward(user);
        }

        public void Dispose()
        {
            RealmDb.GetInstance().Dispose();
        }

        public async Task<float> getTotalRewards(User user)
        {
            return await RealmDb.GetInstance().GetTotalRewards(user);
        }
    }
}