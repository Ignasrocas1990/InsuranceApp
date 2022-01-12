using System;
using Android;
using Android.App;
using Android.Content;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;

namespace watch
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);
            SetContentView(Resource.Layout.activity_main);

            SetAmbientEnabled();
            
            
            Intent intent = new Intent(this, typeof(SensorService));
            StartForegroundService(intent);

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


