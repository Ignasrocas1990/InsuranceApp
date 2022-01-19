using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Android.OS;
using Insurance_app.BLE;
using Insurance_app.Models;
using Insurance_app.ViewModels;
using Realms;
using Realms.Sync;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Quote = Insurance_app.Models.Quote;

namespace Insurance_app
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage,IPageMethods
    {
        public LogInPage()
        {
            InitializeComponent();
            BindingContext = new LogInViewModel(this);
            
        }
        
/*
        private async void Login_Button_Clicked(object sender, EventArgs e)
        {
            await Login();
        }

        private async void GetQuote_Button_CLicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new QuotePage());

        }
        public async Task Login()
        {
            try
            {
                var user = await App.RealmApp.LogInAsync(
                    Credentials.EmailPassword(email, password));
                if (user != null)
                {
                    await Navigation.PushAsync(new MainPage());
                }
                else throw new Exception();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Login Failed", ex.Message, "close");
            }
        }
*/
        public async Task NavigateToMainPage()
        {
            await Navigation.PushAsync(new MainPage());
        }

        public async Task NavigateToQuotePage()
        {
            await Navigation.PushAsync(new QuotePage());
            //return Task.CompletedTask;
        }

        public async Task Notify(string title, string message, string button)
        {
           await DisplayAlert(title, message, button);
            //return Task.CompletedTask;
        }
    }
}