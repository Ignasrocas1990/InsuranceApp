﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.BLE;
using Insurance_app.Models;
using Insurance_app.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class QuoteViewModel : ObservableObject
    {
        private QuoteOptions quoteOptions;
        public ICommand GetQuotCommand { get; }
        private INotification notification; 
        private INavigation nav;
        private InferenceService inf;

        private int hospitals=0;
        private int age=18;
        private int cover=0;
        private int hospitalExcess=150;
        private int plan=0;
        private int smoker=0;
        private bool wait;
        
        public QuoteViewModel() { }
        public QuoteViewModel(INotification notification,INavigation nav)
       {
           quoteOptions = new QuoteOptions();
           GetQuotCommand = new AsyncCommand(GetQuote);
           this.notification = notification;
           this.nav = nav;
           inf = new InferenceService();
       }

       private async Task GetQuote()
       {
           var TempQuote = new Dictionary<string, int>()
           {
               {"Hospitals",0},
               {"Age",18},
               {"Cover",0},
               {"Hospital_Excess",150},
               {"Plan",0},
               {"Smoker",0}
           };
           string priceString = " ";
           CircularWaitDisplay=true;
           var result = await inf.Predict(TempQuote);
           priceString =  await result.Content.ReadAsStringAsync();
           CircularWaitDisplay=false;
           
           bool action = await notification.NotifyOption("Price",priceString,  "Accept","Deny");
           if (action)
           {
               float price =0;
               float.TryParse(priceString, out price);
               try
               {   
                   var registrationPage = new RegistrationPage(price,TempQuote);
                   await nav.PushAsync(registrationPage);
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