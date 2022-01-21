using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Pages;
using Insurance_app.Pages.SidePageNavigation;
using Java.Lang;
using Realms.Exceptions;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LogInViewModel : ObservableObject
    {
        private string email; 
        private string password;
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
            
            try
            {
                await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                var mainPage = new FlyoutContainerPage();
                await Nav.PushModalAsync(mainPage);
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e);
                await notification.Notify("Login Failed", e.Message, "close");

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