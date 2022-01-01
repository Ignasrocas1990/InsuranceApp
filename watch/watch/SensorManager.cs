using System;
using Android.Util;
using Xamarin.Essentials;

namespace watch
{

    public class SensorManager
    {

        public event EventHandler<SensorArgs> AccEventHandler;
        public event EventHandler<SensorArgs> GyroEventHandler;
        
        private int nOfAcc;
        private int nOfGyro;
        private MaFilter filter;

        SensorSpeed speed = SensorSpeed.UI;

        public SensorManager()
        {
            filter = new MaFilter();
            Accelerometer.ReadingChanged += AcceReadingChanged;
            Gyroscope.ReadingChanged += GyroReadingChanged;
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

        void AcceReadingChanged(object s, AccelerometerChangedEventArgs args)
        {
            var reading = args.Reading;
            filter.AddAcc(reading.Acceleration);
            nOfAcc++;
            if (nOfAcc > 2)
            {
                nOfAcc = 0;
                AccEventHandler?.Invoke(this, new SensorArgs(){ Data = filter.GetAcc()});
                filter.ClearAcc();
            }
        }
        public void ToggleSensors()
        {
            try
            {
                if (Accelerometer.IsMonitoring || Gyroscope.IsMonitoring)
                {
                    Accelerometer.Stop();
                    Gyroscope.Stop();
                    filter.ClearAcc();
                    filter.ClearGyro();
                    nOfAcc = 0;
                    nOfGyro = 0;
                }
                else
                {
                    Accelerometer.Start(speed);
                    Gyroscope.Start(speed);
                }
            }
            catch (FeatureNotSupportedException fe)
            {
                Log.Verbose("mon-stdout",fe.Message);
            }
            catch (Exception ex)
            {
                Log.Verbose("mon-stdout",ex.Message);

            }
        }
        public void UnsubscribeSensors()
        {
            Console.WriteLine("UnsubscribeSensors><");
            Accelerometer.ReadingChanged -= AcceReadingChanged;
            Gyroscope.ReadingChanged -= GyroReadingChanged;
        }
        public  bool isM() => Gyroscope.IsMonitoring;

    }
    public class SensorArgs:EventArgs{
        public string Data { get; set; }
    }
    
}