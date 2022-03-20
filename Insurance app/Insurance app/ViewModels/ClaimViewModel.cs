/*   Copyright 2020,Ignas Rocas

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
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    [QueryProperty(nameof(CustomerId),"CustomerId")]
    public class ClaimViewModel : ObservableObject,IDisposable
    {
        public ICommand CreateClaimCommand { get; }
        public ICommand ViewPreviousClaimsCommand { get; }
        public ICommand ResolveClaimCommand { get; }
        public ICommand AddInfoCommand{ get; }

        public readonly ClaimManager ClaimManager;

        private string dateString;
        private string hospitalPostcode="";
        private string patientNr="";
        private string status = "Not Created";
        private const string Type = "health"; //If application extended, this has to be moved to App
        private const string ViewExtraStr = "View Extra info";
        private const string AddExtraStr = "Add Extra info";
        private string customerId = "";
        private string extraInfo="";
        

        public ClaimViewModel()
        {
            CreateClaimCommand = new AsyncCommand(CreateClaim);
            ViewPreviousClaimsCommand = new AsyncCommand(GetClaims);
            ResolveClaimCommand = new AsyncCommand(ResolveClaim);
            ClaimManager = new ClaimManager();
            AddInfoCommand = new AsyncCommand(AddExtraInfo);
        }
        

        public async Task SetUp()
        {
            try
            {
                UnderReviewDisplay = false;
                SetUpWaitDisplay = true;
                if (customerId  == "")
                    customerId = App.RealmApp.CurrentUser.Id;
            
                await ClaimManager.GetClaims(App.RealmApp.CurrentUser,customerId);
                var claim = ClaimManager.GetCurrentClaim();
                if (claim != null)
                {
                    extraInfo = claim.ExtraInfo;
                    var dtoDate = claim.StartDate;
                    string displayDate = "Date Not found";
                    if (dtoDate !=null)
                    {
                        displayDate = dtoDate.Value.Date.ToString("d");

                    }
                    
                    IsReadOnly = true;
                    DateDisplay = displayDate;
                    HospitalPostCodeDisplay = claim.HospitalPostCode;
                    PatientNrDisplay = claim.PatientNr;
                    UnderReviewDisplay = true;
                    ExtraBtnText = ViewExtraStr;
                    if (customerId != App.RealmApp.CurrentUser.Id)
                    {
                        CanBeResolved = true;
                    }
                }
                else
                {
                    ExtraBtnText = AddExtraStr;
                    IsReadOnly = false;
                    DateDisplay = DateTimeOffset.Now.Date.ToString("d");
                    HospitalPostCodeDisplay = "";
                    PatientNrDisplay = "";
                    CanBeResolved = false;
                }

                PreviousBtnIsEnabled = ClaimManager.GetResolvedClaimCount()>0;
                SetUpWaitDisplay = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task GetClaims()
        {
            try
            {
                var closedClaims = ClaimManager.GetResolvedClaims() ?? new List<Claim>();
                
                await Application.Current.MainPage.Navigation
                    .ShowPopupAsync(new ExistingClaimsPopup(closedClaims));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        
        private async Task CreateClaim()
        {
            bool answer = await Shell.Current.DisplayAlert("Notice",
               "Are you sure to open new Claim?", "create", "cancel");
           if (!answer) return;

            CircularWaitDisplay = true;
            IsReadOnly = true;

            await ClaimManager.CreateClaim(hospitalPostcode, patientNr, Type,App.RealmApp.CurrentUser,customerId,extraInfo);
            
            await Shell.Current.DisplayAlert(Msg.Notice, 
                "New Claim has been Opened.\nClient will take a look at it shortly",
                "close");
            ExtraBtnText = ViewExtraStr;
            CircularWaitDisplay = false;
            UnderReviewDisplay = true;
            if (customerId != App.RealmApp.CurrentUser.Id)
            {
                CanBeResolved = true; 
            }
        }
        
        private async Task ResolveClaim()
        {
            try
            {
                var (reason, action) = await ClaimManager.GetClientAction(extraInfo);
                if (reason=="-1") return;
                
                CircularWaitDisplay = true;
                var customer = await ClaimManager.ResolveClaim(customerId,App.RealmApp.CurrentUser,reason,action);
                if (customer !=null)
                {
                    HttpService.ClaimNotifyEmail(customer.Email, customer.Name, DateTime.Now,action,reason);
                    await Shell.Current.DisplayAlert(Msg.Notice, Msg.EmailSent, "close");
                }
                
                CircularWaitDisplay = false;
                CanBeResolved = false;
                extraBtnText = AddExtraStr;
                extraInfo = "";
                await SetUp();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async Task AddExtraInfo()
        {
            string popupDisplayText = "";
            if (extraInfo.Length > 10)
                popupDisplayText = extraInfo;
            
            var tempStr = await Application.Current.MainPage.Navigation.ShowPopupAsync(
                new EditorPopup("Please enter extra claim info",underReview,popupDisplayText));
            if (tempStr != null && tempStr.Length > 10)
            {
                ExtraBtnText = ViewExtraStr;
                extraInfo = tempStr;
            }
            
        }


        //property bindings
        public string HospitalPostCodeDisplay
        {
            get => hospitalPostcode;
            set => SetProperty(ref hospitalPostcode, value);

        }
        public string PatientNrDisplay
        {
            get => patientNr;
            set => SetProperty(ref patientNr, value);

        }
        private bool wait;
        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        private bool fieldsEnabled;
        public bool IsReadOnly
        {
            get => fieldsEnabled;
            set => SetProperty(ref fieldsEnabled, value);
        }
        
        private bool previousExist;
        public bool PreviousBtnIsEnabled
        {
            get => previousExist;
            set => SetProperty(ref previousExist, value);
        }

        public string DateDisplay
        {
            get => dateString;
            set => SetProperty(ref dateString, value);
        }

        private bool setUpWait;
        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }

        private bool isClient;

        public bool CanBeResolved
        {
            get => isClient;
            set => SetProperty(ref isClient, value);
        }
        public string CustomerId
        {
            get => customerId;
            set =>  customerId = Uri.UnescapeDataString(value ?? String.Empty);

        }

        private bool underReview;
        public bool UnderReviewDisplay
        {
            get => underReview;
            set => SetProperty(ref underReview, value);
        }

        private string extraBtnText;
        public string ExtraBtnText
        {
            get => extraBtnText;
            set => SetProperty(ref extraBtnText, value);
        }


        public void Dispose()
        {
            ClaimManager.Dispose();
        }
    }
}