using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Android.Content.Res;
using Android.OS;
using Insurance_app.Communications;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Pages.ClientPages;
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms;
using Realms.Sync;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Exception = System.Exception;
using UserManager = Insurance_app.Logic.UserManager;

namespace Insurance_app.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LogInViewModel : ObservableObject
    {
        private string email=""; 
        private string password="";
        private UserManager userManager;

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

        private async Task ClientRegister()
        {
            CircularWaitDisplay = true;
            await Application.Current.MainPage.Navigation.PushAsync(new ClientRegistration());
            CircularWaitDisplay = false;
        }
        public async Task CheckIfUserExist()
        {
 
            SetUpWaitDisplay = true;
            try
            {
                if (App.RealmApp.CurrentUser != null)
                {
                    await App.RealmApp.CurrentUser.LogOutAsync();
                    RealmDb.GetInstance().Dispose();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            SetUpWaitDisplay = false;
        }

        private async Task NavigateToQuote()
        {
            try
            {
                CircularWaitDisplay = true;
                //await Shell.Current.GoToAsync($"//{nameof(QuotePage)}");
                await Application.Current.MainPage.Navigation.PushAsync(new QuotePage());
                CircularWaitDisplay = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task LogIn()
        {
            try
            {
                CircularWaitDisplay = true;
                if (App.NetConnection())
                {
                    if (!emailIsValid || !passIsValid )
                    {
                        throw new Exception("Log in details are not invalid");
                    }

                    var user = await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    if (user is null) throw new Exception("User fail log in");
                    
                    //await CleanDatabase();//TODO remove when submitting 
                    
                     var typeUser = await TypeUser(user);
                     if (typeUser.Equals($"{UserType.Customer}"))
                     {
                         Application.Current.MainPage = new AppShell();
                         await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                     }else if (typeUser.Equals($"{UserType.Client}"))
                     {
                         Application.Current.MainPage = new ClientShell();
                         await Shell.Current.GoToAsync($"//{nameof(CustomersPage)}");
                     }
                     else
                     {
                         await CheckIfUserExist();
                         throw new Exception("User has not been found");
                     }
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Notice", StaticOpt.NCE, "close");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Application.Current.MainPage.DisplayAlert("Login Failed", e.Message, "close");

            }
            CircularWaitDisplay = false;
        }
        private async Task<string> TypeUser(User user)
        {
            return await userManager.FindTypeUser(user);
        }
       

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

        private bool circularWaitDisplay;
        

        public bool CircularWaitDisplay
        {
            get => circularWaitDisplay;
            set => SetProperty(ref circularWaitDisplay, value);
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

        private async Task CleanDatabase()//TODO Remove when submitting
        {
            await RealmDb.GetInstance().CleanDatabase(App.RealmApp.CurrentUser);
        }
        

    }
}