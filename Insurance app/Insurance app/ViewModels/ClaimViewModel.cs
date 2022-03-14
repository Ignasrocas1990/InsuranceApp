using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
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

        public readonly ClaimManager ClaimManager;

        private string dateString;
        private string hospitalPostcode="";
        private string patientNr="";
        private string status = "Not Created";
        private const string Type = "health"; //If application extended, this has to be moved to App
        private string customerId = "";
        private HttpService api;
        

        public ClaimViewModel()
        {
            CreateClaimCommand = new AsyncCommand(CreateClaim);
            ViewPreviousClaimsCommand = new AsyncCommand(GetClaims);
            ResolveClaimCommand = new AsyncCommand(ResolveClaim);
            ClaimManager = new ClaimManager();
            api = new HttpService();

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
                    StatusDisplay = "open";
                    UnderReviewDisplay = true;
                    if (customerId != App.RealmApp.CurrentUser.Id)
                    {
                        CanBeResolved = true;
                    }
                }
                else
                {
                    IsReadOnly = false;
                    DateDisplay = DateTimeOffset.Now.Date.ToString("d");
                    HospitalPostCodeDisplay = "";
                    PatientNrDisplay = "";
                    StatusDisplay = "Not Created";
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

            await ClaimManager.CreateClaim(hospitalPostcode, patientNr, Type,App.RealmApp.CurrentUser,customerId);
            
            StatusDisplay = "open";
            
            await Shell.Current.DisplayAlert(Msg.Notice, 
                "New Claim has been Opened.\nClient will take a look at it shortly",
                "close");
            CircularWaitDisplay = false;
        }
        
        private async Task ResolveClaim()
        {
            try
            {
                var action = await Shell.Current.DisplayAlert(Msg.Notice, 
                    "Do you want to Resolve claim by Accepting or Denying it?", "Accept", "Deny");

                var answerString  = action ? "Accept" : "Deny";
                
                var result = await Shell.Current.DisplayAlert(Msg.Notice, 
                    $"Are you sure you want to {answerString} the Claim?", "Yes", "No");
                if (!result) return;

                string reason="";
                if (!action)
                {
                    reason =await Application.Current.MainPage.Navigation.ShowPopupAsync(new EditorPopup());
                    if (reason is "")
                    {
                        await Shell.Current.DisplayAlert
                            (Msg.Notice, "Claim cant be Denied without a reason", "close");
                        return;
                    }
                }
                // pass in reason, and action to save to claim
                CircularWaitDisplay = true;
                var customer = await ClaimManager.ResolveClaim(customerId,App.RealmApp.CurrentUser,reason,action);
                if (customer !=null)
                {
                    api.ClaimNotifyEmail(customer.Email, customer.Name, DateTime.Now,action,reason);
                    await Shell.Current.DisplayAlert(Msg.Notice, Msg.EmailSent, "close");
                }
                
                CircularWaitDisplay = false;
                CanBeResolved = false;
                await SetUp();

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
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
        
        public string StatusDisplay
        {
            get => status;
            set => SetProperty(ref status, value);
        }
        private bool wait;
        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        private bool fieldsEnabled;
        public bool IsReadOnly// opposite to this
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

        public void Dispose()
        {
            ClaimManager.Dispose();
        }
    }
}