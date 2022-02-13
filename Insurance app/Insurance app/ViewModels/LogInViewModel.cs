using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.OS;
using Insurance_app.Pages;
using Insurance_app.Service;
using Realms;
using Realms.Sync;
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
        public ICommand LogInCommand { get; }
        public ICommand QuoteCommand { get; }

        public LogInViewModel()
        {
            checkIfUserExist();
            LogInCommand = new AsyncCommand(LogIn);
            QuoteCommand = new AsyncCommand(NavigateToQuote);
            Connectivity.ConnectivityChanged += (s, e) =>
            {
                App.Connected =  (e.NetworkAccess ==  NetworkAccess.Internet);
            };
            
        }

        private async void checkIfUserExist()
        {
            try
            {
                if (App.RealmApp.CurrentUser == null) return;
                await App.RealmApp.CurrentUser.LogOutAsync();
                RealmDb.GetInstance().Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task NavigateToQuote()
        {
            try
            {
                await Shell.Current.GoToAsync($"//{nameof(QuotePage)}");
                //var quotePage = new QuotePage();
                //await Nav.PushAsync(quotePage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            //return Task.CompletedTask;
        }

        private async Task LogIn()
        { 
            
            try
            {
                CircularWaitDisplay = true;
                if (App.NetConnection())
                {
                    
                    await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    // await CleanDatabase();//TODO remove when submitting
                    
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
                }
                else
                {
                   await Shell.Current.DisplayAlert("Notice", "Network Connection needed for log in", "close");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Shell.Current.DisplayAlert("Login Failed", e.Message, "close");

            }
            CircularWaitDisplay = false;
            
            //return Task.CompletedTask;
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

        private async Task CleanDatabase()//TODO Remove when submitting
        {
            await RealmDb.GetInstance().CleanDatabase(App.RealmApp.CurrentUser);
        }

    }
}