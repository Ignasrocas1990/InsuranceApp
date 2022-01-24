using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Models;
using Insurance_app.Pages;
using Newtonsoft.Json;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

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

        
        public RegistrationViewModel()
        {
            RegisterCommand = new AsyncCommand(Register);
        }

        private async Task Register()
        {
            try
            {
                CircularWaitDisplay = true;
                var registered = await App.RealmDb.Register(email, password);
                
                if (registered == "success")
                {
                    CircularWaitDisplay = true;
                    var user =  await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    var customer = new Customer()
                   {
                       Id = user.Id, Age = Quote["Age"],
                       Name = fName, LastName = lName, PhoneNr = phoneNr, Email=email, Partition = $"Customer ={user.Id}"
                   };
                   customer.Policy = new PersonalPolicy()
                    {
                        Price = getPrice(),
                        Cover = Quote["Cover"], HospitalFee = Quote["Hospital_Excess"],
                        Hospitals = Quote["Hospitals"], Plan = Quote["Plan"], Smoker = Quote["Smoker"],
                        Status = true, StartDate = DateTime.UtcNow
                    };
                   Customer c = await App.RealmDb.AddCustomer(customer);
                   if (c is null)
                       throw new Exception("Registration failed");
                   
                }
                else
                {
                    CircularWaitDisplay = false;
                    await Application.Current.MainPage.DisplayAlert("error", $"{registered}", "close");
                    return;
                }

            }
            catch (Exception e)
            {
                CircularWaitDisplay = false;
                Console.WriteLine(e);
                return;
            }
            await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
            CircularWaitDisplay = false;
            await Application.Current.MainPage.DisplayAlert("Notice", "Registration completed successfully", "Close");
            await Shell.Current.GoToAsync($"//{nameof(LogInPage)}",false);
       
           
        }
        // display properties
        public string PhoneNrDisplay
        {
            get => phoneNr;
            set => SetProperty(ref phoneNr, CheckPhoneNr(phoneNr,value));
        }

        private float getPrice()
        {
            return (float) Math.Round(float.Parse(price) * 100f) / 100f;
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