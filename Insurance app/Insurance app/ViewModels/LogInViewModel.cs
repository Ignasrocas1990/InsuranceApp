﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Android.OS;
using Insurance_app.Communications;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Pages.ClientPages;
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms;
using Realms.Sync;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Exception = System.Exception;

namespace Insurance_app.ViewModels
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public class LogInViewModel : ObservableObject
    {
        private string email=""; 
        private string password="";

        public ICommand LogInCommand { get; }
        public ICommand QuoteCommand { get; }
        public ICommand ClientRegCommand { get; }


        public LogInViewModel()
        {
            LogInCommand = new AsyncCommand(LogIn);
            QuoteCommand = new AsyncCommand(NavigateToQuote);
            ClientRegCommand = new AsyncCommand(ClientRegister);
            Connectivity.ConnectivityChanged += (s, e) =>
            {
                App.Connected =  (e.NetworkAccess ==  NetworkAccess.Internet);
            };
        }

        private async Task ClientRegister()
        {
            CircularWaitDisplay = true;
            await Application.Current.MainPage.Navigation.PushAsync(new ClientRegistration());
            CircularWaitDisplay = false;
        }
        public async Task CheckIfUserExist()
        {
            CircularWaitDisplay = true;
            try
            {
                if (App.RealmApp.CurrentUser != null)
                {
                    await App.RealmApp.CurrentUser.LogOutAsync();
                    RealmDb.GetInstance().Dispose();
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            CircularWaitDisplay = false;
        }

        private async Task NavigateToQuote()
        {
            try
            {
                //await Shell.Current.GoToAsync($"//{nameof(QuotePage)}");
                await Application.Current.MainPage.Navigation.PushAsync(new QuotePage());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task LogIn()
        {
            try
            {
                CircularWaitDisplay = true;
                if (App.NetConnection())
                {
                    if (email.Length < 1 || password.Length < 6 )
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Email or Password fields left blank",
                            "close");
                        return;
                    }
                    await App.RealmApp.LogInAsync(Credentials.EmailPassword(email, password));
                    //await CleanDatabase();//TODO remove when submitting 
                    //return;
                    //find customer if no customer found, find client. (What ever is found will set Shell)
                    //customer found below
                    Application.Current.MainPage = new AppShell();
                    await Shell.Current.GoToAsync($"//{nameof(HomePage)}");

                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Notice", StaticOpt.NCE, "close");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await Shell.Current.DisplayAlert("Login Failed", e.Message, "close");

            }
            CircularWaitDisplay = false;

        }
       

        public string EmailDisplay
        {
            get => email;
            set => SetProperty(ref email, value);
        }
        public string PasswordDisplay
        {
            get => password;
            set => SetProperty(ref password, value);
        }

        private bool circularWaitDisplay;

        public bool CircularWaitDisplay
        {
            get => circularWaitDisplay;
            set => SetProperty(ref circularWaitDisplay, value);
        }

        private async Task CleanDatabase()//TODO Remove when submitting
        {
            await RealmDb.GetInstance().CleanDatabase(App.RealmApp.CurrentUser);
        }
        

    }
}