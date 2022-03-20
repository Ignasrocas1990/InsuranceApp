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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms.Sync;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{    
    public class RegistrationViewModel : ObservableObject,IDisposable
    {
        private readonly Dictionary<string, string> quote;
        public readonly string AddressSText = "Address saved";
        public ICommand ConfirmEmailCommand { get; }
        private bool wait;
        private string price="";
        private string email="";
        private string password="";
        private string fName="";
        private string lName="";
        private string phoneNr="";
        private string code = "";
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
            ConfirmEmailCommand = new AsyncCommand(ConfirmEmail);
            this.price = price;
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
                   var priceFloat = StaticOpt.GetPrice(price);
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
        private async Task ConfirmEmail()
        {
            try
            {
                if (!App.NetConnection())
                {
                    await Msg.AlertError(Msg.NetworkConMsg);
                    return;
                }
                if (code == "")
                {
                    code = StaticOpt.TempPassGenerator(6,false);
                    HttpService.EmailConfirm(email, DateTime.Now.Date, code);
                }
                var result = await Application.Current
                    .MainPage.DisplayPromptAsync(
                        "Email Confirmation", "Please enter email confirmation code",
                        "submit", "cancel", "check your email");
                if (result != code)
                {
                    var answer = await Application.Current.MainPage
                        .DisplayAlert("Email code", "Email Code is invalid", "Try again later", "Resend new Code");
                    if (!answer)
                    {
                        code = "";
                        ConfirmEmailCommand.Execute(null);
                    }
                }
                else
                {
                    EmailNotConfirmedDisplay = false;
                    EmailConfirmedDisplay = true;
                    await Msg.Alert("Email has been confirmed successfully");
                }
            }
            catch (Exception e)
            {
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
        private bool emailConfirmed;
        public bool EmailConfirmedDisplay
        {
            get => emailConfirmed;
            set => SetProperty(ref emailConfirmed, value);
        }
        private bool setUpWait;
        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }

        private bool notConfirmed;
        public bool EmailNotConfirmedDisplay
        {
            get => notConfirmed;
            set => SetProperty(ref notConfirmed, value);
        }

        public void Dispose()
        {
            userManager.Dispose();
        }
    }
}