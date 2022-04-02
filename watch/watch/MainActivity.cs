/*
    Copyright 2020,Ignas Rocas

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

using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.Wearable.Activity;
using Android.Views;
using Android.Widget;
using watch.Services;

namespace watch
{
    /// <summary>
    /// Its only used to start the service... That is it.
    /// </summary>
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        private Button btn;
        private Intent intent;
        private static MainActivity _instance;
        public static PowerManager.WakeLock mWakeLock;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            SetContentView(Resource.Layout.activity_main);
            SetAmbientEnabled();


            if (Window != null)
                Window.AddFlags(WindowManagerFlags.KeepScreenOn | WindowManagerFlags.DismissKeyguard |
                                WindowManagerFlags.ShowWhenLocked | WindowManagerFlags.TurnScreenOn);


            intent = new Intent(this, typeof(WatchService));
            btn = FindViewById<Button>(Resource.Id.closeApp);
            btn.Click += (s,e) =>
            {
                StopService(intent);
                this.OnDestroy();
                Finish();
            };
            PowerManager pm = (PowerManager)GetSystemService(PowerService);
            mWakeLock = pm.NewWakeLock(WakeLockFlags.Full, "systemService");
            StartForegroundService(intent);
            _instance = this;
        }

        public static void Fin()
        {
            _instance.Finish();
        } 


        // Android overridden methods ---------------------------------------------------------------------
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
        }
    }
}


