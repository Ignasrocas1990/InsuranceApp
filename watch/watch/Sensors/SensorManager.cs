using System;
using Android.Util;
using watch.Ble;
using Xamarin.Essentials;
using Exception = System.Exception;

namespace watch.Sensors
{

    public class SensorManager
    {
        private const string TAG = "mono-stdout";
        private bool Shake = false;
        public StepDetector stepDetector;
        SensorSpeed speed = SensorSpeed.UI;
        

        public SensorManager()
        {
            Accelerometer.ReadingChanged += AcceReadingChanged;
            Accelerometer.ShakeDetected += (s,e) =>
            {
                Log.Verbose(TAG, "device shaked");
                Shake = true;
            };
            stepDetector = new StepDetector();

        }
//---------------------------- here------------------------------------------------
        void AcceReadingChanged(object s, AccelerometerChangedEventArgs args)
        {
            var vector = args.Reading.Acceleration;
            long timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds() * 1000000;
            if (!Shake)
            {
                stepDetector.updateAccel(timeStamp,vector.X,vector.Y,vector.Z);
            }
            Shake = false;
            
/*
            if (nOfAcc > 2)
            {
                nOfAcc = 0;
                AccEventHandler?.Invoke(this, new SensorArgs(){ Data = filter.GetAcc()});
                filter.ClearAcc();
            }
*/
        }
        public void ToggleSensors(string state)
        {
            try
            {
                if ((Accelerometer.IsMonitoring && state.Equals("Disconnected")) || state.Equals("off"))
                {
                    Log.Verbose(TAG,"acc stop monitoring ");

                    Accelerometer.Stop();
                }
                else
                {
                    Accelerometer.Start(speed);
                    Log.Verbose(TAG,"acc start monitoring ");
                }
            }
            catch (FeatureNotSupportedException fe)
            {
                Log.Verbose(TAG,fe.Message);
            }
            catch (Exception ex)
            {
                Log.Verbose(TAG,ex.Message);

            }
        }
        public void UnsubscribeSensors()
        {
            Accelerometer.ReadingChanged -= AcceReadingChanged;
            //Gyroscope.ReadingChanged -= GyroReadingChanged;
        }
        public  bool isM() => Accelerometer.IsMonitoring;

    }
}