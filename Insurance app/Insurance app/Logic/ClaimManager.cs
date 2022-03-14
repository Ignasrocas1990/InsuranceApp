using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms;
using Realms.Sync;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;

namespace Insurance_app.Logic
{
    public class ClaimManager : IDisposable
    {
        public List<Claim> Claims { get; set; }
        private RealmDb realmDb;

        public ClaimManager()
        {
            realmDb=RealmDb.GetInstancePerPage();
        }
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