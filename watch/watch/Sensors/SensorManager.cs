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
using System.Threading.Tasks;
using Android.Util;
using Xamarin.Essentials;

namespace watch.Sensors
{
    /// <summary>
    /// Uses Xamarin Essentials to monitor sensor movement data
    /// using accelerometer.
    /// </summary>
    public class SensorManager
    {
        private const string Tag = "mono-stdout";
        
        public event EventHandler<SensorArgs> AccEventHandler;
        private readonly StepDetector detector;
        private const SensorSpeed Speed = SensorSpeed.UI;
        private long shakeDetected = 0;
        private const long ShakeTimeGap = 500;
        private int count = 0;
        private List<float> data;



        public SensorManager()
        {
            detector = new StepDetector();
            Accelerometer.ReadingChanged += AcceReadingChanged;
            Accelerometer.ShakeDetected += ShakeDetected;
            data = new List<float>();
        }
        
        /// <summary>
        /// When detecting shake initialize when it happened
        /// </summary>
        private void ShakeDetected(object sender, EventArgs e)=>
            shakeDetected = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        
        /// <summary>
        /// Detects a Acceleration reading, checks vs shake detected, and using StepDetector
        /// and sends reading to WatchService via EventHandler
        /// </summary>
        void AcceReadingChanged(object s, AccelerometerChangedEventArgs args)
        {
            var vec = args.Reading.Acceleration;
            var timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            if (detector.UpdateAccel(timeStamp, vec.X, vec.Y, vec.Z) == 1 && (shakeDetected+ShakeTimeGap) <= timeStamp)
            {
                Log.Verbose(Tag,$"step counted {++count}");
                AccEventHandler?.Invoke(this, new SensorArgs()
                {
                    Data = $"{vec.X},{vec.Y},{vec.Z}"
                });

            }
        }
        /// <summary>
        /// Starts/Stops monitoring
        /// </summary>
        /// <param name="state">start stop monitoring</param>
        public void ToggleSensors(string state)
        {
            try
            {
                switch (Accelerometer.IsMonitoring)
                {
                    case true when state.Equals("Disconnected"):
                        Accelerometer.Stop();
                        Log.Verbose(Tag, "ToggleSensors,stop to monitor");
                        break;
                    case false when state.Equals("Connected"):
                        Accelerometer.Start(Speed);
                        Log.Verbose(Tag, "ToggleSensors,start to monitor");
                        break;
                }
            }
            catch (FeatureNotSupportedException fe)
            {
                Log.Verbose(Tag,fe.Message);
            }
            catch (Exception ex)
            {
                Log.Verbose(Tag,ex.Message);

            }
        }
        /// <summary>
        /// Unsubscribes from monitoring
        /// </summary>
        public void UnsubscribeSensors()
        {
            Log.Verbose(Tag, "SensorManager : unsubscribed");
            Accelerometer.ReadingChanged -= AcceReadingChanged;
        }
        public  bool isMonitoring() => Accelerometer.IsMonitoring;

        public int SendDataCounter;//TODO remove from here --------------------------------
        public async Task SendTestData()//TODO remove from here --------------------------------
        {
            if (SendDataCounter == -1) return; 
            await Task.Delay(10000);
           await Task.Run(() =>
           {
               if (AccEventHandler == null) return;
               for (int i = 0; i < 3; i++)
               {
                   if (AccEventHandler != null)
                       AccEventHandler.Invoke(this, new SensorArgs()
                       {
                           Data = "0000,0000,0000"
                       });
                    
               } 
               SendTestData();
           });
        }
    }
    public class SensorArgs:EventArgs{
        public string Data { get; set; }
    }
    
}