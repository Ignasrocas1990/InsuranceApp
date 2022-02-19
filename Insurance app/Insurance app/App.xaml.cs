using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
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
            // await Application.Current.MainPage.Navigation.PushAsync(new QuotePage());

        }

        protected override void OnStart()
        {
            Connected=NetConnection();
            try
            {
                RealmApp = Realms.Sync.App.Create(StaticOpt.MyRealmAppId);
                MainPage = new NavigationPage(new LogInPage());
            }
            catch (Exception e)
            {
                MainPage.DisplayAlert("error", "Application server is down\nPlease try again later","close");
            }
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
            var connectionProfiles = profiles as ConnectionProfile[] ?? profiles.ToArray();
            if (connectionProfiles.Contains(ConnectionProfile.WiFi) || connectionProfiles.Contains(ConnectionProfile.Cellular))
            {
                return (Connectivity.NetworkAccess == NetworkAccess.Internet);
            }

            return false;


        }
       
    }
    
}
