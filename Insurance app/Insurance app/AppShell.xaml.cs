using System;
using Insurance_app.Logic;
using Insurance_app.Pages;
using Xamarin.Forms;

namespace Insurance_app
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
        }
        private async void MenuItem_OnClicked(object sender, EventArgs e)
        {
            try
            {
                await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                await Current.GoToAsync($"//{nameof(LogInPage)}");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}