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

        public event EventHandler<SensorArgs> accEventHandler;
        public event EventHandler<SensorArgs> gyroEventHandler;

        static Queue<String> queue = new Queue<String>();  

        SensorArgs sensorArgs;
        SensorSpeed speed = SensorSpeed.UI;

        public SensorManager(){
            Accelerometer.ReadingChanged += AcceReadingChanged;
            Gyroscope.ReadingChanged += GyroReadingChanged;

        }

        private void GyroReadingChanged(object s, GyroscopeChangedEventArgs args)
        {
            var gData = args.Reading;
            string data = "G" + (gData.AngularVelocity.X).ToString() 
                + "," + (gData.AngularVelocity.Y).ToString() + "," + (gData.AngularVelocity.Z).ToString();
            ToggleGyro();
            sensorArgs = new SensorArgs();
            sensorArgs.full = data;
            gyroEventHandler?.Invoke(this, sensorArgs);
        }

        void AcceReadingChanged(object s, AccelerometerChangedEventArgs args)
        {
            var accData = args.Reading;
            string data = "A"+(accData.Acceleration.X).ToString() +","
                +(accData.Acceleration.Y).ToString()+","+(accData.Acceleration.Z).ToString();
           // queue.Enqueue(temp);
            ToggleAcce();

            sensorArgs = new SensorArgs();
            sensorArgs.full = data;
            accEventHandler?.Invoke(this, sensorArgs);
        }


        public static string getSensorData()
        {
            if (queue.Count !=0)
            {
                return queue.Dequeue();
            }
            return null;
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
        public void unsubscribeSensors()
        {
            Accelerometer.ReadingChanged -= AcceReadingChanged;
            Gyroscope.ReadingChanged -= GyroReadingChanged;


        }
    }
    public class SensorArgs:EventArgs{
        public string full { get; set; }
    }
}