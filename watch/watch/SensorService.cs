using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;

namespace watch
{
    [Service]
    public class SensorService : Service
    {
        const int SERVICE_RUNNING_NOTIFICATION_ID = 10000;
        private const string TAG = "mono-stdout";
        private BleServer bleServer;
        private SensorManager sensorManager;

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
            // sensor data transfer to BLE server
            sensorManager.AccEventHandler += (s, e) =>
            {
                bleServer.SensorData.Enqueue(e.Data);
                Log.Verbose(TAG,$"enqued:{e.Data}");


            };
            sensorManager.GyroEventHandler += (s, e) =>
            {
                bleServer.SensorData.Enqueue(e.Data);
                
            };

            //Server communications
            if (bleServer.BltCallback != null)
            {
                bleServer.BltCallback.DisconectHandler += (s, e) =>
                {
                    sensorManager.ToggleSensors();
                };
                
                
                bleServer.BltCallback.DataWriteHandler += (s, e) =>
                {
                    sensorManager.ToggleSensors();
                    sensorManager.UnsubscribeSensors();
                    StopForeground(true);
                    StopSelf();
                
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
    }
    
}