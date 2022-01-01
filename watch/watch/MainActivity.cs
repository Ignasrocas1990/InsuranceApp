using System;
using System.Collections.Concurrent;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;
using Java.Lang;
using Thread = System.Threading.Thread;

namespace watch
{
    [Activity(Label = "@string/app_name", MainLauncher = true)]
    public class MainActivity : WearableActivity
    {
        //static event EventHandler<AccArgs> accEventHandler;
        private BleServer bleServer;
        private SensorManager sensorManager;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            Xamarin.Essentials.Platform.Init(this, bundle);

            SetContentView(Resource.Layout.activity_main);

            SetAmbientEnabled();
            
            sensorManager = new SensorManager();
            int i = 0;

            bleServer = new BleServer(ApplicationContext);
            SubscribeToSensor(); //-------------call this to start monitor data
            
        }
        public void SubscribeToSensor()
        {
            bleServer.BltCallback.dataWriteHandler += (s, e) =>
            {
                sensorManager.ToggleSensors();
                sensorManager.UnsubscribeSensors();
                
            };
            
             sensorManager.AccEventHandler += (s, e) =>
            {
                bleServer.SensorData.Enqueue(e.Data);
                Console.WriteLine($"enqued:{e.Data}");

            };
            
            sensorManager.GyroEventHandler += (s, e) =>
            {
                bleServer.SensorData.Enqueue(e.Data);
                
            };
            bleServer.ToggleSensorsEventHandler += (s,e) =>
            {
                sensorManager.ToggleSensors();
            };
            if (bleServer.BltCallback != null)
            {
                bleServer.BltCallback.DisconectedHandler += (s, e) =>
                {
                    sensorManager.ToggleSensors();
                };
            }
        }
        // Android overridden methods ---------------------------------------------------------------------
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnStop()
        {
            base.OnStop();
            Console.WriteLine($"onStop : is monitoring : {sensorManager.isM()}");
        }

        protected override void OnPause()
        {
            base.OnPause();
            Console.WriteLine($"onPause : is monitoring : {sensorManager.isM()}");

        }

        protected override void OnDestroy()
        {
            Console.WriteLine("unsubscribed");
            sensorManager.UnsubscribeSensors();
            bleServer.StopAdvertising();
            base.OnDestroy();

        }
    }
}


