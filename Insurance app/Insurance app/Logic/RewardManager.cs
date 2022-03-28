/*
    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Realms.Sync;

namespace Insurance_app.Logic
{
    /// <summary>
    /// Class used to connect between Database and UI,
    /// while processing Rewards
    /// </summary>
    public class RewardManager : IDisposable
    {
        private readonly RealmDb realmDb;
        public RewardManager()
        {
            realmDb = RealmDb.GetInstancePerPage();
        }
        /// <summary>
        /// Uses RealmDb helper to get completed rewards,sum and is
        /// data being collected switch
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="userId">current user Id</param>
        /// <returns>Is data collected & reward completed sum</returns>
        public async Task<(DataSendSwitch toggle, float totalSum)> GetTotalRewards(User user,string userId)
        {
            var (toggle,rewards)= await realmDb.GetTotalRewards(user,userId);
            return rewards.Count == 0 ? (toggle, 0.0f) : (toggle,GetRewardSum(rewards));
        }
        /// <summary>
        /// Finds sum of completed rewards
        /// </summary>
        /// <param name="rewards">List of rewards</param>
        /// <returns>sum completed rewards</returns>
        public float GetRewardSum(List<Reward>rewards)
        {
            if (rewards.Count == 0) return 0;
            return rewards.Where(r => r.FinDate != null && r.DelFlag == false)
                .Sum(reward => (float) reward.Cost);
        }
        /// <summary>
        /// Uses RealmDb helper to get current rewards
        /// </summary>
        /// <param name="user">Current user</param>
        /// <returns>Current Reward Instance</returns>
        public async Task<Reward> FindReward(User user)
        {
         return await realmDb.FindReward(user);
        }
        /// <summary>
        /// release Realm instance
        /// </summary>
        public void Dispose()
        {
            RealmDb.Dispose();
        }
        /// <summary>
        /// Compares total rewards vs price
        /// </summary>
        /// <param name="totalRewards">double Total earned rewards</param>
        /// <param name="price">double current price</param>
        /// <returns>doubles price to be displayed & reward left overs</returns>

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

        /// <summary>
        /// Uses RealmDb helper to update used rewards (when earned rewards > policy price)
        /// </summary>
        /// <param name="price">float price to be updated</param>
        /// <param name="user">current Realm user</param>
        /// <param name="customerId">User id</param>
        public void UpdateRewardsWithOverdraft(float price, User user, string customerId)
        {
            Task.FromResult(realmDb.UpdateRewardsWithOverdraft(price, user, customerId));
        }

        /// <summary>
        /// Uses RealmDb helper to update all earned rewards
        /// </summary>
        /// <param name="user">current Realm user</param>
        /// <param name="customerId">User id</param>
        public void UserRewards(User user, string customerId)
        {
            Task.FromResult(realmDb.UseRewards(user, customerId));
        }
    }
}