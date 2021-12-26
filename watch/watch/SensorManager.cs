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
        private GyroscopeData oldGyroData = new GyroscopeData(0.0, 0.0, 0.0);
        private AccelerometerData oldAccData = new AccelerometerData(0.0, 0.0, 0.0);

        AccArgs accArgs;
        private string gyroTemp;
        private string accTemp;

        SensorSpeed speed = SensorSpeed.UI;

        public SensorManager(){
            Accelerometer.ReadingChanged += AcceReadingChanged;
            Gyroscope.ReadingChanged += GyroReadingChanged;
            
            
            gyroArgs = new GyroArgs {Full = "n"};
            accArgs = new AccArgs {Full = "n"};
            
        }

        private void GyroReadingChanged(object s, GyroscopeChangedEventArgs args)
        {
            var g = args.Reading;
            if (g.Equals(oldGyroData))
            {
                return;
            }
            oldGyroData = g;
            
            gyroArgs.Full= "G" + (g.AngularVelocity.X).ToString() 
                               + "," + (g.AngularVelocity.Y).ToString() + "," + (g.AngularVelocity.Z).ToString();
            
            GyroEventHandler?.Invoke(this, gyroArgs);
            
            
          
        }

        void AcceReadingChanged(object s, AccelerometerChangedEventArgs args)
        {
            var currentReading = args.Reading;
            if (currentReading.Equals(oldAccData))
            {
                return;
            }
            oldAccData = currentReading;
            accArgs.Full = "A"+(currentReading.Acceleration.X).ToString() +","
                           +(oldAccData.Acceleration.Y).ToString()+","+(currentReading.Acceleration.Z).ToString();

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