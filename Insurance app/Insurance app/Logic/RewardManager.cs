using System;
using System.Threading.Tasks;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class RewardManager : IDisposable
    {
        private RealmDb db;
        public RewardManager()
        {
            db = new RealmDb();
        }

        public async Task CreateReward(User user)
        {
           await db.AddNewReward(user);
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}