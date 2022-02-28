using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using watch.Ble;
using watch.Models;
using watch.Sensors;

namespace watch.Services
{
    [Service]
    public class WatchService : Service
    {
        private const string TAG = "mono-stdout";
        
        const int ServiceRunningNotificationId = 10000;
        private const int MaxTimeAfterSwitchOff = 600;//10 min before dispose;
        const int MaxRunTime = 14400;// 4 hours max run time
        private const int ElapsedTime = 1000;//1sec
        
        private int runCounter = 0;
        private int switchCounter = 0;
        private BleServer bleServer;
        private SensorManager sensorManager;
        private Timer runTimeTimer;
        private Timer switchTimer;
        private SqlService localDb;
        private List<string> dataToBeSaved;
        private bool stopSavingData;
        
        public override StartCommandResult OnStartCommand(Intent intent, StartCommandFlags flags, int startId)
        {
             StartForegroundNotification();
             dataToBeSaved = new List<string>();
             localDb = SqlService.GetInstance();

             bleServer =  new BleServer(this);
             sensorManager = new SensorManager();
             switchTimer = new Timer(ElapsedTime);
             switchTimer.Elapsed += SwitchTimeCheck;
             runTimeTimer = new Timer(ElapsedTime);
             runTimeTimer.Elapsed += RunTimeCheck;
             runTimeTimer.Start();
             SubscribeToListeners();
             ReSetService();

             return StartCommandResult.Sticky;
        }
        private async Task ReSetService()
        {
            try
            {
                var user = localDb.FindUser();
                if (user != null)
                {
                    await RealmDb.GetInstance().LogIn(user.Email, user.Pass);
                    var isMonitoring = await RealmDb.GetInstance().CheckIfMonitoring();
                    if (!isMonitoring)
                    {
                        switchTimer.Start();
                        SaveData();
                    }
                    else
                    {
                        runCounter = 0;
                        runTimeTimer.Start();
                        sensorManager.ToggleSensors("Connected");
                        sensorManager.SendTestData(); //TODO remove from here -------------------------------------
                    }
                }
            }
            catch (Exception e)
            {
                Log.Verbose(TAG, e.Message);
            }
        }
        private async Task SaveData()
        {
            try
            {
                Log.Verbose(TAG,$"Do we stop SavingData? {stopSavingData}");
                if (stopSavingData) return;
                //Task.Run(async () =>
               // {
               Log.Verbose(TAG,$"count SavingData? {dataToBeSaved.Count}");
                    if (dataToBeSaved.Count >= 5)
                    {
                        var data = new List<string>(dataToBeSaved);
                       dataToBeSaved.Clear();
                       await RealmDb.GetInstance().AddMovData(data); 
                       SaveData();
                    }
                    //});
            }
            catch (Exception e)
            {
                Log.Verbose(TAG, e.Message);
            }
        }

        /**
         * Subscribe to sensor and Server
         * to communicate in between
         */
        private void SubscribeToListeners()
        {
            // sensor data transfer to BLE server
            sensorManager.AccEventHandler += (s, e) =>
            {
                if (stopSavingData)
                {
                    bleServer.SensorData.Enqueue(e.Data);
                    dataToBeSaved.Add(e.Data);
                    if (dataToBeSaved.Count >=5)
                    { 
                        SaveData();
                    }
                }
                else
                {
                    sensorManager.sendDataCounter = -1;
                }
                
            };
            RealmDb.GetInstance().LoggedInCompleted += (s,e) =>
            {
                
                Log.Verbose(TAG, "User longed in");
                SaveData();
            };
            RealmDb.GetInstance().StopDataGathering += (s,e) =>//stop gathering data for 20min before shut down
            {
                if (switchCounter==0)
                {
                    switchCounter = 0;
                    switchTimer.Start();
                }
                else if (switchCounter>300)
                {
                    sensorManager.ToggleSensors("Disconnected");
                    sensorManager.sendDataCounter = -1; //TODO remove from here ------------------------------------------------
                }
               
                
            };
            //Server communications
            if (bleServer.BltCallback == null) return;
            bleServer.BltCallback.DataWriteHandler += (s,e) =>// start writing the data
            {
                if (e.Value == null) return;
                String detailsString = Encoding.Default.GetString(e.Value);
                if (detailsString.Equals("Stop"))
                {
                    stopSavingData = true;
                    sensorManager.ToggleSensors("Disconnected");
                    switchTimer.Start();
                }
                else
                {
                    LogIn(detailsString);
                }
                Log.Verbose(TAG,$"received write details");
            };

            bleServer.BltCallback.StateHandler += (s, e) =>
            {
                switch (e.State)
                {
                    case "Disconnected":
                        runCounter = 0;
                        runTimeTimer.Start();
                        break;
                    
                    case "Connected":
                        switchTimer.Stop();
                        switchCounter = 0;
                        runTimeTimer.Stop();
                        runCounter = 0;
                        sensorManager.ToggleSensors("Connected");
                        sensorManager.SendTestData(); //TODO remove from here -------------------------------------
                        Log.Verbose(TAG, $" is monitoring ? : {sensorManager.isMonitoring()}");
                        break;
                }
            };
            //check if reading but not sending.(in-case the android killed the process)
            bleServer.BltCallback.ReadHandler += (s, e) =>
            {
                try
                {
                    runTimeTimer.Stop();
                    runCounter = 0;
                    if (!sensorManager.isMonitoring()) sensorManager.ToggleSensors("Connected");
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            };
        }

        private async Task LogIn(string detailsString)
        {
            try
            {
                var splitData = detailsString.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                User user = localDb.FindUser();
                if (user is null)
                {
                    localDb.AddUser(splitData[0], splitData[1], splitData[2]);
                    user = localDb.FindUser();
                    await RealmDb.GetInstance().LogIn(user.Email, user.Pass);
                }
            }
            catch (Exception e)
            {
                Log.Verbose(TAG,"LogIn WatchService Error");
            }
            
        }
        
        private void RunTimeCheck(object sender, ElapsedEventArgs e)
        {
            
            runCounter += 1;
            if (runCounter == MaxRunTime)
            {
                
                OnDestroy();
            }
        }
        private void SwitchTimeCheck(object sender, ElapsedEventArgs e)
        {
            
            switchCounter += 1;
            if (switchCounter == MaxTimeAfterSwitchOff)
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
                return;
            }

            var channel = new NotificationChannel("999", "channelName", NotificationImportance.Default) {
                Description = "channelDescription"
            };
            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            if (notificationManager != null) notificationManager.CreateNotificationChannel(channel);
        }
        public override async void OnDestroy()
        {
            Log.Verbose(TAG, "Service Closing ==================[OnDestroy]===========================");
            stopSavingData = true;
            await RealmDb.GetInstance().UpdateSwitch();
            dataToBeSaved = null;
            
            bleServer.StopAdvertising();
            bleServer.Dispose();
            sensorManager.ToggleSensors("Disconnected");
            sensorManager.UnsubscribeSensors();
            runTimeTimer.Dispose();
            localDb.Dispose();
            StopForeground(true);
            StopSelf();
            MainActivity.Fin();
        }
        public override IBinder OnBind(Intent intent) =>  null;
    }
    
}