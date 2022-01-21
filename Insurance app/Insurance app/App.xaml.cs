using System;
using Insurance_app.Pages;
using Insurance_app.Pages.SidePageNavigation;
using Realms.Sync;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
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


            if (RealmApp.CurrentUser is null)
            {
                MainPage = new NavigationPage(new LogInPage());
            }
            else
            {
                MainPage = new NavigationPage(new FlyoutContainerPage());
            }
            Connectivity.ConnectivityChanged += (s,e) =>
            {
                Connected=NetConnection();
            };
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
