using System;
using System.Threading.Tasks;
using Android.Util;
using Insurance_app.Pages;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class LogOut
    {
        private INavigation nav;
        public LogOut(INavigation nav)
        {
            this.nav = nav;
        }

        public async void Logout()
        {
            try
            {
                await nav.PushModalAsync(new LogInPage(),false);//go back to log in screen
                await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
    }
}