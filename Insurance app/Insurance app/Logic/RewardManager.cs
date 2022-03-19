using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<(bool toggle, float totalSum)> GetTotalRewards(User user,string id)
        {
            var (toggle,rewards)= await realmDb.GetTotalRewards(user,id);
            return rewards.Count == 0 ? (toggle, 0.0f) : (toggle,GetRewardSum(rewards));
        }

        public float GetRewardSum(List<Reward>rewards)
        {
            if (rewards.Count == 0) return 0;
            return rewards.Where(reward => reward.Cost != null).Sum(reward => (float) reward.Cost);
        }
        public async Task<Reward> FindReward(User user)
        {
         return await realmDb.FindReward(user);
        }
        public void Dispose()
        {
            realmDb.Dispose();
        }

        public (double, double) ChangePrice(double totalRewards, double price)
        {
            double priceDisplay=0;
            double rewardsLeftover=0;
            if (totalRewards > price)
            {
                priceDisplay = 1.0;
                rewardsLeftover = totalRewards -(price - 1);
            }
            else if (totalRewards == price)
            {
                priceDisplay = 1.0;
                rewardsLeftover = 1.0;
            }
            else
            {
                priceDisplay = price - totalRewards;
                rewardsLeftover = 0;
            }

            return new ValueTuple<double, double>(priceDisplay, rewardsLeftover);
        }

        public void UpdateRewardsWithOverdraft(float price, User user, string customerId)
        {
            Task.FromResult(realmDb.UpdateRewardsWithOverdraft(price, user, customerId));
        }

        public void UserRewards(User user, string customerId)
        {
            Task.FromResult(realmDb.UseRewards(user, customerId));
        }
    }
}