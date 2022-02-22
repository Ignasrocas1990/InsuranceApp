using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Logic;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Insurance_app.ViewModels
{
    public class PolicyViewModel : ObservableObject
    {
        
        private bool wait;
        private int hospitals;
        private int cover;
        private int fee;
        private int plan;
        private bool isSmoker;
        private float price;
        private DateTimeOffset date;
        public ICommand UpdatePolicy { get; }
        public ICommand HospitalInfoCommand { get; }
        public ICommand CoverInfoCommand { get; }
        public ICommand FeeInfoCommand { get; }
        public ICommand PlanInfoCommand { get; }
        public IList<string> HospitalList { get; } = StaticOpt.HospitalsEnum();
        public IList<string> CoverList { get; } = Enum.GetNames(typeof(StaticOpt.CoverEnum)).ToList();
        public IList<int> HospitalFeeList { get; } = StaticOpt.ExcessFee();
        public IList<string> PlanList { get; } = Enum.GetNames(typeof(StaticOpt.PlanEnum)).ToList();
        private readonly PolicyManager policyManager;

        public PolicyViewModel()
        {
            UpdatePolicy = new AsyncCommand(Update);
            HospitalInfoCommand = new AsyncCommand(HospitalInfoPopup);
            CoverInfoCommand = new AsyncCommand(CoverInfoPopup);
            FeeInfoCommand = new AsyncCommand(FeeInfoPopup);
            PlanInfoCommand = new AsyncCommand(PlanInfoPopup);
            policyManager = new PolicyManager();
            
        }

        public async Task Setup()
        {
            bool tempUpdate=false;
            try
            {
                SetUpWaitDisplay = true;
                UnderReviewDisplay = false;
                InfoIsVisible = false;
                var policyDic = await policyManager.FindPolicy(App.RealmApp.CurrentUser);//return <1,.>if under review
                var policy = policyDic.FirstOrDefault(u => u.Key == 1).Value;//try to see if under review
                if (policy is null)
                {
                    policy = policyDic[0];
                    tempUpdate = false;
                    if (policy.Price != null)
                    {
                        price = (float) policy.Price;
                        PriceDisplay = (Math.Round(price * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                        DisableColour = Color.SteelBlue;
                    }
                }
                else
                {
                    tempUpdate = true;
                    PriceDisplay = "Under Review";
                }
                if (policy.Hospitals != null) SelectedHospital = (int) policy.Hospitals;
                if (policy.Cover != null) SelectedCover = (int) policy.Cover;
                if (policy.HospitalFee != null) SelectedItemHospitalFee = (int) policy.HospitalFee;
                if (policy.Plan != null) SelectedPlan = (int) policy.Plan;
                IsSmokerDisplay = Convert.ToBoolean(policy.Smoker);
                if (policy.ExpiryDate != null)
                {
                    ExpiryDateDisplay = policy.ExpiryDate.Value.Date.ToString("d");
                    date = (DateTimeOffset) policy.ExpiryDate;
                }

               
            }
            catch (Exception e)
            {
                Console.WriteLine($"policy setup problem: \n {e}");
            }
            UnderReviewDisplay = tempUpdate;
            InfoIsVisible = !tempUpdate;
            SetUpWaitDisplay = false;
        }

        private async Task Update()
        {

            try
            {
                var answer = await Shell.Current.CurrentPage.DisplayAlert(
                    "Notice","You about to request to update the policy", "save", "cancel");
                if (!answer) return;
                UnderReviewDisplay = true;
                InfoIsVisible = !UnderReviewDisplay;
                CircularWaitDisplay = true;
                var newPolicy = policyManager.CreatePolicy(price,price,
                    cover, fee, hospitals, plan, smoker,
                    false, date,DateTimeOffset.Now.Date,App.RealmApp.CurrentUser.Id);
                await policyManager.AddPolicy(App.RealmApp.CurrentUser, newPolicy);
                PriceDisplay = "Under Review";
                CircularWaitDisplay = false;
                DisableColour = Color.SteelBlue;
                await Shell.Current.DisplayAlert("Message", "Update requested successfully", "close");
            }
            catch (Exception e)
            {
                Console.WriteLine($" Update policy error : {e}");
            }
        }

//-----------------------------data binding methods ------------------------------------------------
       public bool CircularWaitDisplay
       {
           get => wait;
           set => SetProperty(ref wait,value);
       }

       public int SelectedHospital
        {
            get => hospitals;
            set => SetProperty(ref hospitals, value);
        }
        public int SelectedCover
        {
            get => cover;
            set => SetProperty(ref cover, value);
        }
        public int SelectedItemHospitalFee
        {
            get => fee;
            set => SetProperty(ref fee, value);
        }
        public int SelectedPlan
        {
            get => plan;
            set => SetProperty(ref plan, value);
        }
        
        public bool IsSmokerDisplay
        {
            get => isSmoker;
            set => SetProperty(ref isSmoker, (UpdateSmokerValue(value)));
        }

        private int smoker;
        private bool UpdateSmokerValue(bool value)
        {
            smoker = value ? 1 : 0;
            return value;
        }
        private bool updating;
        public bool UnderReviewDisplay
        {
            get => updating;
            set => SetProperty(ref updating, value);
        }

        private string expiryDate;
        public string ExpiryDateDisplay
        {
            get => expiryDate;
            set => SetProperty(ref expiryDate, value);
        }

        private string priceString;
        

        public string PriceDisplay
        {
            get => priceString;
            set => SetProperty(ref priceString, value);
        }

        private Color dColor = Color.White;
        public Color DisableColour
        {
            get => dColor;
            set => SetProperty(ref dColor,value);
        }
        private bool setUpWait;
        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }

        private bool infoIsVisible;
        public bool InfoIsVisible
        {
            get => infoIsVisible;
            set => SetProperty(ref infoIsVisible, value);

        }

        //------------------------------ information popups ----------------------------      
        private async Task HospitalInfoPopup()
        {
            await Application.Current.MainPage.Navigation.ShowPopupAsync(new InfoPopup("Hospital"));
        }
        private async Task CoverInfoPopup()
        {
            await Application.Current.MainPage.Navigation.ShowPopupAsync(new InfoPopup("Cover"));
        }
        private async Task FeeInfoPopup()
        {
            await Application.Current.MainPage.Navigation.ShowPopupAsync(new InfoPopup("Fee"));
        }
        private async Task PlanInfoPopup()
        {
            await Application.Current.MainPage.Navigation.ShowPopupAsync(new InfoPopup("Plan"));
        }

    }
}