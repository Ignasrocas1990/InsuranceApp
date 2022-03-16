using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class ChangePassViewModel : ObservableObject,IDisposable
    {
        private string password;

        public ICommand ChangePassCommand { get; }
        private readonly UserManager userManager;
        
        public ChangePassViewModel()
        {
            ChangePassCommand = new AsyncCommand(ChangePassword);
            userManager = new UserManager();
        }
        
        private async Task ChangePassword()
        {
            try
            {
                CircularWaitDisplay = true;
                var customer = await userManager.GetCustomer(App.RealmApp.CurrentUser, App.RealmApp.CurrentUser.Id);
                if (customer != null)
                {
                    await App.RealmApp.EmailPasswordAuth
                        .CallResetPasswordFunctionAsync(customer.Email,password);
                }
                await Application.Current.MainPage.DisplayAlert(
                    Msg.Error, "Password changed successfully.", "close");                
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(
                    Msg.Error, "Password change failed.\nTry again later.", "close");
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;

        }
        public string PassDisplay
        {
            get => password;
            set => SetProperty(ref password, value);
        }
        private string password2;
        public string PassDisplay2
        {
            get => password2;
            set => SetProperty(ref password2, value);
        }
        private bool wait;
        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
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
    }
}