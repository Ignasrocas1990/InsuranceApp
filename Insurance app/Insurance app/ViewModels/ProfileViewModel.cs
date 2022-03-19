using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    [QueryProperty(nameof(CustomerId), "CustomerId")]
    public class ProfileViewModel:ObservableObject,IDisposable
    {
        public readonly UserManager UserManager;
        private string name="";
        private string lastName="";
        private string phoneNr="";
        private bool wait = false;
        private string addressText = "Click to update address";

        private string email;
        //--- address backing fields ---
        private int? houseN;
        private string postCode="";
        private string street="";
        private string county="";
        private string country="";
        private string city="";
        private Address address;
        private string customerId="";
        public ICommand UpdateCommand { get; }
        public ICommand AddressCommand { get; }
        public ICommand ResetPasswordCommand { get; }


        public ProfileViewModel()
        {
            UserManager = new UserManager();
            AddressCommand = new AsyncCommand(UpdateAddress);
            UpdateCommand = new AsyncCommand(Update);
            ResetPasswordCommand = new AsyncCommand(ResetPassword);
        }
        public async Task Setup()
        {
           
            try
            {
                if(customerId.Equals(""))
                {
                    customerId = App.RealmApp.CurrentUser.Id;
                }
                else
                {
                    IsClientDisplay = true;
                }
                var customer = await UserManager.GetCustomer(App.RealmApp.CurrentUser, customerId);
                if (customer !=null)
                {
                    NameDisplay = customer.Name;
                    LastNameDisplay = customer.LastName;
                    PhoneNrDisplay = customer.PhoneNr;
                    customerId = customer.Id;
                    email = customer.Email;
                    
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

            SetUpWaitDisplay = false;
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
        private async Task Update()
        {
            var answer = await Shell.Current.CurrentPage.DisplayAlert(
                Msg.Notice,"You about to update details", "save", "cancel");
            if (!answer) return;
            
            //save to database
            try
            {
                CircularWaitDisplay = true;
                
               await UserManager.UpdateCustomer(name,lastName,phoneNr,address, App.RealmApp.CurrentUser,customerId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
            await Shell.Current.DisplayAlert(Msg.Notice, Msg.SuccessUpdateMsg, "close");
        }
        private async Task ResetPassword()
        {

            try
            {
                if (!App.NetConnection())
                {
                    await Msg.AlertError(Msg.NetworkConMsg);
                    return;
                }
            
                CircularWaitDisplay = true;
                await UserManager.ResetPassword(name, email);
                CircularWaitDisplay = false;
                await Msg.Alert(Msg.ResetPassMsg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Msg.AlertError("Password Reset Failed");
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
        public string CustomerId
        {
            get => customerId;
            set => customerId = value;

        }

        private bool isclient;
        public bool IsClientDisplay
        {
            get => isclient;
            set => SetProperty(ref isclient, value);
        }
        private bool setUpWait;
        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }

        public void Dispose()
        {
            address = null;
            UserManager.Dispose();
        }

      
    }
}