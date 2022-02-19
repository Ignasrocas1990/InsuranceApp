using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Newtonsoft.Json;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Exception = System.Exception;
using Xamarin.CommunityToolkit.Extensions;


namespace Insurance_app.ViewModels
{    //[QueryProperty(nameof(PriceDisplay),nameof(PriceDisplay))]
    //[QueryProperty(nameof(TempQuote),nameof(TempQuote))]
    public class RegistrationViewModel : ObservableObject
    {
        private readonly Dictionary<string, int> quote;
        public ICommand RegisterCommand { get; }
        private bool wait;
        private string price="";
        private string email="";
        private string password="";
        private string fName="";
        private string lName="";
        private string phoneNr="";
        //private string qString="";
        private string addressText = "Add address";
        private readonly UserManager userManager;
        private readonly PolicyManager policyManager;
        private Address address;
        public ICommand AddressCommand { get; }

/*
        public RegistrationViewModel()
        {
            address = new Address();
            userManager = new UserManager();
            policyManager = new PolicyManager();
            RegisterCommand = new AsyncCommand(Register);
            AddressCommand = new AsyncCommand(GetAddress);
        }
*/
        public RegistrationViewModel(Dictionary<string, int> tempQuote, string price)
        {
            address = new Address();
            userManager = new UserManager();
            policyManager = new PolicyManager();
            RegisterCommand = new AsyncCommand(Register);
            AddressCommand = new AsyncCommand(GetAddress);
            PriceDisplay = price;
            quote = tempQuote;
        }

        private async Task Register()
        {
            try
            {
                var errors = "";//StaticOptions.IsValid(fName, lName, phoneNr, email);//TODO uncomment
                errors += "";//StaticOptions.IsPasswordValid(password);
                if (errors.Length > 2)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", errors, "close");
                    return;
                }
                CircularWaitDisplay = true;
                var registered = await userManager.Register(email, password);

                if (registered == "success")
                {
                    var user =  await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    
                   var customer = userManager.CreateCustomer(GetDob(),fName, lName,phoneNr,email,address);
                   if (customer is null)
                   {
                       throw new Exception("registration failed");
                   }
                   customer.Policy.Add(policyManager.CreatePolicy(price, quote["Cover"], quote["Hospital_Excess"],
                       quote["Hospitals"], quote["Plan"], quote["Smoker"], false, DateTimeOffset.Now,user.Id,DateTimeOffset.Now));

                   await userManager.AddCustomer(customer, App.RealmApp.CurrentUser);
                   await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                   userManager.Dispose();
                   if (App.RealmApp.CurrentUser != null)
                   {
                       await App.RealmApp.CurrentUser.LogOutAsync();
                   }
                   else
                   {
                       Console.WriteLine("user longed out");
                   }
                   
                   await Application.Current.MainPage.DisplayAlert("Notice", "Registration completed successfully", "Close");
                   await Application.Current.MainPage.Navigation.PushModalAsync(new LogInPage());
                }
                else
                {
                    
                    await Application.Current.MainPage.DisplayAlert("error", $"{registered}", "close");
                }
                
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert("error", "registration failed", "close");
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
        }

        private DateTimeOffset GetDob()
        {
            try
            {
                var dateString = quote.FirstOrDefault(x => x.Value == -1).Key;
                return DateTimeOffset.Parse(dateString);
            }
            catch (Exception e)
            {
                Console.WriteLine($" GetDob error : {e}");
            }

            return DateTimeOffset.Now.AddYears(-18);
        }
        private async Task GetAddress()
        {
           var newAddress = await Application.Current.MainPage.Navigation.ShowPopupAsync<Address>(new AddressPopup(address));
            if (newAddress!=null)
            {
                AddressDisplay = "Address saved";
                address = newAddress;
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

        public string AddressDisplay
        {
            get => addressText;
            set => SetProperty(ref addressText, value);
        }
        public string PriceDisplay
        {
            get => price;
            set => SetProperty(ref price, value);
        }
        
/*
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
        
        }*/
        
    }
}