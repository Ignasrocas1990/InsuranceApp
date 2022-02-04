﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Java.Lang;
using Newtonsoft.Json;
using Realms;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Exception = System.Exception;

namespace Insurance_app.ViewModels
{    [QueryProperty(nameof(PriceDisplay),nameof(PriceDisplay))]
    [QueryProperty(nameof(TempQuote),nameof(TempQuote))]
    public class RegistrationViewModel : ObservableObject
    {
        INavigation nav;
        private Dictionary<string, int> Quote;
        public ICommand RegisterCommand { get; }
        private bool wait;
        private string price;
        private string email;
        private string password;
        private string fName;
        private string lName;
        private string phoneNr;
        private bool inputsValid = false;
        private string qString;
        private UserManager userManager;
        private PolicyManager policyManager;


        public RegistrationViewModel()
        {
            userManager = new UserManager();
            policyManager = new PolicyManager();
            RegisterCommand = new AsyncCommand(Register);
        }

        private async Task Register()
        {
            try
            {
                CircularWaitDisplay = true;
                var registered = await userManager.Register(email, password);

                if (registered == "success")
                {
                    var user =  await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                   var customer = userManager.CreateCustomer(user.Id,Quote["Age"],fName, lName,phoneNr,email);
                   customer.Policy=policyManager.CreatePolicy(price, Quote["Cover"], Quote["Hospital_Excess"],
                       Quote["Hospitals"], Quote["Plan"], Quote["Smoker"], true, DateTime.UtcNow,user.Id);

                   await userManager.AddCustomer(customer, App.RealmApp.CurrentUser);
                   await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                   userManager.StopSync();
                   if (App.RealmApp.CurrentUser != null)
                   {
                       await App.RealmApp.CurrentUser.LogOutAsync();
                   }
                   else
                   {
                       Console.WriteLine("user longed out");
                   }
                   CircularWaitDisplay = false;
                   
                   await Application.Current.MainPage.DisplayAlert("Notice", "Registration completed successfully", "Close");
                   await Shell.Current.GoToAsync($"//{nameof(LogInPage)}",false);
                }
                else
                {
                    CircularWaitDisplay = false;
                    await Shell.Current.DisplayAlert("error", $"{registered}", "close");
                }
            }
            catch (Exception e)
            {
                CircularWaitDisplay = false;
                await Shell.Current.DisplayAlert("error", "registration failed", "close");
                Console.WriteLine(e);
                return;
            }



        }
        // display properties
        public string PhoneNrDisplay
        {
            get => phoneNr;
            set => SetProperty(ref phoneNr, CheckPhoneNr(phoneNr,value));
        }
        
        private string CheckPhoneNr(string oldValue,string newValue)
        {
            if (newValue.Length == 0) return oldValue;
            if (newValue[0] != '+')
            {
                //notification.Notify("error", "number has to start with +", "close");
                // dont use Notification ... instead create an invisible Label with error message (bellow error box)
                return oldValue;
            }
            return newValue;
        }

        public string EmailDisplay
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        public string PassDisplay
        {
            get => password;
            set => SetProperty(ref password, PassCheck(value));
        }

        private string PassCheck(string newValue)
        {
            if (newValue.Length < 6) inputsValid = false;//we can set the label below to red or something
            return newValue;
        }

        public string FNameDisplay
        {
            get => fName;
            set => SetProperty(ref fName, value);
        }
        public string LNameDisplay
        {
            get => lName;
            set => SetProperty(ref lName, value);
        }
        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }

        public string TempQuote
        {
            get => qString;
            set
            {
                var v = Uri.UnescapeDataString(value ?? string.Empty);
                Quote= JsonConvert.DeserializeObject<Dictionary<string,int>>(v);
            } 
        }

        public string PriceDisplay
        {
            get => price;
            set
            {
                price = Uri.UnescapeDataString(value ?? string.Empty);
                
            } 
        
        }
    }
}