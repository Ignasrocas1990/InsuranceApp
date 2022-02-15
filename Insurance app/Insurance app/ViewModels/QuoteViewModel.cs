using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Newtonsoft.Json;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Timers;
using Android.Util;
using Insurance_app.Pages.Popups;
using Xamarin.CommunityToolkit.Extensions;


namespace Insurance_app.ViewModels
{
    public class QuoteViewModel : ObservableObject
    {
        public ICommand GetQuotCommand { get; }
        private bool buttonEnabled = true;
        private int responseCounter = 0;
        private InferenceService inf;
        private bool wait;

        private int hospitals;
        private int cover;
        private int hospitalExcess;
        private int plan;
        private int smoker=0;
        private bool isSmokerChecker=false;
        private readonly Timer timer;
        private string elegalChars = "";
        public ICommand HospitalInfoCommand { get; }
        public ICommand CoverInfoCommand { get; }
        public ICommand FeeInfoCommand { get; }
        public ICommand PlanInfoCommand { get; }
        public IList<String> HospitalList { get; } = StaticOpt.HospitalsEnum();
        //age
        public IList<String> CoverList { get; } = Enum.GetNames(typeof(StaticOpt.CoverEnum)).ToList();
        public IList<int> HospitalFeeList { get; } = StaticOpt.ExcessFee();
        public IList<String> PlanList { get; } = Enum.GetNames(typeof(StaticOpt.PlanEnum)).ToList();

        public QuoteViewModel()
       {
           timer = new Timer(1000);
           GetQuotCommand = new AsyncCommand(GetQuote);
           inf = new InferenceService();
           timer.Elapsed += CheckResponseTime;
           HospitalInfoCommand = new AsyncCommand(HospitalInfoPopup);
           CoverInfoCommand = new AsyncCommand(CoverInfoPopup);
           FeeInfoCommand = new AsyncCommand(FeeInfoPopup);
           PlanInfoCommand = new AsyncCommand(PlanInfoPopup);
       }



        private async Task GetQuote()
       {
           if (!App.NetConnection())
           {
               await Shell.Current.CurrentPage.DisplayAlert("error",StaticOpt.ConnectionErrorMessage, "close");
               return;
           }
           if (elegalChars != "")
           {
               await Shell.Current.CurrentPage.DisplayAlert("Error",elegalChars , "close");
               return;
           }

           var age = DateTime.Now.Year - selectedDate.Year;
           var tempQuote = new Dictionary<string, int>()
           {
               {"Hospitals",hospitals},
               {"Age",age},
               {"Cover",cover},
               {"Hospital_Excess",hospitalExcess},
               {"Plan",plan},
               {"Smoker",smoker}
           };
           string price = " ";
           
           try
           {
               CircularWaitDisplay=true;
                ButtonEnabled = false;
                timer.Start();
                var result = await inf.Predict(tempQuote);
               price =  await result.Content.ReadAsStringAsync();

           }
           catch (Exception e)
           {
               CircularWaitDisplay=false;
                ButtonEnabled = true;
                timer.Stop();
                responseCounter = 0;
                await Shell.Current.CurrentPage.DisplayAlert("Error", StaticOpt.ConnectionErrorMessage, "close");
               return;
           }
           CircularWaitDisplay=false;
           ButtonEnabled = true;
           timer.Stop();
           responseCounter = 0;
            bool action = await Shell.Current.CurrentPage.DisplayAlert("Price",price,  "Accept","Deny");
           if (action)
           {
               try
               {   
                   tempQuote.Add(selectedDate.ToString("d"),-1);
                   var jsonQuote = JsonConvert.SerializeObject(tempQuote);
                   await Shell.Current.GoToAsync($"//{nameof(RegistrationPage)}?PriceDisplay={price}&TempQuote={jsonQuote}");
               }
               catch (Exception e)
               {
                   Console.WriteLine(e);
               }
           } 
       }

       private async void CheckResponseTime(object o, ElapsedEventArgs e)
       {
           responseCounter += 1;
           if (responseCounter == StaticOpt.MaxResponseTime)
           {
               CircularWaitDisplay=false;
               ButtonEnabled = true;
               responseCounter = 0;
               await Shell.Current.CurrentPage.DisplayAlert("Error",StaticOpt.ConnectionErrorMessage, "close");
           }
       }

//-----------------------------data binding methods ------------------------------------------------
       public bool CircularWaitDisplay
       {
           get => wait;
           set => SetProperty(ref wait,value);
       }

       public bool ButtonEnabled
        {
           get => buttonEnabled;
           set => SetProperty(ref buttonEnabled, value);
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

        public DateTime MinDate { get; } = DateTime.Now.AddYears(-65);
        public DateTime MaxDate { get; } = DateTime.Now.AddYears(-18);
        
        private DateTime selectedDate = DateTime.Now.AddYears(-18);
        

        public DateTime SelectedDate
        {
            get => selectedDate;
            set => SetProperty(ref selectedDate, value);
        }
        
        public bool IsSmoker
        {
            get => isSmokerChecker;
            set => SetProperty(ref isSmokerChecker, UpdateSmokerValue(value));
        }
        private bool UpdateSmokerValue(bool value)
        {
            smoker = value ? 1 : 0;
            return value;
        }
        
        private bool enabled=true;
        public bool IsEnabled
        {
            get => enabled;
            set => SetProperty(ref enabled, value);
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