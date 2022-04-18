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
using Insurance_app.Pages;
using Insurance_app.Pages.ClientPages;
using Insurance_app.SupportClasses;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.ViewModels
{
    /// <summary>
    /// Class used to store and manipulate LogInPage UI components in real time via BindingContext and its properties
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LogInViewModel : ObservableObject
    {
        private string email="";
        private string password = "";
        private const string ExpiredCustomerStr = "Accounts policy has been expired" +
                                            "\nPlease select one of the following options.";

        private const string CreateNewAccStr = "Create new account.";
        private const string ResetStr = "Reset the old account.";
        
        private readonly UserManager userManager;

        public ICommand LogInCommand { get; }
        public ICommand QuoteCommand { get; }
        public ICommand ClientRegCommand { get; }


        public LogInViewModel()
        {
            LogInCommand = new AsyncCommand(LogIn);
            QuoteCommand = new AsyncCommand(NavigateToQuote);
            ClientRegCommand = new AsyncCommand(ClientRegister);
            Connectivity.ConnectivityChanged += (s, e) =>
            {
                App.Connected =  (e.NetworkAccess ==  NetworkAccess.Internet);
            };
            userManager = new UserManager();
        }
        /// <summary>
        /// Transfers to Client Registration page
        /// </summary>
        private async Task ClientRegister()
        {
            CircularWaitDisplay = true;
            await Application.Current.MainPage.Navigation.PushAsync(new ClientRegistration());
            CircularWaitDisplay = false;
        }
        /// <summary>
        /// logs outs current user.
        /// </summary>
        public async Task ExistUser()
        {
            try
            {
                if (App.RealmApp.CurrentUser != null)
                {
                    await App.RealmApp.CurrentUser.LogOutAsync();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Navigate to QuotePage
        /// </summary>
        private async Task NavigateToQuote()
        {
            try
            {
                CircularWaitDisplay = true;
                await Application.Current.MainPage.Navigation.PushAsync(new QuotePage(""));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
        }
        /// <summary>
        /// Check which type of user it is via manager and
        /// sets navigation for that particular type while
        /// checking if the credentials are valid
        /// </summary>
        private async Task LogIn()
        {
            try
            {
                if (!App.NetConnection())
                { 
                    await Msg.AlertError(Msg.NetworkConMsg);
                   return;
                }

                
                CircularWaitDisplay = true;

                if (!emailIsValid || !passIsValid )
                {
                    throw new Exception("Log in details are not invalid");
                }

                var user = await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    
                await CleanDatabase();//TODO remove when submitting -------------------------------------------------##############
                    
                var typeUser = await userManager.FindTypeUser(user);
                if (typeUser.Equals($"{UserType.Customer}"))
                {
                    Application.Current.MainPage = new AppShell();
                    string route = $"//{nameof(HomePage)}?Email={email}&Pass={password}";
                    await Shell.Current.GoToAsync(route,true);
                }
                else if (typeUser.Equals($"{UserType.Client}"))
                {
                    Application.Current.MainPage = new ClientShell();
                    await Shell.Current.GoToAsync($"//{nameof(ClientMainPage)}",true);
                }
                else if (typeUser.Equals($"{UserType.UnpaidCustomer}"))
                {
                    await Msg.Alert( "Seems like you haven't payed yet.\nDirecting to payment page...");
                    await Application.Current.MainPage.Navigation.PushAsync(new PaymentPage(null));
                }
                else if (typeUser.Equals($"{UserType.ExpiredCustomer}"))
                {
                    await Msg.Alert( "Seems like your policy has expired.\nDirecting to payment page...");
                    await Application.Current.MainPage.Navigation.PushAsync(new PaymentPage(null));
                }
                else if(typeUser.Equals(""))
                {
                    await ExistUser();
                }
                else
                {
                    var answer = await Application.Current.MainPage.DisplayAlert(Msg.Notice, ExpiredCustomerStr,
                        CreateNewAccStr,ResetStr );
                    if (answer)
                    {
                        await ExistUser();
                        QuoteCommand.Execute(null);
                    }
                    else
                    {
                        await Application.Current.MainPage.Navigation.PushAsync(new QuotePage(typeUser),true);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Msg.AlertError("Invalid Credentials");
            }
            CircularWaitDisplay = false;
        }
//-------------------- Bindable properties below -------------------

        public string EmailDisplay
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        public string PasswordDisplay
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private bool circularWait;
        

        public bool CircularWaitDisplay
        {
            get => circularWait;
            set => SetProperty(ref circularWait, value);
        }

        private bool passIsValid;
        public bool PassIsValid
        {
            get => passIsValid;
            set => SetProperty(ref passIsValid, value);
        }
        private bool emailIsValid;

        public bool EmailIsValid
        {
            get => emailIsValid;
            set => SetProperty(ref emailIsValid, value);
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
        private async Task CleanDatabase()//TODO Remove when submitting
        {
            //await userManager.CleanDatabase(App.RealmApp.CurrentUser);
        }
        

    }
}