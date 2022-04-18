/*
    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Realms.Sync;
using watch.Ble;
using watch.Sensors;

namespace watch.Services
{
    /// <summary>
    /// Service(Long term background thread) that is main connection between
    /// Monitoring accelerometer & Bluetooth server.
    /// </summary>
    [Service]
    public class WatchService : Service
    {
        private const string Tag = "mono-stdout";
        private const int ServiceRunningNotificationId = 10000;
        private const int MaxTimeAfterSwitchOff = 240;//4 min before dispose;
        private const int MaxRunTime = 14400;// 4 hours max run time
        private const int ElapsedTime = 1000;//1sec
        private static readonly Timer SaveDataTimer = new Timer {Interval = 5000, AutoReset = true};
        
        private int runCounter = 0;
        private int switchCounter = 0;
        private BleServer bleServer;
        private SensorManager sensorManager;
        private Timer runTimeTimer;
        private Timer switchTimer;
        private SqlService localDb;
        private List<string> dataToBeSaved;
        private bool savingData;
        private bool firstTime = true;
        private User user;
        
        /// <summary>
        /// Initialize the service, and if null re-log in (restart of monitoring)
        /// </summary>
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
             MainActivity.mWakeLock.Acquire();
             if (intent is null)
             {
                 Log.Verbose(Tag, "intent is null");
                 ReSetService();
             }
             return StartCommandResult.Sticky;
        }
        /// <summary>
        /// Re-log, start monitoring of movement data
        /// </summary>
        private async Task ReSetService()
        {
            try
            {
                var user = localDb.FindUser();
                if (user != null)
                {
                    await RealmDb.GetInstance().LogIn(user.Email, user.Pass);
                    var saveData = await RealmDb.GetInstance().CheckIfMonitoring();
                    Log.Verbose(Tag, $"RealDb, CheckIfMonitoring = {saveData}");
                    if(!saveData)
                    {
                        SaveDataTimer.Stop();
                        savingData = false;
                        sensorManager.SendDataCounter = -1;//TODO remove from here -------------------------------------
                        OnDestroy();
                    }
                    firstTime = false;
                }
            }
            catch (Exception e)
            {
                Log.Verbose(Tag, e.Message);
            }
        }
        /// <summary>
        /// When above 5 steps saves the steps to database
        /// </summary>
        private async void SaveData(object s, EventArgs args)
        {
            try
            {
                    Log.Verbose(Tag,$"are we SavingData ? {savingData}");
                    if (!savingData) return;
                    if (dataToBeSaved.Count > 0 && RealmDb.RealmApp.CurrentUser != null )
                    {
                        var data = new List<string>(dataToBeSaved);
                        dataToBeSaved.Clear();
                        await RealmDb.GetInstance().AddMovData(data);
                        Log.Verbose(Tag, "Saved Mov Data");
                    }
            }
            catch (Exception e)
            {
                Log.Verbose(Tag, e.Message);
            }
        }
        
        /// <summary>
        /// Used to subscribe to event handlers
        /// </summary>
        private void SubscribeToListeners()
        {
            // sensor data transfer to BLE server
            sensorManager.AccEventHandler += (s, e) =>
            {
                if (savingData)
                {
                    dataToBeSaved.Add(e.Data);
                }
            };
            RealmDb.GetInstance().LoggedInCompleted += OnLoggedInCompleted;
            RealmDb.GetInstance().StopDataGathering += (s,e) =>//stop gathering data for 4min before shut down
            {
                switchCounter = 0;
                switchTimer.Start();
                SaveDataTimer.Stop();
                savingData = false;
                Log.Verbose(Tag, "Stopping data gathering");
                sensorManager.ToggleSensors("Disconnected");
                //sensorManager.SendDataCounter = -1; //TODO remove from here ------------------------------------------------
                
            };
            //Server communications
            if (bleServer.BltCallback == null) return;
            bleServer.BltCallback.DataWriteHandler += async (s,e) =>// start writing the data
            {
                if (e.Value == null) return;
                var detailsString = Encoding.Default.GetString(e.Value);
                
                if (detailsString.Equals("Stop"))
                {
                    Log.Verbose(Tag, " STOP the data sending =>>>>>"+detailsString); 
                    await RealmDb.GetInstance().UpdateSwitch(false);
                    RealmDb.GetInstance().StopDataGathering.Invoke(this,EventArgs.Empty);
                }
                else
                {
                    await LogIn(detailsString);
                }
                Log.Verbose(Tag,$"received write details");
            };

            bleServer.BltCallback.StateHandler += (s, e) =>//Todo Maybe remove=========================
            {
                switch (e.State)
                {
                    case "Disconnected":
                        runCounter = 0;
                        runTimeTimer.Start();
                        break;
                    
                    case "Connected":
                        savingData = true;
                        switchTimer.Stop();
                        switchCounter = 0;
                        runTimeTimer.Stop();
                        runCounter = 0;
                        sensorManager.ToggleSensors("Connected");
                        Log.Verbose(Tag, $" is monitoring ? : {sensorManager.isMonitoring()}");
                        break;
                }
            };
            SaveDataTimer.Elapsed += SaveData;
        }
        /// <summary>
        /// called when the real db completes the log in
        /// It starts seniors monitoring and updates customers monitoring switch
        /// </summary>
        private async void OnLoggedInCompleted(object s, EventArgs e)
        {
            Log.Verbose(Tag, "logged IN Completed =============================");
            savingData = true;
            sensorManager.ToggleSensors("Connected");
            SaveDataTimer.Start(); // Starts saving gathered data
            await RealmDb.GetInstance().UpdateSwitch(true);

            //sensorManager.SendDataCounter = 0; //TODO remove from here -------------------------------------##################
            //sensorManager.SendTestData(); //TODO remove from here -------------------------------------##################
        }

        /// <summary>
        /// After bluetooth connection made, perform log in to cloud database
        /// </summary>
        /// <param name="detailsString">User Id,password,email</param>
        private async Task LogIn(string detailsString)
        {
            try
            {
                Log.Verbose(Tag,"Login in with details: "+detailsString);
                var splitData = detailsString.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries);
                Log.Verbose(Tag,"splitData.length : "+splitData.Length);
                if (firstTime)
                {
                    var user = localDb.FindUser();
                    if (user is null && splitData.Length > 1)
                    {
                        localDb.AddUser(splitData[0], splitData[1], splitData[2]);
                    }
                }
                await RealmDb.GetInstance().LogIn(splitData[1], splitData[2]);
                firstTime = false;
            }
            catch (Exception e)
            {
                Log.Verbose(Tag,$"LogIn WatchService Error : {e}");
            }
            
        }
        /// <summary>
        /// Allows only particular time for run time when not connected via phone
        /// </summary>
        private void RunTimeCheck(object sender, ElapsedEventArgs e)
        {
            
            Log.Verbose(Tag,$"r.c.:{runCounter}");
            if (++runCounter == MaxRunTime) OnDestroy();
            
        }
        private void SwitchTimeCheck(object sender, ElapsedEventArgs e)
        {
            Log.Verbose(Tag,$"s.c.:{switchCounter}");
            if (switchCounter++ == MaxTimeAfterSwitchOff) OnDestroy();
        }

        // -------------------------------------- Support methods ---------------------------------------------------------
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
                Log.Verbose(Tag,e.Message);
            }
        }
        private void CreateNotificationChannel() {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) {
                return;
            }
            Log.Verbose(Tag, "Notification created");
            var channel = new NotificationChannel("999", "channelName", NotificationImportance.Default) {
                Description = "channelDescription"
            };
            var notificationManager = (NotificationManager) GetSystemService(NotificationService);
            if (notificationManager != null) notificationManager.CreateNotificationChannel(channel);
        }
        public override async void OnDestroy()
        {
            try
            {
                Log.Verbose(Tag, "Service Closing ==================[OnDestroy]===========================");
                savingData = false;
                await RealmDb.GetInstance().UpdateSwitch(false);
                dataToBeSaved = null;

                bleServer.StopAdvertising();
                bleServer.Dispose();
                sensorManager.ToggleSensors("Disconnected");
                sensorManager.UnsubscribeSensors();
                runTimeTimer.Dispose();
                switchTimer.Dispose();
                localDb.Dispose();
                SaveDataTimer.Dispose();
                StopForeground(true);
                StopSelf();
                MainActivity.mWakeLock.Release();
                MainActivity.mWakeLock.Dispose();
                MainActivity.Fin();
            }
            catch (Exception e)
            {
               Log.Verbose(Tag,e.Message);
            }
        }
        public override IBinder OnBind(Intent intent) =>  null;
    }
    
}