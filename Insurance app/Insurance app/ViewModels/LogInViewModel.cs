using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Pages;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Exception = System.Exception;

namespace Insurance_app.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LogInViewModel : ObservableObject
    {
        private string email; 
        private string password;
        public ICommand LogInCommand { get; }
        public ICommand QuoteCommand { get; }

        public LogInViewModel()
        {
            
            LogInCommand = new AsyncCommand(LogIn);
            QuoteCommand = new AsyncCommand(NavigateToQuote);


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
                await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                await Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Application.Current.MainPage.DisplayAlert("Login Failed", e.Message, "close");

            }

            
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
        

    }
}