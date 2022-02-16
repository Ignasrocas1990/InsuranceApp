using System;
using System.Threading.Tasks;
using Android.Nfc;
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
        private StepDetector detector;
        SensorSpeed speed = SensorSpeed.UI;
        private long shakeDetected = 0;
        private const long ShakeTimeGap = 300;
        private int count = 0;

        

        public SensorManager()
        {
            detector = new StepDetector();
            Accelerometer.ReadingChanged += AcceReadingChanged;
            Accelerometer.ShakeDetected += ShakeDetected;
        }

        private void ShakeDetected(object sender, EventArgs e)
        {
            shakeDetected = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
        }

        void AcceReadingChanged(object s, AccelerometerChangedEventArgs args)
        {
            var vec = args.Reading.Acceleration;
            long timeStamp = new DateTimeOffset(DateTime.UtcNow).ToUnixTimeMilliseconds();
            if (detector.updateAccel(timeStamp, vec.X, vec.Y, vec.Z) == 1 && (shakeDetected+ShakeTimeGap) <= timeStamp)
            {
                Log.Verbose(TAG,$"step counted {++count}");

                AccEventHandler?.Invoke(this, new SensorArgs()
                {
                    Data = $"{vec.X},{vec.Y},{vec.Z}"
                });

            }
            
        }
        public void ToggleSensors(string state)
        {
            try
            {
                if (Accelerometer.IsMonitoring && state.Equals("Disconnected"))
                {
                    Accelerometer.Stop();
                }
                else if(!Accelerometer.IsMonitoring && state.Equals("Connected"))
                {
                    Accelerometer.Start(speed);
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
        public  bool isMonitoring() => Accelerometer.IsMonitoring;

        public int sendDataCounter;//TODO remove from here --------------------------------

        public void SendTestData()//TODO remove from here --------------------------------
        {
            if (sendDataCounter > 5) return;
            Task task = Task.Run( async () =>
            {
                if (AccEventHandler == null) return;
                for (int i = 0; i < 5; i++)
                {
                    if (AccEventHandler != null)
                        AccEventHandler.Invoke(this, new SensorArgs()
                        {
                            Data = "0000,0000,0000"
                        });
                    
                }
                await Task.Delay(10000);
                sendDataCounter++;
                Log.Debug(TAG, "sending test data "+sendDataCounter);
                SendTestData();
            });

        }
    }
    
    public class SensorArgs:EventArgs{
        public string Data { get; set; }
    }
    
}