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
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms.Sync;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace Insurance_app.Logic
{
    public class ClaimManager : IDisposable
    {
        /// <summary>
        /// Class used to connect between Database
        /// and UI, while processing some Claims
        /// </summary>
        private List<Claim> Claims { get; set; }
        private readonly RealmDb realmDb;

        public ClaimManager()
        {
            realmDb=RealmDb.GetInstancePerPage();
            Claims = new List<Claim>();
        }
        
        /// <summary>
        /// Gets a count of number Claims that are resolved
        /// </summary>
        /// <returns>number of resolved claims</returns>
        public int GetResolvedClaimCount()
        {
            try
            {
                return Claims.Count(c => c.CloseDate != null && c.DelFlag == false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return 0;
        }
        
        /// <summary>
        /// finds claims that are resolved
        /// </summary>
        /// <returns>null/resolved claims</returns>
        public List<Claim> GetResolvedClaims()
        {
            try
            {
                return Claims.Where(c => c.CloseDate != null && c.DelFlag == false).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
        /// <summary>
        /// Sends information to realm Database
        /// </summary>
        /// <param name="hospitalPostcode">User input</param>
        /// <param name="patientNr">User input</param>
        /// <param name="type">health (health insurance)</param>
        /// <param name="user">Current user</param>
        /// <param name="customerId"></param>
        /// <param name="extraInfo">User input</param>
        public async Task CreateClaim(string hospitalPostcode, string patientNr, string type, User user,string customerId,string extraInfo)
        {
            await realmDb.AddClaim(hospitalPostcode, patientNr, type, user,customerId,extraInfo);
        }
        /// <summary>
        /// Retrieves claims from RealmDb helper
        /// </summary>
        /// <param name="user">Current user</param>
        /// <param name="customerId">User/Customer Id</param>
        public async Task GetClaims(User user, string customerId)
        {
            Claims = await realmDb.GetClaims(user,customerId);
        }
        
        /// <summary>
        /// Find current claim.
        /// </summary>
        /// <returns>Current claim/null</returns>
        public Claim GetCurrentClaim()
        {
            try
            {
                var aClaim = Claims.FirstOrDefault(claim => claim.CloseDate == null);
                return aClaim ?? null;
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetCurrentClaim error : {e}");
            }
            return null;
        }
        
        /// <summary>
        /// Passes Claim to realmDb helper so it can be resolved
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Client</param>
        /// <param name="reason">Customer Comment / Client deny input</param>
        /// <param name="action">accept/deny the claim</param>
        /// <returns></returns>
        public async Task<Customer> ResolveClaim(string customerId, User user,string reason,bool action)
        {
           return await realmDb.ResolveClaim(customerId,user,reason,action);
        }
        /// <summary>
        /// release allocated memory & Realm instance
        /// </summary>
        public void Dispose()
        {
            RealmDb.Dispose();
            if (Claims != null) Claims = null;
        }
        /// <summary>
        /// Finds open claims 
        /// </summary>
        /// <param name="user">Client</param>
        /// <returns>Open Claims</returns>
        public async Task<IEnumerable<Claim>> GetAllOpenClaims(User user)
        {
           return await realmDb.GetAllOpenClaims(user);
        }
        /// <summary>
        /// Find if client want to resolve claim and get reason if not
        /// </summary>
        /// <param name="extraInfo">Customer comment</param>
        /// <returns>Customer comment or Client decline reason and accept/deny claim</returns>
        public async Task<Tuple<string, bool>> GetClientAction(string extraInfo)
        {
            var action = await Application.Current.MainPage.DisplayAlert(Msg.Notice, 
                "Do you want to Resolve claim by Accepting or Denying it?", "Accept", "Deny");

            var answerString  = action ? "Accept" : "Deny";
                
            var result = await  Application.Current.MainPage.DisplayAlert(Msg.Notice, 
                $"Are you sure you want to {answerString} the Claim?", "Yes", "No");
            var reason="-1";
            
            if (!result) return new Tuple<string, bool>(reason, action);
            switch (action)
            {
                case false:
                {
                    reason =await Application.Current.MainPage.Navigation.ShowPopupAsync(
                        new EditorPopup("Please enter reason for Denying Claim",false,""));
                    if (reason is "")
                    {
                        await Msg.Alert("Claim cant be Denied without a reason");
                        reason="-1";
                    }

                    break;
                }
                case true:
                    reason = extraInfo;
                    break;
            }

            return new Tuple<string, bool>(reason, action);
        }
    }
}