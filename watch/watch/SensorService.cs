using System;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using watch.Ble;
using watch.Sensors;

namespace watch
{
    [Service]
    public class SensorService : Service
    {
        private const string TAG = "mono-stdout";
        const int ServiceRunningNotificationId = 10000;
        const int MaxDisconnectionTime = 300;//5min
        private const int ElapsedTime = 1000;//1sec
        
        private int curDisconnectCounter = 0;
        private BleServer bleServer;
        private SensorManager sensorManager;
        private Timer timer;

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            StartForegroundNotification();
             bleServer =  new BleServer(this);
             sensorManager = new SensorManager();
             SubscribeToSensor();
             timer = new Timer(ElapsedTime);
             timer.Elapsed += DisconnectedCheck;
             return StartCommandResult.Sticky;
        }
        

        /**
         * Subscribe to sensor and Server
         * to communicate in between
         */
        private void SubscribeToSensor()
        {
            // sensor data transfer to BLE server
            sensorManager.AccEventHandler += (s, e) =>
            {
                bleServer.SensorData.Enqueue(e.Data);
            };

            //Server communications
            if (bleServer.BltCallback != null)
            {
                bleServer.BltCallback.StateHandler += (s, e) =>
                {
                    if (e.State.Equals("Disconnected"))
                    {
                        timer.Start();
                    }
                    else
                    {
                        timer.Stop();
                        curDisconnectCounter = 0;
                    }
                    sensorManager.ToggleSensors(e.State);
                    Log.Verbose(TAG, $" is monitoring ? : {sensorManager.isM()}");
                    
                };
            }
            
        }
        private void DisconnectedCheck(object sender, ElapsedEventArgs e)
        {
            curDisconnectCounter += 1;
            if (curDisconnectCounter == MaxDisconnectionTime)
            {
                OnDestroy();
            }
        }

        // -------------------------------------- Notification methods ---------------------------------------------------------
        private void StartForegroundNotification()
        {
            CreateNotificationChannel();
            try
            {
                var notification = new Notification.Builder(this,"999")
                    .SetContentTitle("a")
                    ?.SetContentText("b")
                    ?.SetOngoing(true)
                    ?.Build();
                StartForeground(ServiceRunningNotificationId, notification);
            }
            catch (Exception e)
            {
                Log.Verbose(TAG,e.Message);
                throw;
            }
            
        }

        
        private void CreateNotificationChannel() {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) {
                // Notification channels are new in API 26 (and not a part of the
                // support library). There is no need to create a notification
                // channel on older versions of Android.
                return;
            }

            var channel = new NotificationChannel("999", "channelName", NotificationImportance.Default) {
                Description = "channelDescription"
            };

            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            if (notificationManager != null) notificationManager.CreateNotificationChannel(channel);
        }


        public override IBinder OnBind(Intent intent)
        {
            return null;
        }

        public override void OnDestroy()
        {
            bleServer.StopAdvertising();
            sensorManager.ToggleSensors("off");
            sensorManager.UnsubscribeSensors();
            bleServer.Dispose();
            timer.Dispose();
            StopForeground(true);
            StopSelf();
        }
    }
    
}