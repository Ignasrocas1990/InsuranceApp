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
using Insurance_app.Pages.Popups;
using Xamarin.CommunityToolkit.Extensions;


namespace Insurance_app.ViewModels
{
    public class QuoteViewModel : ObservableObject
    {
        public ICommand GetQuotCommand { get; }
        private int responseCounter = 0;
        private InferenceService inf;
        private bool wait;
        private bool tooLate;
        private int hospitals;
        private int cover;
        private int hospitalExcess;
        private int plan;
        private int smoker=0;
        private bool isSmokerChecker=false;
        private readonly Timer timer;
        private string elegalChars = "";


        public ICommand InfoCommand { get; }
        public IList<String> HospitalList { get; } = StaticOpt.HospitalsEnum();
        //age
        public IList<String> CoverList { get; } = Enum.GetNames(typeof(StaticOpt.CoverEnum)).ToList();
        public IList<int> HospitalFeeList { get; } = StaticOpt.ExcessFee();
        public IList<String> PlanList { get; } = Enum.GetNames(typeof(StaticOpt.PlanEnum)).ToList();

        public QuoteViewModel()
       {
           timer = new Timer(1000);
           timer.Elapsed += CheckResponseTime;
           GetQuotCommand = new AsyncCommand(GetQuote);
           inf = new InferenceService();
           InfoCommand = new AsyncCommand<string>(StaticOpt.InfoPopup);
       }
        private async Task GetQuote()
       {
           if (!App.NetConnection())
           {
               await Application.Current.MainPage.DisplayAlert("error",StaticOpt.NCE, "close");
               return;
           }
           if (elegalChars != "")
           {
               await Application.Current.MainPage.DisplayAlert("Error",elegalChars , "close");
               return;
           }
            
           var age = DateTime.Now.Year - selectedDate.Year;
          
           string price;
           
           try
           {
               CircularWaitDisplay=true;
                timer.Start();
                price =  await inf.SendQuoteRequest(hospitals, age, cover, hospitalExcess, plan, smoker);
                timer.Stop();
                if (tooLate)
                {
                    tooLate = false;
                    return;
                }

           }
           catch
           {
                timer.Stop();
                responseCounter = 0;
                await Application.Current.MainPage.DisplayAlert("Error", "Something went wrong, try again in a min", "close");
               return;
           }
           CircularWaitDisplay=false;
           responseCounter = 0;
            bool action = await Application.Current.MainPage.DisplayAlert("Price",$"Price for the quote is : {price}",  "Accept","Deny");
           if (action)
           {
               try
               {   
                   var tempQuote = new Dictionary<string, int>
                   {
                       {"Hospitals",hospitals},
                       {"Age",age},
                       {"Cover",cover},
                       {"Hospital_Excess",hospitalExcess},
                       {"Plan",plan},
                       {"Smoker",smoker},
                       {selectedDate.ToString("d"), -1}
                   };
                   //var jsonQuote = JsonConvert.SerializeObject(tempQuote);
                   //await Shell.Current.GoToAsync($"//{nameof(RegistrationPage)}?PriceDisplay={price}&TempQuote={jsonQuote}");
                   await Application.Current.MainPage.Navigation.PushAsync(new RegistrationPage(tempQuote,price));
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
           if (responseCounter != StaticOpt.MaxResponseTime) return;
           tooLate = true;
           CircularWaitDisplay=false;
           responseCounter = 0;
           await Application.Current.MainPage.DisplayAlert("Error",StaticOpt.NCE, "close");
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
    }
}