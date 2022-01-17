﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.OS;
using Insurance_app.BLE;
using Insurance_app.Models;
using Realms;
using Realms.Sync;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Quote = Insurance_app.Models.Quote;

namespace Insurance_app
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage
    {
        private string email;
        private string password;
        private bool newCustomer = false;
        private InferenceService inference;
        public LogInPage()
        {
            InitializeComponent();
            inference = new InferenceService();
        }

        private async void Login_Button_Clicked(object sender, EventArgs e)
        {
            await Login();
        }

        private async void GetQuote_Button_CLicked(object sender, EventArgs e)
        {
            
            await Navigation.PushAsync(new QuotePage());

        }
        

        private async Task Login()
        {
            try
            {
                var user = await App.RealmApp.LogInAsync(
                    Credentials.EmailPassword(email, password));
                if (user != null)
                {
                    if (newCustomer)
                    {
                        RealmDb db = new RealmDb();                     //TODO move
                        await db.AddCustomer(email, password, user.Id);
                        newCustomer = false;
                    }
                    await Navigation.PushAsync(new MainPage());
                }
                else throw new Exception();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Login Failed", ex.Message, "close");
            }
        }

        private async Task Register()
        {
            try
            {
                Console.WriteLine("customer registration");
                await App.RealmApp.EmailPasswordAuth.RegisterUserAsync(email, password);
                newCustomer = true;
                await Login();
            }
            catch (Exception e)
            {
                await DisplayAlert("Registration Failed", e.Message, "close");
            }
        }
        private void Email_Entry_Completed(object sender, TextChangedEventArgs e)
        {
            email = e.NewTextValue;
        }

        private void Password_Entry_Completed(object sender, TextChangedEventArgs e)
        {
            password = e.NewTextValue;
        }

        private void Button_OnClicked(object sender, EventArgs e)
        {
        }
    }
}