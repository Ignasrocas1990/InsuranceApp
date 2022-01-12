using System;
using Android.Util;
using Java.Lang;
using Xamarin.Essentials;
using Exception = System.Exception;
using Math = Java.Lang.Math;

namespace watch.Sensors
{

    public class SensorManager
    {
        private const string TAG = "mono-stdout";
        private double prevMessurement = 0.0f;
        private int counter = 0;
        private bool Shake = false;
        
        public event EventHandler<SensorArgs> AccEventHandler;
        public event EventHandler<SensorArgs> GyroEventHandler;
        
        private int nOfAcc;
        private int nOfGyro;
        private BleMaFilter filter;
        public StepDetector stepDetector;
        SensorSpeed speed = SensorSpeed.UI;
        

        public SensorManager()
        {
            filter = new BleMaFilter();
            Accelerometer.ReadingChanged += AcceReadingChanged;
            Accelerometer.ShakeDetected += (s,e) =>
            {
                Log.Verbose(TAG, "device shaked");
                Shake = true;
            };
            stepDetector = new StepDetector();
            //Gyroscope.ReadingChanged += GyroReadingChanged;
            nOfAcc = 0;
            nOfGyro = 0;
            
        }

        private void GyroReadingChanged(object s, GyroscopeChangedEventArgs args)
        {
            var reading = args.Reading;
            filter.AddGyro(reading.AngularVelocity);
            nOfGyro++;
            if (nOfGyro > 2)
            {
                nOfGyro = 0;
                GyroEventHandler?.Invoke(this, new SensorArgs(){Data = filter.GetGyro()});
                filter.ClearGyro();
            }
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
            filter.AddAcc(reading.Acceleration);
            nOfAcc++;
            if (nOfAcc > 2)
            {
                nOfAcc = 0;
                AccEventHandler?.Invoke(this, new SensorArgs(){ Data = filter.GetAcc()});
                filter.ClearAcc();
            }
*/
        }
        public void ToggleSensors()
        {
            try
            {
                if (Accelerometer.IsMonitoring) //|| Gyroscope.IsMonitoring)
                {
                    Log.Verbose(TAG,"acc stop monitoring ");

                    Accelerometer.Stop();
                    //Gyroscope.Stop();
                    //filter.ClearAcc();
                    //filter.ClearGyro();
                    nOfAcc = 0;
                    nOfGyro = 0;
                }
                else
                {
                    Accelerometer.Start(speed);
                    Log.Verbose(TAG,"acc start monitoring ");
                    //Gyroscope.Start(speed);
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
    public class SensorArgs:EventArgs{
        public string Data { get; set; }
    }
    
}