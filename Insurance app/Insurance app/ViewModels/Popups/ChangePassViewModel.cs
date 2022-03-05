using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Pages.Popups;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    public class ChangePassViewModel : ObservableObject
    {
        private readonly ChangePassPopup popup;
        private string password;
        private string email;

        public ICommand ChangePassCommand { get; }
        public ICommand CloseCommand { get; }



        public ChangePassViewModel(ChangePassPopup popup,string email)
        {
            this.popup = popup;
            this.email = email;
            ChangePassCommand = new AsyncCommand(ChangePassword);
            CloseCommand = new Command(Close);
        }

        private void Close()
        {
            popup.Dismiss(false);
        }

        private async Task ChangePassword()
        {
            try
            {
                CircularWaitDisplay = true;
                await App.RealmApp.EmailPasswordAuth
                    .CallResetPasswordFunctionAsync(email,password);
                CircularWaitDisplay = false;
                popup.Dismiss(true);
            }
            catch (Exception e)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error", "Password change failed.\nTry again later.", "close");
                Console.WriteLine(e);
            }
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
    }
}