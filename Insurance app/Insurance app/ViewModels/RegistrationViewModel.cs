using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Models;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class RegistrationViewModel : ObservableObject
    {
        private INotification notification;
        INavigation nav;
        private Dictionary<string, int> tempQuote;
        public ICommand RegisterCommand { get; }
        private bool wait;
        private readonly float price;
        private string email;
        private string pass;
        private string fName;
        private string lName;
        private string phoneNr;
        private bool inputsValid = false;
        
        public RegistrationViewModel(INotification notification, INavigation nav, float price, Dictionary<string, int> tempQuote)
        {
            this.notification = notification;
            this.nav = nav;
            this.price = price;
            this.tempQuote = tempQuote;
            RegisterCommand = new AsyncCommand(Register);
        }

        private async Task Register()
        {
            try
            {
                var registered = await App.RealmDb.Register(email, pass);
                var user = await App.RealmDb.Login(email, pass);

                if (registered == "success")
                {
                   Customer customer = new Customer()
                   {
                       Id = user.Id, Age = tempQuote["Age"],
                       Name = fName, LastName = lName, PhoneNr = phoneNr, Email=email, Partition = $"Customer ={user.Id}"
                   };
                   customer.Policy = new PersonalPolicy()
                    {
                        Price = price, Cover = tempQuote["Cover"], HospitalFee = tempQuote["Hospital_Excess"],
                        Hospitals = tempQuote["Hospitals"], Plan = tempQuote["Plan"], Smoker = tempQuote["Smoker"],
                        Status = true, StartDate = DateTime.UtcNow
                    };
                   await App.RealmDb.AddCustomer(customer);
                }
                else
                {
                    await notification.Notify("error", $"{registered}", "close");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
            await nav.PopToRootAsync();//go back to log in screen
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
            get => pass;
            set => SetProperty(ref pass, PassCheck(value));
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
    }
}