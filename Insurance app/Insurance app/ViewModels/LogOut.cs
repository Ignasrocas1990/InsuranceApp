using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.Util;
using Insurance_app.Pages;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class LogOut
    {
        private ICommand LogoutCommand { get; }

        public LogOut( )
        {
            LogoutCommand = new Command(Logout);
        }

        public async void Logout()
        {
            try
            {
                await Shell.Current.GoToAsync($"//{nameof(LogInPage)}");
                await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}