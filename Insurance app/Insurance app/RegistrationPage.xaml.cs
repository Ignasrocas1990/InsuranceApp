using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.BLE;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage(string price,Dictionary<String,int> quote)
        {
            InitializeComponent();
            
        }
        // enter details etc...
        
        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            if (Check())
            {
                await Register();
                /*
                InferenceService inf = new InferenceService(); 
                CircularWait.IsVisible=true;
                var sent = await inf.Email(EmailEntry.Text); 
                CircularWait.IsVisible=false;
                if (sent != null)
                { 
                    await DisplayAlert("Registration", "Confirmation email sent,\n Please check your email", "close");
                }
                */
            }
            else
            {
                await DisplayAlert("Attention", "Inputs are wrong", "close");

            }


        }
        private async Task Register()
        {
            try
            {
                Console.WriteLine("customer registration");
                await App.RealmApp.EmailPasswordAuth.RegisterUserAsync(EmailEntry.Text, PassEntry.Text); 
                RealmDb db = new RealmDb();
                await db.AddCustomer(EmailEntry.Text, PassEntry.Text, App.RealmApp.CurrentUser.Id);
                await DisplayAlert("Notice", "Registration successful", "close");//change
                await Navigation.PushAsync(new LogInPage());

            }
            catch (Exception e)
            {
                await DisplayAlert("Registration Failed", e.Message, "close");//change
            }
        }

        private bool Check()
        {
            if (EmailEntry.Text.Length == 0) return false;
            if (PassEntry.Text.Length < 6) return false;
            return true;
        }
    }
}