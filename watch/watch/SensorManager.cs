using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xamarin.Essentials;

namespace watch
{

    public class SensorManager
    {

        public event EventHandler<AccArgs> AccEventHandler;
        public event EventHandler<GyroArgs> GyroEventHandler;
        
        GyroArgs gyroArgs;
        AccArgs accArgs;
        private string gyroTemp;
        private string accTemp;

        SensorSpeed speed = SensorSpeed.UI;

        public SensorManager(){
            Accelerometer.ReadingChanged += AcceReadingChanged;
            Gyroscope.ReadingChanged += GyroReadingChanged;
            gyroArgs = new GyroArgs();
            gyroArgs.Full = "n";
            accArgs = new AccArgs();
            accArgs.Full = "n";


        }

        private void GyroReadingChanged(object s, GyroscopeChangedEventArgs args)
        {
            var g = args.Reading;
            
            gyroTemp = "G" + (g.AngularVelocity.X).ToString() 
                              + "," + (g.AngularVelocity.Y).ToString() + "," + (g.AngularVelocity.Z).ToString();
            if (gyroTemp.Contains(gyroArgs.Full)) return;

            gyroArgs.Full = gyroTemp;
            GyroEventHandler?.Invoke(this, gyroArgs);
        }

        void AcceReadingChanged(object s, AccelerometerChangedEventArgs args)
        {
            var accData = args.Reading;
            accTemp = "A"+(accData.Acceleration.X).ToString() +","
                +(accData.Acceleration.Y).ToString()+","+(accData.Acceleration.Z).ToString();
            
            if (accTemp.Contains(accArgs.Full)) return;
            accArgs.Full = accTemp;
            AccEventHandler?.Invoke(this, accArgs);
        }
        
        public void ToggleAcce()
        {
            try
            {
                if (Accelerometer.IsMonitoring)

                    Accelerometer.Stop();
                else
                    Accelerometer.Start(speed);
            }
            catch (FeatureNotSupportedException fe)
            {
                Console.WriteLine(" Feature not supported on device--------------" + fe.Message);
            }
            catch (Exception oe)
            {
                Console.WriteLine("--------------------- other error ? " + oe.Message);
            }
            
            
        }
        public void ToggleGyro()
        {
            try
            {
                if (Gyroscope.IsMonitoring)
                    Gyroscope.Stop();
                else
                    Gyroscope.Start(speed);
            }
            catch (FeatureNotSupportedException fe)
            {
                Console.WriteLine(" - - - - -- ------------- Feature not supported on device");
            }
            catch (Exception ex)
            {
                Console.WriteLine(" - - - - -- ------------- Other error has occurred.");

            }
        }
        public void UnsubscribeSensors()
        {
            Accelerometer.ReadingChanged -= AcceReadingChanged;
            Gyroscope.ReadingChanged -= GyroReadingChanged;


        }
    }
    public class GyroArgs:EventArgs{
        public string Full { get; set; }
    }
    public class AccArgs:EventArgs{
        public string Full { get; set; }
    }
}