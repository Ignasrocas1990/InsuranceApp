using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class ProfileViewModel:ObservableObject,IDisposable
    {
        private UserManager userManager;
        private int age;
        private string name;
        private string lastName;
        private string phoneNr;
        private string email;
        private bool wait = false;
        private string addressText = "Click to update address";
        //--- address backing fields ---
        private int? HouseN;
        private string PostCode;
        private string Street;
        private string County;
        private string Country;
        private string City;
        private Address address;

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
                    if (customer.Age != null) age = (int) customer.Age;
                    NameDisplay = customer.Name;
                    LastNameDisplay = customer.LastName;
                    PhoneNrDisplay = customer.PhoneNr;
                    EmailDisplay = customer.Email;
                    
                    //address backing fields
                    HouseN = customer.Address.HouseN;
                    Street = customer.Address.Street;
                    City = customer.Address.City;
                    Country = customer.Address.Country;
                    County = customer.Address.County;
                    PostCode = customer.Address.PostCode;
                    address = new Address()
                    {
                        HouseN = HouseN,
                        Street = Street,
                        Country = Country,
                        City = City,
                        County = County,
                        PostCode = PostCode
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
               HouseN = HouseN,
               City = City,
               Country = Country,
               County = County,
               PostCode = PostCode,
               Street = Street
           }));
            if (newAddress!=null)
            {
                AddressDisplay = "Address saved";
                address = newAddress;
            }
        }
        private async Task Update()
        {
            //save to database
            try
            {
                var errors = ""; //StaticOptions.IsValid(name, lastName, phoneNr, email);//TODO uncomment for submission
                if (errors.Length > 2)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Error", errors, "close");
                    return;
                }

                IsEnabled = false;
                CircularWaitDisplay = true;
                
               
               await userManager.updateCustomer(age,name,lastName,phoneNr,email,address, App.RealmApp.CurrentUser);
               //await App.RealmApp.EmailPasswordAuth.CallResetPasswordFunctionAsync(email, password); make it separate screen
               
               CircularWaitDisplay = false;
               IsEnabled = true;

            }
            catch (Exception e)
            {
                CircularWaitDisplay = false;
                IsEnabled = true;
                Console.WriteLine(e);
            }
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
        public string EmailDisplay
        {
            get => email;
            set => SetProperty(ref email, value);
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
        
        private bool enabled = true;
        public bool IsEnabled
        {
            get => enabled;
            set => SetProperty(ref enabled, value);
        }

        public void Dispose()
        {
            address = null;
            userManager.Dispose();
        }
    }
}