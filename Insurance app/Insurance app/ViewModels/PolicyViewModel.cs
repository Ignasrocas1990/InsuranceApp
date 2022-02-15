using System;
using System.Collections.Generic;
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

namespace Insurance_app.ViewModels
{
    public class PolicyViewModel : ObservableObject
    {
        
        private bool wait;
        private int hospitals;
        private int cover;
        private int hospitalExcess;
        private int plan;
        private bool isSmokerChecker;
        public ICommand UpdatePolicy { get; }
        public ICommand HospitalInfoCommand { get; }
        public ICommand CoverInfoCommand { get; }
        public ICommand FeeInfoCommand { get; }
        public ICommand PlanInfoCommand { get; }
        public IList<String> HospitalList { get; } = StaticOpt.HospitalsEnum();
        public IList<String> CoverList { get; } = Enum.GetNames(typeof(StaticOpt.CoverEnum)).ToList();
        public IList<int> HospitalFeeList { get; } = StaticOpt.ExcessFee();
        public IList<String> PlanList { get; } = Enum.GetNames(typeof(StaticOpt.PlanEnum)).ToList();
        private PolicyManager policyManager;

        public PolicyViewModel()
        {
            UpdatePolicy = new AsyncCommand(Update);
            HospitalInfoCommand = new AsyncCommand(HospitalInfoPopup);
            CoverInfoCommand = new AsyncCommand(CoverInfoPopup);
            FeeInfoCommand = new AsyncCommand(FeeInfoPopup);
            PlanInfoCommand = new AsyncCommand(PlanInfoPopup);
            policyManager = new PolicyManager();
        }

        public void Setup()
        {
           //get stuff from database 
        }

        private async Task Update()
        {
            //go to database and update
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
        public int SelectedHospitalExcess
        {
            get => hospitalExcess;
            set => SetProperty(ref hospitalExcess, value);
        }
        public int SelectedPlan
        {
            get => plan;
            set => SetProperty(ref plan, value);
        }
        
        public bool IsSmoker
        {
            get => isSmokerChecker;
            set => SetProperty(ref isSmokerChecker, (UpdateSmokerValue(value)));
        }

        private int smoker;
        private bool UpdateSmokerValue(bool value)
        {
            smoker = value ? 1 : 0;
            return value;
        }
        private bool updating;
        public bool Updating
        {
            get => updating;
            set => SetProperty(ref updating, value);
        }

        private string startDate;
        public string StartDateDisplay
        {
            get => startDate;
            set => SetProperty(ref startDate, value);
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