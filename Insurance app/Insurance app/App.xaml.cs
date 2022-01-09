using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app
{
    public partial class App : Application
    {
        private const string MyRealmAppId = "application-0-bvutx";
        public static Realms.Sync.App RealmApp;
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStart()
        {
            RealmApp = Realms.Sync.App.Create(MyRealmAppId);
            if (RealmApp.CurrentUser is null)
            {
                MainPage = new NavigationPage(new LogInPage());
            }
            else
            {
                MainPage = new NavigationPage(new MainPage());
            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
