using System;
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
        private void MenuItem_OnClicked(object sender, EventArgs e)
        {
            try
            {
                Current.GoToAsync($"//{nameof(LogInPage)}");
                App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}