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
    /// Class used to connect between Database
    /// and UI, while processing Policies
    /// </summary>
    public class PolicyManager : IDisposable
    {
        public List<Policy> PreviousPolicies;
        private readonly RealmDb realmDb;

        public PolicyManager()
        {
            PreviousPolicies = new List<Policy>();
            realmDb = RealmDb.GetInstancePerPage();
        }
        /// <summary>
        /// Creates a Policy instance (Update policy)
        /// </summary>
        /// <param name="price">predicted price by ML api</param>
        /// <param name="payedPrice">0</param>
        /// <param name="cover">Selected one of these options Low/Medium/High </param>
        /// <param name="fee">Selected price they pay in-case of hospital admission</param>
        /// <param name="hospitals">User selected type of hospital for cover</param>
        /// <param name="plan">Selected one of </param>
        /// <param name="smoker">user input</param>
        /// <param name="underReview">is policy updated</param>
        /// <param name="expiryDate">expiration date => time to pay</param>
        /// <param name="updateDate">date been updated</param>
        /// <param name="owner">customer Id</param>
        /// <returns>Policy Instance</returns>
        public Policy CreatePolicy(float price,float payedPrice, string cover, int fee, string hospitals, string plan, int smoker, bool underReview, 
            DateTimeOffset expiryDate,DateTimeOffset updateDate,string owner)
        {
           
            return new Policy()
            {
                Price = price,PayedPrice = payedPrice, Cover = cover, HospitalFee = fee,
                Hospitals = hospitals, Plan = plan, Smoker = smoker,
                UnderReview = underReview,Owner = owner,ExpiryDate = expiryDate,UpdateDate = updateDate
            };
        }
        /// <summary>
        /// Passes policy to realmDb helper so it can be saved.
        /// </summary>
        /// <param name="customerId"/>
        /// <param name="user">customer/client id</param>
        /// <param name="newPolicy">policy instance</param>
        public async Task AddPolicy(string customerId,User user,Policy newPolicy)
        {
            await realmDb.UpdatePolicy(customerId,user, newPolicy);
        }
        
        /// <summary>
        /// User realmDb helper to find policy and if can be updated
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">customer/client</param>
        /// <returns>Can be updated? & policy instance</returns>
        public async Task<Tuple<bool,Policy>> FindPolicy(string customerId,User user)
        {
            return await realmDb.FindPolicy(customerId,user);
        }
        /// <summary>
        /// Create new Instance of policy
        /// </summary>
        /// <param name="price">predicted price by ML api</param>
        /// <param name="payedPrice">0</param>
        /// <param name="cover">Selected one of these options Low/Medium/High </param>
        /// <param name="fee">Selected price they pay in-case of hospital admission</param>
        /// <param name="hospitals">User selected type of hospital for cover</param>
        /// <param name="plan">Selected one of </param>
        /// <param name="smoker">user input</param>
        /// <param name="underReview">is policy updated</param>
        /// <param name="expiryDate">expiration date => time to pay</param>
        /// <param name="owner">customer Id</param>
        /// <returns>Policy instance</returns>
        public Policy RegisterPolicy(float price,float payedPrice, string cover, int fee, string hospitals, string plan, int smoker, bool underReview, 
            DateTimeOffset expiryDate,string owner)
        {
            return new Policy()
            {
                Price = price,PayedPrice = payedPrice, Cover = cover, HospitalFee = fee,
                Hospitals = hospitals, Plan = plan, Smoker = smoker,
                UnderReview = underReview,Owner = owner,ExpiryDate = expiryDate
            };
        }
        /// <summary>
        /// Uses realmDb helper to get previous policies
        /// </summary>
        /// <param name="customerId"/>
        /// <param name="user">current user</param>
        public async Task GetPreviousPolicies(string customerId, User user)
        {
           PreviousPolicies.Clear();
           PreviousPolicies = await realmDb.GetPreviousPolicies(customerId, user);
        }
        /// <summary>
        /// Uses realmDb helper to update policy
        /// </summary>
        /// <param name="customerId"/>
        /// <param name="user">current user</param>
        /// <param name="allowUpdate"> allow or deny the update</param>
        /// <returns>Customer instance</returns>
        public async Task<Customer> AllowUpdate(string customerId, User user,bool allowUpdate)
        {
         return await realmDb.ResolvePolicyUpdate(customerId,user,allowUpdate);
        }
        /// <summary>
        /// Remove policy from previous policies
        /// </summary>
        /// <param name="policy">current policy</param>
        public void RemoveIfContains(Policy policy)
        {
            try
            {
                if (PreviousPolicies.Contains(policy)) PreviousPolicies.Remove(policy);
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"RemoveIfContains error : {e}");
            }
        }
        /// <summary>
        /// releases allocated memory & realm instance 
        /// </summary>
        public void Dispose()
        {
            RealmDb.Dispose();
            PreviousPolicies = null;
        }
        /// <summary>
        /// Uses realm Db helper to find updated policies
        /// </summary>
        /// <param name="user">current user</param>
        /// <returns>list of updated policies</returns>
        public Task<IEnumerable<Policy>> GetAllUpdatedPolicies(User user)
        {
          return realmDb.GetAllUpdatedPolicies(user);
        }
        /// <summary>
        /// Uses realm Db helper to update policy price
        /// </summary>
        /// <param name="policy">current policy</param>
        /// <param name="user">customer</param>
        /// <param name="price">payed price</param>
        public async Task UpdatePolicyPrice(Policy policy,User user, float price)
        {
           await realmDb.UpdatePolicyPrice(policy,user, price);
        }
        /// <summary>
        /// Finds policy that is unpaved
        /// </summary>
        /// <param name="customer">Current customer</param>
        /// <returns>Un-payed policy instance</returns>
        public Policy FindUnpayedPolicy(Customer customer)
        {
            return customer.Policy.FirstOrDefault(p => p.PayedPrice == 0 && p.DelFlag == false);
        }
    }
}