using System;
using System.Text;
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
    public class WatchService : Service
    {
        private const string TAG = "mono-stdout";
        const int ServiceRunningNotificationId = 10000;
        const int MaxDisconnectionTime = 600;//10min=600
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
             if (intent==null)
             {
                 Log.Verbose(TAG, "killed service");
                 sensorManager.ToggleSensors("Connected");
             }
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
                    else if (e.State.Equals("Connected"))
                    {
                        timer.Stop();
                        curDisconnectCounter = 0;
                    }
                    sensorManager.ToggleSensors(e.State);
                    Log.Verbose(TAG, $" is monitoring ? : {sensorManager.isMonitoring()}");
                    
                };
                //check if reading but not sending.(in-case the android killed the process)
                bleServer.BltCallback.ReadHandler += (s, e) =>
                {
                    try
                    {
                        if (!sensorManager.isMonitoring())
                        {
                            sensorManager.ToggleSensors("Connected");
                            timer.Stop();
                            curDisconnectCounter = 0;
                        }
                    }
                    catch (Exception exception)
                    {
                        Console.WriteLine(exception);
                    }
                };
            }
            
        }
        private void DisconnectedCheck(object sender, ElapsedEventArgs e)
        {
            curDisconnectCounter += 1;
            Log.Verbose(TAG,$"{curDisconnectCounter}");
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
                    .SetContentTitle("")
                    ?.SetContentText("")
                    ?.SetOngoing(true)
                    ?.Build();
                StartForeground(ServiceRunningNotificationId, notification);
            }
            catch (Exception e)
            {
                Log.Verbose(TAG,e.Message);
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
            timer.Dispose();
            StopForeground(true);
            StopSelf();
            
        }
    }
    
}