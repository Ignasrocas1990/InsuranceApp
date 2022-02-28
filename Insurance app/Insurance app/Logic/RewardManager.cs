using System;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class RewardManager : IDisposable
    {
        public RewardManager()
        {
        }

        public void Dispose()
        {
            RealmDb.GetInstance().Dispose();
        }

        public async Task<Tuple<bool, float>> getTotalRewards(User user,string id)
        {
            return await RealmDb.GetInstance().GetTotalRewards(user,id);
        }

        public Task addNewMovDate(float x, float y, float z,User user)
        {
            return Task.FromResult(RealmDb.GetInstance().AddMovData(x, y, z, user));
        }

        public async Task<Reward> FindReward(User user)
        {
         return await RealmDb.GetInstance().FindReward(user);
        }
    }
}