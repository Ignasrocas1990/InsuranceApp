using System;
using Android;
using Android.App;
using Android.Content;
using Android.Hardware;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;
using watch.Services;

namespace watch
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        private Button btn;
        private Intent intent;
        public static MainActivity Instance;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            SetContentView(Resource.Layout.activity_main);

            SetAmbientEnabled();
            
            btn = FindViewById<Button>(Resource.Id.closeApp);
            btn.Click += (s,e) =>
            {
                StopService(intent);
                this.OnDestroy();
                Finish();
            };
            intent = new Intent(this, typeof(WatchService));
            StartForegroundService(intent);
            Instance = this;
        }

        public static void Fin()
        {
            Instance.Finish();
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


