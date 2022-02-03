using System;
using System.Diagnostics;
using System.Linq;
using Insurance_app.Pages;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace Insurance_app
{
    public partial class App : Application
    {
        //public static RealmDb RealmDb;
        public static Realms.Sync.App RealmApp;
        public static bool Connected;

        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            Connected=NetConnection();

            
            //RealmDb = new RealmDb();
            RealmApp = Realms.Sync.App.Create(StaticOptions.MyRealmAppId);
            MainPage = new AppShell();
            if (!NetConnection() || RealmApp.CurrentUser is null)
            {
                Shell.Current.GoToAsync($"//{nameof(LogInPage)}");
            }
            else
            {
                Shell.Current.GoToAsync($"//{nameof(HomePage)}");
            }
/*
            if (RealmApp.CurrentUser is null)
            {
                Shell.Current.GoToAsync($"//{nameof(LogInPage)}");
            }
            else
            {
                RealmDb.user = RealmApp.CurrentUser;
            }
 */           
            


           //sModel.AddViewModel(nameof(LogInViewModel),new LogInViewModel());
           //sModel.AddViewModel(nameof(QuoteViewModel), new QuoteViewModel());
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        public static bool NetConnection()
        {
            var connection = Connectivity.NetworkAccess;
            var profiles = Connectivity.ConnectionProfiles;
            if (profiles.Contains(ConnectionProfile.WiFi) || profiles.Contains(ConnectionProfile.Cellular))
            {
                return (Connectivity.NetworkAccess == NetworkAccess.Internet);
            }

            return false;


        }
    }
    
}
