using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Java.Lang;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LogInViewModel : ObservableObject
    {
        string Email; 
        string Password;
        public ICommand LogInCommand { get; }
        public ICommand QuoteCommand { get; }

        private IPageMethods page;
        public LogInViewModel() { }
        public LogInViewModel(IPageMethods page)
        {
            this.page = page;
            LogInCommand = new AsyncCommand(LogIn);
            QuoteCommand = new AsyncCommand(page.NavigateToQuotePage);
        }



        private async Task LogIn()
        { 
            var user = await App.RealmDb.Login(Email, Password);
            if (user is null)
            {
                await page.Notify("Login Failed", "ex.Message", "close");
                return;
            }

            await page.NavigateToMainPage();
        }

        public string EmailDisplay
        {
            get => Email;
            set => SetProperty(ref value, Email);
        }
        public string PasswordDisplay
        {
            get => Password;
            set => SetProperty(ref value, Password);
        }
        

    }
}