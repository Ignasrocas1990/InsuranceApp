/*   Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using System.Linq;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace Insurance_app
{
    /// <summary>
    /// checks the network connection
    /// & passes the the main page to Log in page
    /// </summary>
    public partial class App : Application
    {
        public static Realms.Sync.App RealmApp;
        public static bool Connected;
        private bool disconnected;
        public static bool WasPaused { get; set; }
        

        public App()
        {
            InitializeComponent();
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

            Connectivity.ConnectivityChanged += (s, e) =>
            {
                if (disconnected && NetConnection())
                {
                    disconnected = false;
                }
            };
            
        }
        /// <summary>
        /// Checks if device has wifi/cellular internet connection
        /// </summary>
        /// <returns>boolean value true when internet connection found</returns>
        public static bool NetConnection()
        {
            var profiles = Connectivity.ConnectionProfiles;
            var connectionProfiles = profiles as ConnectionProfile[] ?? profiles.ToArray();
            if (connectionProfiles.Contains(ConnectionProfile.WiFi) || connectionProfiles.Contains(ConnectionProfile.Cellular))
            {
                return (Connectivity.NetworkAccess == NetworkAccess.Internet);
            }

            return false;
        }
        protected override void OnSleep() { }
        protected override void OnResume() { }
    }
    
}
