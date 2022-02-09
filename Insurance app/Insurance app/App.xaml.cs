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
        }

        protected override void OnStart()
        {
            Connected=NetConnection();

            
            //RealmDb = new RealmDb();
            RealmApp = Realms.Sync.App.Create(StaticOptions.MyRealmAppId);
            
            //firstly check here if user is customer or client at welcome screen
            MainPage = new AppShell();//there should be 2 type of AppShells... (customer/client)
            if (NetConnection() && RealmApp.CurrentUser != null)
            {
                 Shell.Current.GoToAsync($"//{nameof(HomePage)}");// for simplicity just make them log in everytime
            }
            else
            {
                Shell.Current.GoToAsync($"//{nameof(LogInPage)}");
                //also dont use shell till we know if it is a customer/client so use previous navigation(gihub)
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
            var connectionProfiles = profiles as ConnectionProfile[] ?? profiles.ToArray();
            if (connectionProfiles.Contains(ConnectionProfile.WiFi) || connectionProfiles.Contains(ConnectionProfile.Cellular))
            {
                return (Connectivity.NetworkAccess == NetworkAccess.Internet);
            }

            return false;


        }
       
    }
    
}
