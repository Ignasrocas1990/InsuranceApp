using System;
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
        private Dictionary<string, int> quote;
        public ICommand RegisterCommand { get; }
        private bool wait=false;
        private string price="";
        private string email="";
        private string password="";
        private string fName="";
        private string lName="";
        private string phoneNr="";
        private string qString="";
        private readonly UserManager userManager;
        private readonly PolicyManager policyManager;
        private  string errors ="";


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
                IsValid();
                if (errors != "")
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Error", errors, "close");
                    return;
                }
                CircularWaitDisplay = true;
                var registered = await userManager.Register(email, password);

                if (registered == "success")
                {
                    var user =  await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                   var customer = userManager.CreateCustomer(user.Id,quote["Age"],fName, lName,phoneNr,email);
                   customer.Policy=policyManager.CreatePolicy(price, quote["Cover"], quote["Hospital_Excess"],
                       quote["Hospitals"], quote["Plan"], quote["Smoker"], true, DateTime.UtcNow,user.Id);

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
        /**
         * Validating inputs
         */
        private void IsValid()
        {
            if (fName.Length < 2 || fName.Length > 20 || StaticOptions.HasNumbers(fName))
            {
                errors += " First name is invalid \n";
            }
            if (lName.Length < 4 || lName.Length > 20 || StaticOptions.HasNumbers(lName))
            {
                errors += " Last name is invalid \n";
            }
            if (phoneNr.Length < 9 || StaticOptions.HasNumbers(lName))
            {
                errors += " Phone nr is invalid \n";
            }
            
            if (password.Length < 7)
            {
                errors += " Password is invalid \n";
            }
            if (email.Length < 15 || !email.Contains("@") || !email.Contains("."))
            {
                errors += " Email is invalid \n";
            }
            
            
        }

        // property bindings
        public string PhoneNrDisplay
        {
            get => phoneNr;
            set => SetProperty(ref phoneNr,value);
        }

        public string EmailDisplay
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        public string PassDisplay
        {
            get => password;
            set => SetProperty(ref password, value);
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
                quote= JsonConvert.DeserializeObject<Dictionary<string,int>>(v);
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