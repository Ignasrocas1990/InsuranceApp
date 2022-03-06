using System;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class RewardManager : IDisposable
    {
        private RealmDb realmDb;
        public RewardManager()
        {
            realmDb = RealmDb.GetInstancePerPage();
        }
        public async Task<Tuple<bool, float>> getTotalRewards(User user,string id)
        {
            return await realmDb.GetTotalRewards(user,id);
        }
        public async Task<Reward> FindReward(User user)
        {
         return await realmDb.FindReward(user);
        }
        public void Dispose()
        {
            realmDb.Dispose();
        }
    }
}