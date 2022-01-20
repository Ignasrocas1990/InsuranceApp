using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Pages;
using Java.Lang;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LogInViewModel : ObservableObject
    {
        private string Email=""; 
        private string Password="";
        public ICommand LogInCommand { get; }
        public ICommand QuoteCommand { get; }

        private INotification notification;
        private INavigation Nav; 
        public LogInViewModel() { }
        public LogInViewModel(INotification notification,INavigation nav)
        {
            this.notification = notification;
            this.Nav = nav;
            LogInCommand = new AsyncCommand(LogIn);
            QuoteCommand = new AsyncCommand(NavigateToQuote);
        }

        private async Task NavigateToQuote()
        {
            await Nav.PushAsync(new QuotePage());
            //return Task.CompletedTask;
        }

        private async Task LogIn()
        { 
            var user = await App.RealmDb.Login(Email, Password);
            if (user is null)
            {
                await notification.Notify("Login Failed", "ex.Message", "close");
                return;
            }
            var mainPage = new FlyoutPage1();
            await Nav.PushAsync(mainPage);
            
            //return Task.CompletedTask;
        }

        public string EmailDisplay
        {
            get => Email;
            set => SetProperty(ref Email, value);
        }
        public string PasswordDisplay
        {
            get => Password;
            set => SetProperty(ref Password, value);
        }
        

    }
}