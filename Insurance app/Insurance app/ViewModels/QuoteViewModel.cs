﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Models;
using Insurance_app.Pages;
using Newtonsoft.Json;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class QuoteViewModel : ObservableObject
    {
        private QuoteOptions quoteOptions;
        public ICommand GetQuotCommand { get; }
        public INotification notification; 
        private InferenceService inf;

        private int hospitals=0;
        private int age=18;
        private int cover=0;
        private int hospitalExcess=150;
        private int plan=0;
        private int smoker=0;
        private bool wait;
        
        public QuoteViewModel()
       {
           quoteOptions = new QuoteOptions();
           GetQuotCommand = new AsyncCommand(GetQuote);
           inf = new InferenceService();
       }

       private async Task GetQuote()
       {
           if (!App.Connected)
           {
               await notification.Notify("error", "Network connectivity not available", "close");
               return;
           }
           var TempQuote = new Dictionary<string, int>()
           {
               {"Hospitals",0},
               {"Age",18},
               {"Cover",0},
               {"Hospital_Excess",150},
               {"Plan",0},
               {"Smoker",0}
           };
           string price = " ";
           CircularWaitDisplay=true;
           var result = await inf.Predict(TempQuote);
           price =  await result.Content.ReadAsStringAsync();
           CircularWaitDisplay=false;
           
           bool action = await notification.NotifyOption("Price",price,  "Accept","Deny");
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
       public bool CircularWaitDisplay
       {
           get => wait;
           set => SetProperty(ref wait, value);
       }
    }
}