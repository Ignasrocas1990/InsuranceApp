using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class ProfileViewModel:ObservableObject,IDisposable
    {
        private UserManager userManager;
        private string name="";
        private string lastName="";
        private string phoneNr="";
        private bool wait = false;
        private string addressText = "Click to update address";
        //--- address backing fields ---
        private int? houseN;
        private string postCode="";
        private string street="";
        private string county="";
        private string country="";
        private string city="";
        private Address address;
        private string customerId;

        public ICommand UpdateCommand { get; }
        public ICommand AddressCommand { get; }

        public ProfileViewModel()
        {
            userManager = new UserManager();
            AddressCommand = new AsyncCommand(UpdateAddress);
            UpdateCommand = new AsyncCommand(Update);
            
        }
        public async Task Setup()
        {
            try
            {
                var customer =  await userManager.GetCustomer(App.RealmApp.CurrentUser);
                if (customer !=null)
                {
                    NameDisplay = customer.Name;
                    LastNameDisplay = customer.LastName;
                    PhoneNrDisplay = customer.PhoneNr;
                    customerId = customer.Id;
                    
                    //address backing fields
                    houseN = customer.Address.HouseN;
                    street = customer.Address.Street;
                    city = customer.Address.City;
                    country = customer.Address.Country;
                    county = customer.Address.County;
                    postCode = customer.Address.PostCode;
                    address = new Address()
                    {
                        HouseN = houseN,
                        Street = street,
                        Country = country,
                        City = city,
                        County = county,
                        PostCode = postCode
                    };
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem in customer setup : {e}");
            }
        }
        private async Task UpdateAddress()
        {
            
           var newAddress = await Application.Current.MainPage.Navigation.ShowPopupAsync<Address>(new AddressPopup(new Address()
           {
               HouseN = houseN,
               City = city,
               Country = country,
               County = county,
               PostCode = postCode,
               Street = street
           }));
            if (newAddress!=null)
            {
                AddressDisplay = "Address saved";
                address = newAddress;
            }
        }
        public async Task Update()
        {
            var answer = await Shell.Current.CurrentPage.DisplayAlert(
                "Notice","You about to update details", "save", "cancel");
            if (!answer) return;
            
            //save to database
            try
            {
                CircularWaitDisplay = true;
                
               await userManager.updateCustomer(name,lastName,phoneNr,address, App.RealmApp.CurrentUser,customerId);
               //await App.RealmApp.EmailPasswordAuth.CallResetPasswordFunctionAsync(email, password); make it separate screen
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
            await Shell.Current.DisplayAlert("Message", "Details updated", "close");
        }

        public string NameDisplay
        {
            get => name;
            set => SetProperty(ref name, value);
        } 
        public string LastNameDisplay
        {
            get => lastName;
            set => SetProperty(ref lastName, value);
        } 
        public string PhoneNrDisplay
        {
            get => phoneNr;
            set => SetProperty(ref phoneNr, value);
        }

        public string AddressDisplay
        {
            get => addressText;
            set => SetProperty(ref addressText, value);
        }

        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        public void Dispose()
        {
            address = null;
            userManager.Dispose();
        }
    }
}