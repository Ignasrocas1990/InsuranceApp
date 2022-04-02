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
    /// <summary>
    /// Class used to store and manipulate ClaimPage UI inputs in real time via BindingContext and its properties
    /// </summary>
    [QueryProperty(nameof(TransferredCustomerId),"TransferredCustomerId")]
    public class ClaimViewModel : ObservableObject,IDisposable
    {
        public ICommand CreateClaimCommand { get; }
        public ICommand ViewPreviousClaimsCommand { get; }
        public ICommand ResolveClaimCommand { get; }
        public ICommand AddInfoCommand{ get; }

        private readonly ClaimManager claimManager;

        private string dateString;
        private string hospitalPostcode="";
        private string patientNr="";
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
            claimManager = new ClaimManager();
            AddInfoCommand = new AsyncCommand(AddExtraInfo);
        }
        
        /// <summary>
        /// Loads in data using manager classes via database and set it to Bindable properties(UI)
        /// </summary>
        public async Task SetUp()
        {
            try
            {
                UnderReviewDisplay = false;
                SetUpWaitDisplay = true;
                customerId = TransferredCustomerId == "" 
                    ? App.RealmApp.CurrentUser.Id : TransferredCustomerId;
            
                await claimManager.GetClaims(App.RealmApp.CurrentUser,customerId);
                var claim = claimManager.GetCurrentClaim();
                if (claim != null)
                {
                    extraInfo = claim.ExtraInfo;
                    var dtoDate = claim.StartDate;
                    var displayDateString = "Date Not found";
                    if (dtoDate !=null)
                    {
                        displayDateString = dtoDate.Value.Date.ToString("d");

                    }
                    
                    IsReadOnly = true;
                    DateDisplay = displayDateString;
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

                PreviousBtnIsEnabled = claimManager.GetResolvedClaimCount()>0;
                SetUpWaitDisplay = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Get Resolved claims via manager and display pop up with them
        /// </summary>
        private async Task GetClaims()
        {
            try
            {
                var closedClaims = claimManager.GetResolvedClaims() ?? new List<Claim>();
                
                await Application.Current.MainPage.Navigation
                    .ShowPopupAsync(new ExistingClaimsPopup(closedClaims));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        /// <summary>
        /// Creates Claim instance on user click
        /// while changing UI elements
        /// </summary>
        private async Task CreateClaim()
        {
            var answer = await Shell.Current.DisplayAlert(Msg.Notice,
               "Are you sure to open new Claim?", "create", "cancel");
           if (!answer) return;

            CircularWaitDisplay = true;
            IsReadOnly = true;

            await claimManager.CreateClaim(hospitalPostcode, patientNr, Type,App.RealmApp.CurrentUser,customerId,extraInfo);
            
            await Msg.Alert("New Claim has been Opened.\nClient will take a look at it shortly");
            ExtraBtnText = ViewExtraStr;
            CircularWaitDisplay = false;
            UnderReviewDisplay = true;
            if (customerId != App.RealmApp.CurrentUser.Id)
            {
                CanBeResolved = true; 
            }
        }
        /// <summary>
        /// Updates Claim (resolve) by client.
        /// And sends an email
        /// </summary>
        private async Task ResolveClaim()
        {
            try
            {
                var (reason, action) = await claimManager.GetClientAction(extraInfo);
                if (reason=="-1") return;
                
                CircularWaitDisplay = true;
                var customer = await claimManager.ResolveClaim(customerId,App.RealmApp.CurrentUser,reason,action);
                if (customer !=null)
                {
                    HttpService.ClaimNotifyEmail(customer.Email, customer.Name, DateTime.Now,action,reason);
                    await Msg.Alert(Msg.EmailSent);
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
        /// <summary>
        /// Displays a pop up so the user can add extra info.
        /// </summary>
        private async Task AddExtraInfo()
        {
            string popupDisplayText = "";
            if (extraInfo.Length > 10)
                popupDisplayText = extraInfo;
            var popUpHeading = "Please enter extra claim info";
            if (underReview)
            {
                popUpHeading = "Previously entered extra information";
            }
            var tempStr = await Application.Current.MainPage.Navigation.ShowPopupAsync(
                new EditorPopup(popUpHeading,underReview,popupDisplayText));
            if (tempStr != null && tempStr.Length > 10)
            {
                ExtraBtnText = ViewExtraStr;
                extraInfo = tempStr;
            }
            
        }


        //---------------------Bindable properties below------------------------------------
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

        private string transferredId="";
        public string TransferredCustomerId
        {
            get => transferredId;
            set =>  transferredId = Uri.UnescapeDataString(value ?? String.Empty);

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
            claimManager.Dispose();
        }
    }
}