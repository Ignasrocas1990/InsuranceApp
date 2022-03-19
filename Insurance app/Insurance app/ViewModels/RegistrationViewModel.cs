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
{    
    public class RegistrationViewModel : ObservableObject,IDisposable
    {
        private readonly Dictionary<string, string> quote;
        public readonly string AddressSText = "Address saved";
        private bool wait;
        private string price="";
        private string email="";
        private string password="";
        private string fName="";
        private string lName="";
        private string phoneNr="";
        public string AddressText = "Add address please";
        private readonly UserManager userManager;
        private readonly PolicyManager policyManager;
        private Address address;
        public ICommand AddressCommand { get; }
        public RegistrationViewModel(Dictionary<string, string> tempQuote, string price)
        {
            address = new Address();
            userManager = new UserManager();
            policyManager = new PolicyManager();
            AddressCommand = new AsyncCommand(GetAddress);
            PriceDisplay = price;
            quote = tempQuote;
        }

        public async Task Register()
        {
            try
            {
                CircularWaitDisplay = true;
                var registered = await userManager.Register(email, password);

                if (registered == "success")
                {
                    var user =  await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    
                    if (user is null)throw new Exception("registration failed");
                        
                   var customer = userManager.CreateCustomer(GetDob(),fName, lName,phoneNr,email,address);
                        
                   if (customer is null)throw new Exception("registration failed");
                   var expiryDate = DateTimeOffset.Now.AddMonths(1);
                   var priceFloat = Converter.GetPrice(price);
                   customer.Policy.Add(policyManager.RegisterPolicy(priceFloat,0, quote["Cover"]
                       , int.Parse(quote["Hospital_Excess"]), quote["Hospitals"], quote["Plan"],
                       int.Parse(quote["Smoker"]), false,expiryDate,user.Id));

                   await userManager.AddCustomer(customer, App.RealmApp.CurrentUser);

                  await Msg.Alert("Registration completed successfully.\nRedirecting to payment page");
                   await Application.Current.MainPage.Navigation
                       .PushModalAsync(new PaymentPage(customer));
                }
                else
                {
                    await Msg.AlertError($"{registered}");
                }
                CircularWaitDisplay = false;
            }
            catch (Exception e)
            {
                CircularWaitDisplay = false;
                await Msg.AlertError("registration failed");
                Console.WriteLine(e);
            }
        }

        private DateTimeOffset GetDob()
        {
            try
            {
                var dateString = quote.FirstOrDefault(x => x.Value == "-1").Key;
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
                AddressDisplay = AddressSText;
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
            get => AddressText;
            set => SetProperty(ref AddressText, value);
        }
        public string PriceDisplay
        {
            get => price;
            set => SetProperty(ref price, value);
        }
        
        private bool setUpWait;

        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }

        public void Dispose()
        {
            userManager.Dispose();
        }
    }
}