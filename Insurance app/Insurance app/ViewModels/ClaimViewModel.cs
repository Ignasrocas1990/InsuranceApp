using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Java.Lang.Reflect;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Insurance_app.ViewModels
{
    public class ClaimViewModel : ObservableObject
    {
        public ICommand CreateClaimCommand { get; }
        public ICommand ViewPreviousClaimsCommand { get; }
        private readonly ClaimManager claimManager;
        
        private string hospitalPostcode="";
        private string patientNr="";
        private string status = "Not Created";
        private string type = "health";//If application extended, this has to be moved to App

        public ClaimViewModel()
        {
            CreateClaimCommand = new AsyncCommand(CreateClaim);
            ViewPreviousClaimsCommand = new AsyncCommand(getClaims);
            claimManager = new ClaimManager();
        }

        private async Task getClaims()
        {
            
            //get claims and create list view model inside a pop up.
        }
        
//right now can only have 1 claim open at a time.
        private async Task CreateClaim()
        {
            CircularWaitDisplay = true;
            fieldsEnabled = false;
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
            get => status;
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
        public bool PreviousExistDisplay
        {
            get => previousExist;
            set => SetProperty(ref previousExist, value);
        }
    }
}