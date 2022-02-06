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

namespace Insurance_app.ViewModels
{
    public class QuoteViewModel : ObservableObject
    {
        private QuoteOptions quoteOptions;
        public ICommand GetQuotCommand { get; }
        private bool _gettingQuote = false;
        private bool buttonEnabled = true;

        private InferenceService inf;
        private bool wait;

        private int hospitals;
        private int age;
        private int cover;
        private int hospitalExcess;
        private int plan;
        private int smoker=0;
        private bool isSmokerChecker=false;

        public IList<String> HospitalList { get; } = QuoteOptions.HospitalsEnum();
        //age
        public IList<String> CoverList { get; } = Enum.GetNames(typeof(QuoteOptions.CoverEnum)).ToList();
        public IList<int> HospitalFeeList { get; } = QuoteOptions.ExcessFee();
        public IList<String> PlanList { get; } = Enum.GetNames(typeof(QuoteOptions.PlanEnum)).ToList();

        public QuoteViewModel()
       {
           quoteOptions = new QuoteOptions();
           GetQuotCommand = new AsyncCommand(GetQuote);
           inf = new InferenceService();
          
       }

       private async Task GetQuote()
       {
           if (!App.NetConnection())
           {
               await Application.Current.MainPage.DisplayAlert("error", "Network connectivity not available", "close");
               return;
           }
           var TempQuote = new Dictionary<string, int>()
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
                var result = await inf.Predict(TempQuote);
               price =  await result.Content.ReadAsStringAsync();

           }
           catch (Exception e)
           {
               CircularWaitDisplay=false;
                ButtonEnabled = true;
                await Shell.Current.CurrentPage.DisplayAlert("Error", "Connection not found", "close");
               return;
           }
           CircularWaitDisplay=false;
           ButtonEnabled = true;
            bool action = await Application.Current.MainPage.DisplayAlert("Price",price,  "Accept","Deny");
           if (action)
           {
               try
               {   
                   var jsonQuote = JsonConvert.SerializeObject(TempQuote);
                   await Shell.Current.GoToAsync($"//{nameof(RegistrationPage)}?PriceDisplay={price}&TempQuote={jsonQuote}");
               }
               catch (Exception e)
               {
                   Console.WriteLine(e);
               }
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
        public int AgeEntry
        {
            get => age;
            set => SetProperty(ref age, checkAge(value));
        }
        private int checkAge(int age)
        {
            return age;// need to check here with pop up or create an invisable lable 
        }


        public bool isSmoker
        {
            get => isSmokerChecker;
            set => SetProperty(ref isSmokerChecker, updateSmokerValue(value));
        }
        private bool updateSmokerValue(bool value)
        {
            if (value)
            smoker = value ? 1 : 0;
            return value;
        }
    }
}