using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Pages;
using Java.Lang.Reflect;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class ClaimViewModel : ObservableObject
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
            ViewPreviousClaimsCommand = new Command(GetClaims);
            claimManager = new ClaimManager();
            
        }

        public async Task SetUp()
        {
            await claimManager.GetClaims(App.RealmApp.CurrentUser);
            var claim = claimManager.GetCurrentClaim();
            if (claim != null)
            {
                FieldEnabledDisplay = false;
                DateDisplay = $"{claim.StartDate}";
                HospitalPostCodeDisplay = claim.HospitalPostCode;
                PatientNrDisplay = claim.PatientNr;
                StatusDisplay = "open";
            }
            else
            {
                DateDisplay = $"{DateTimeOffset.Now.DateTime}";
            }

            if (claimManager.Claims.Count > 1)
            {
                PreviousButtonEnabled = true;
            }
        }

        private void GetClaims()
        {
            var displayClaimsPopup = new ExistingClaimsPopup(claimManager.Claims);
        }
        
        private async Task CreateClaim()
        {
            CircularWaitDisplay = true;
            FieldEnabledDisplay = false;
            await claimManager.CreateClaim(hospitalPostcode, patientNr, type, true,App.RealmApp.CurrentUser);
            StatusDisplay = "open";
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
            get => $"Status : {status}";
            set => SetProperty(ref status, value);
        }
        private bool wait;
        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        private bool fieldsEnabled=true;
        public bool FieldEnabledDisplay
        {
            get => fieldsEnabled;
            set => SetProperty(ref fieldsEnabled, value);
        }

        private bool previousExist;
        public bool PreviousButtonEnabled
        {
            get => previousExist;
            set => SetProperty(ref previousExist, value);
        }

        public string DateDisplay
        {
            get => dateString;
            set => SetProperty(ref dateString, value);
        }
    }
}