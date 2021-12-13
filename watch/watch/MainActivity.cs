using System;

using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;

namespace watch
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        //static event EventHandler<AccArgs> accEventHandler;
        int count = 1;
        private BleServer bleServer;
        private SensorManager sensorManager;
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);

            SetContentView(Resource.Layout.activity_main);

            SetAmbientEnabled();

            Button pauseAcc = FindViewById<Button>(Resource.Id.pauseBtnAcc);
            Button pauseGyro = FindViewById<Button>(Resource.Id.pauseBtnGyro);


            pauseAcc.Click += delegate { sensorManager.ToggleAcce(); };
            pauseGyro.Click += delegate { sensorManager.ToggleGyro(); };


            bleServer = new BleServer(ApplicationContext,"");
            sensorManager = new SensorManager();



            subscribeToSensor();
        }
        public void subscribeToSensor()
        {
            sensorManager.accEventHandler += (s, e) =>
            {
                Console.WriteLine("Value is ---------->"+e.full);
                bleServer.dataToSend = e.full;
                //Console.WriteLine($"X is : {e.x} Y is {e.y} Z is {e.z}");
            };
            sensorManager.gyroEventHandler += (s, e) =>
            {
                Console.WriteLine("Value is ---------->" + e.full);
                bleServer.dataToSend = e.full;
                //Console.WriteLine($"X is : {e.x} Y is {e.y} Z is {e.z}");
            };
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
        protected override void OnDestroy()
        {
            base.OnDestroy();
            sensorManager.unsubscribeSensors();
        }
    }
}


