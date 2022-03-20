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
        /// 
        /// </summary>
        /// <returns></returns>
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
        public async Task CreateClaim(string hospitalPostcode, string patientNr, string type, User user,string customerId,string extraInfo)
        {
            await realmDb.AddClaim(hospitalPostcode, patientNr, type, user,customerId,extraInfo);
        }

        public async Task<List<Claim>> GetClaims(User user,string customerId)
        {
            Claims = await realmDb.GetClaims(user,customerId);
            return Claims;
        }

        public Claim GetCurrentClaim()
        {
            try
            {
                var aClaim = Claims.FirstOrDefault(claim => claim.CloseDate == null);
                if (aClaim is null) return null;
                return aClaim;
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetCurrentClaim error : {e}");
            }
            return null;
        }

        public async Task<Customer> ResolveClaim(string customerId, User user,string reason,bool action)
        {
           return await realmDb.ResolveClaim(customerId,user,reason,action);
        }

        public void Dispose()
        {
            realmDb.Dispose();
            if (Claims != null) Claims = null;
        }

        public async Task<IEnumerable<Claim>> GetAllOpenClaims(User user)
        {
           return await realmDb.GetAllOpenClaims(user);
        }

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
                        await Application.Current.MainPage.DisplayAlert
                            (Msg.Notice, "Claim cant be Denied without a reason", "close");
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