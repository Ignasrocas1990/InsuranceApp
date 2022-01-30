using System;
using Android.Util;
using Java.Lang;
using Xamarin.Essentials;
using Exception = System.Exception;

namespace watch.Sensors
{

    public class SensorManager
    {
        private const string TAG = "mono-stdout";
        
        public event EventHandler<SensorArgs> AccEventHandler;
        
        private int nOfAcc;
        private BleMaFilter filter;

        SensorSpeed speed = SensorSpeed.UI;
        

        public SensorManager()
        {
            filter = new BleMaFilter();
            Accelerometer.ReadingChanged += AcceReadingChanged;
            nOfAcc = 0;
        }

        void AcceReadingChanged(object s, AccelerometerChangedEventArgs args)
        {
            var reading = args.Reading;
            filter.AddAcc(reading.Acceleration);
            nOfAcc++;
            if (nOfAcc > 2)
            {
                nOfAcc = 0;
                string data = "";
                long timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
                data = timeStamp + filter.GetAcc();
                AccEventHandler?.Invoke(this, new SensorArgs(){ Data = data});
                filter.ClearFilter();
            }
        }
        public void ToggleSensors(string state)
        {
            try
            {
                if (Accelerometer.IsMonitoring && state.Equals("Disconnected"))
                {
                    Accelerometer.Stop();
                    Thread.Sleep(1000);
                }
                else if(!Accelerometer.IsMonitoring && state.Equals("Connected"))
                {
                    Accelerometer.Start(speed);
                    Thread.Sleep(1000);
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
            Log.Verbose(TAG, "SensorManager : unsubscribed");
            Accelerometer.ReadingChanged -= AcceReadingChanged;
        }
        public  bool isM() => Accelerometer.IsMonitoring;
        
    }
    public class SensorArgs:EventArgs{
        public string Data { get; set; }
    }
    
}