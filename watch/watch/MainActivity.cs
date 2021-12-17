using System;
using System.Collections.Concurrent;
using Android.App;
using Android.Widget;
using Android.OS;
using Android.Support.Wearable.Activity;
using Java.Lang;

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
            sensorManager = new SensorManager();
            int i = 0;
            pauseAcc.Click += (sender, args) => pauseAcc.Text = "click : " + Integer.ToString(i++);
            pauseGyro.Click += (sender, args) => pauseGyro.Text = "click : " + Integer.ToString(i++);

            bleServer = new BleServer(ApplicationContext,"");

        


            SubscribeToSensor(); //-------------call this to start monitor data
            
        }
        public void SubscribeToSensor()
        {
            sensorManager.AccEventHandler += (s, e) =>
            {
                //Console.WriteLine("Acc enqueued "+e.Full);
                bleServer.SensorData.Enqueue(e.Full);
            };
            sensorManager.GyroEventHandler += (s, e) =>
            {
                //Console.WriteLine("Gyro enqueued " + e.Full);
                bleServer.SensorData.Enqueue(e.Full);
            };
            bleServer.ToggleSensorsEventHandler += (s,e) =>
            {
                sensorManager.ToggleAcce();
                sensorManager.ToggleGyro();
            };
            if (bleServer.BltCallback != null)
            {
                bleServer.BltCallback.DisconectedHandler += (s, e) =>
                {
                    sensorManager.ToggleAcce();
                    sensorManager.ToggleGyro();
                    bleServer.IsConnected = false;
                };
            }
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
            sensorManager.UnsubscribeSensors();
        }
    }
}


