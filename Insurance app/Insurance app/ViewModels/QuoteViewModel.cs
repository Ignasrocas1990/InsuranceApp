﻿using System;
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


namespace Insurance_app.ViewModels
{
    public class QuoteViewModel : ObservableObject
    {
        private QuoteOptions quoteOptions;
        public ICommand GetQuotCommand { get; }
        private bool buttonEnabled = true;
        private int responseCounter = 0;

        private InferenceService inf;
        private bool wait;

        private int hospitals;
        private int age;
        private int cover;
        private int hospitalExcess;
        private int plan;
        private int smoker=0;
        private bool isSmokerChecker=false;
        private readonly Timer timer;
        private string elegalChars = "";



        public IList<String> HospitalList { get; } = QuoteOptions.HospitalsEnum();
        //age
        public IList<String> CoverList { get; } = Enum.GetNames(typeof(QuoteOptions.CoverEnum)).ToList();
        public IList<int> HospitalFeeList { get; } = QuoteOptions.ExcessFee();
        public IList<String> PlanList { get; } = Enum.GetNames(typeof(QuoteOptions.PlanEnum)).ToList();

        public QuoteViewModel()
       {
           timer = new Timer(1000);
           quoteOptions = new QuoteOptions();
           GetQuotCommand = new AsyncCommand(GetQuote);
           inf = new InferenceService();
           timer.Elapsed += CheckResponseTime;

          
       }

       private async Task GetQuote()
       {
           if (!App.NetConnection())
           {
               await Shell.Current.CurrentPage.DisplayAlert("error",StaticOptions.ConnectionErrorMessage, "close");
               return;
           }
           if (elegalChars != "")
           {
               await Shell.Current.CurrentPage.DisplayAlert("Error",elegalChars , "close");
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
                timer.Start();
                var result = await inf.Predict(TempQuote);
               price =  await result.Content.ReadAsStringAsync();

           }
           catch (Exception e)
           {
               CircularWaitDisplay=false;
                ButtonEnabled = true;
                timer.Stop();
                responseCounter = 0;
                await Shell.Current.CurrentPage.DisplayAlert("Error", StaticOptions.ConnectionErrorMessage, "close");
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
                   var jsonQuote = JsonConvert.SerializeObject(TempQuote);
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
           if (responseCounter == StaticOptions.MaxResponseTime)
           {
               CircularWaitDisplay=false;
               ButtonEnabled = true;
               responseCounter = 0;
               await Shell.Current.CurrentPage.DisplayAlert("Error",StaticOptions.ConnectionErrorMessage, "close");
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
            set => SetProperty(ref age, CheckAge(value));
        }
        private int CheckAge(int value)
        {
            if ( value > 17 && value < 66)
            {
                elegalChars = "";
            }
            else
            {
                elegalChars = StaticOptions.AgeLimitErrorMessage;
            }
            return value;
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
    }
}