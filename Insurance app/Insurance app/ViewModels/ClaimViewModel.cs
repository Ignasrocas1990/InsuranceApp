using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Pages;
using Java.Lang.Reflect;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class ClaimViewModel : ObservableObject,IDisposable
    {
        public ICommand CreateClaimCommand { get; }
        public ICommand ViewPreviousClaimsCommand { get; }
        private readonly ClaimManager claimManager;

        private string dateString;
        private string hospitalPostcode="";
        private string patientNr="";
        private string status = "Not Created";
        private string type = "health";//If application extended, this has to be moved to App

        public ClaimViewModel()
        {
            CreateClaimCommand = new AsyncCommand(CreateClaim);
            ViewPreviousClaimsCommand = new AsyncCommand(GetClaims);
            claimManager = new ClaimManager();
            
        }

        public async Task SetUp()
        {
            await claimManager.GetClaims(App.RealmApp.CurrentUser);
            var claim = claimManager.GetCurrentClaim();
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
            }
            else
            {
                IsReadOnly = false;
                DateDisplay = DateTimeOffset.Now.Date.ToString("d");
                HospitalPostCodeDisplay = "";
                PatientNrDisplay = "";
                StatusDisplay = "Not Created";
            }
            CreateBtnIsEnabled = !IsReadOnly;

            PreviousBtnIsEnabled = claimManager.Claims.Count > 1;
        }

        private async Task GetClaims()
        {
            await Application.Current.MainPage.Navigation
                .ShowPopupAsync(new ExistingClaimsPopup(claimManager.Claims));
        }
        
        private async Task CreateClaim()
        {
           bool answer = await Shell.Current.DisplayAlert("Notice",
               "Are you sure to open new Claim?", "create", "cancel");
           if (!answer) return;

            CircularWaitDisplay = true;
            IsReadOnly = true;
            CreateBtnIsEnabled = !IsReadOnly;

            await claimManager.CreateClaim(hospitalPostcode, patientNr, type, true,App.RealmApp.CurrentUser);
            
            StatusDisplay = "open";
            
            await Shell.Current.DisplayAlert("Message", 
                "New Claim has been Opened,\n Client will take a look at it shortly",
                "close");
            CircularWaitDisplay = false;
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

        bool createBtnEnabled;
        public bool CreateBtnIsEnabled
        {
            get => createBtnEnabled;
            set => SetProperty(ref createBtnEnabled, value);
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

        public void Dispose()
        {
            claimManager.Dispose();
        }
    }
}