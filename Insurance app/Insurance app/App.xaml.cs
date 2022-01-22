﻿using System;
using System.Diagnostics;
using Insurance_app.Pages;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Insurance_app
{
    public partial class App : Application
    {
        private const string MyRealmAppId = "application-0-bvutx";
        public static RealmDb RealmDb;
        public static Realms.Sync.App RealmApp;
        public static bool Connected;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {

            RealmDb = new RealmDb();
            RealmApp = Realms.Sync.App.Create(MyRealmAppId);
            Connected=NetConnection();
            MainPage = new AppShell();

            if (RealmApp.CurrentUser is null)
            {
                Shell.Current.GoToAsync($"//{nameof(LogInPage)}");
            }
            else
            {
                Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
            Connectivity.ConnectivityChanged += (s,e) =>
            {
                Connected=NetConnection();
            };

           var sModel = ShellViewModel.GetInstance();
           sModel.AddViewModel(nameof(HomePageViewModel),new HomePageViewModel());
           sModel.AddViewModel(nameof(LogInViewModel),new LogInViewModel());
           sModel.AddViewModel(nameof(QuoteViewModel), new QuoteViewModel());
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
        public static bool NetConnection() => (Connectivity.NetworkAccess == NetworkAccess.Internet);
    }
    
}
