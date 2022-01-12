using System;
using System.Threading.Tasks;
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
        const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        private const string TAG = "mono-stdout";
        private BleServer bleServer;
        private SensorManager sensorManager;
        private int stepCount = 0;
        public static EventHandler temp = delegate{};

        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
            StartForegroundNotification();
             bleServer =  new BleServer(this);
             sensorManager = new SensorManager();
             SubscribeToSensor();

            
            
            return StartCommandResult.Sticky;
        }

        /**
         * Subscribe to sensor and Server
         * to communicate in between
         */
        private void SubscribeToSensor()
        {
            
            sensorManager.stepDetector.stepCounted += (s,e) =>
            {
                Log.Verbose(TAG, "counted a step");
                bleServer.SensorData.Enqueue(1);
            };
            

            //Server communications
            if (bleServer.BltCallback != null)
            {
                bleServer.BltCallback.StateHandler += (s, e) => //change this to 2 handlers connected & disconnected
                {
                    sensorManager.ToggleSensors(e.State);
                    Log.Verbose(TAG, $" is monitoring ? : {sensorManager.isM()}");
                };
                bleServer.BltCallback.DataWriteHandler += (s, e) =>
                {
                    Log.Verbose(TAG, $" unsubscribing from sensors");
                    bleServer.StopAdvertising();
                    sensorManager.ToggleSensors("off");
                    sensorManager.UnsubscribeSensors();
                    StopForeground(true);
                    StopSelf();
                    OnDestroy();
                    
                    
                };
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
                StartForeground(SERVICE_RUNNING_NOTIFICATION_ID, notification);
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
            base.OnDestroy();
        }
    }
    
}