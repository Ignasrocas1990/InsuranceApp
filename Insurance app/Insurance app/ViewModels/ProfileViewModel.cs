/*   Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    /// <summary>
    /// Class used to store and manipulate ProfilePage UI components in real time via BindingContext and its properties
    /// </summary>
    [QueryProperty(nameof(TransferredCustomerId), "TransferredCustomerId")]
    public class ProfileViewModel:ObservableObject,IDisposable
    {
        private readonly UserManager userManager;
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
            userManager = new UserManager();
            AddressCommand = new AsyncCommand(UpdateAddress);
            UpdateCommand = new AsyncCommand(Update);
            ResetPasswordCommand = new AsyncCommand(ResetPassword);
        }
        /// <summary>
        /// Loads in data using manager classes via database and set it to Bindable properties(UI)
        /// </summary>
        public async Task Setup()
        {
            try
            {
                if(TransferredCustomerId.Equals(""))
                {
                    customerId = App.RealmApp.CurrentUser.Id;
                }
                else
                {
                    customerId = TransferredCustomerId;
                    IsClientDisplay = true;
                }
                var customer = await userManager.GetCustomer(App.RealmApp.CurrentUser, customerId);
                if (customer !=null)
                {
                    NameDisplay = customer.Name;
                    LastNameDisplay = customer.LastName;
                    PhoneNrDisplay = customer.PhoneNr;
                    //customerId = customer.Id;
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
        
        /// <summary>
        /// displays address pop up with a existing address
        /// and updates the old address backing field to new address
        /// </summary>
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
        /// <summary>
        /// Updates user details via UserManager class
        /// </summary>
        private async Task Update()
        {
            var answer = await Shell.Current.CurrentPage.DisplayAlert(
                Msg.Notice,"You about to update details", "save", "cancel");
            if (!answer) return;
            
            //save to database
            try
            {
                CircularWaitDisplay = true;
                await userManager.UpdateCustomer(name,lastName,phoneNr,address, App.RealmApp.CurrentUser,customerId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
            await Msg.Alert(Msg.SuccessUpdateMsg);
        }
        
        /// <summary>
        /// Resets password to a random password generated via userManager
        /// </summary>
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
                await userManager.ResetPassword(name, email);
                CircularWaitDisplay = false;
                await Msg.Alert(Msg.ResetPassMsg);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Msg.AlertError("Password Reset Failed");
            }
        }
// --------- Bindable properties below --------------------------
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

        private string transferredId="";
        public string TransferredCustomerId
        {
            get => transferredId;
            set => transferredId = value;

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
            userManager.Dispose();
        }

      
    }
}