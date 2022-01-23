using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Text.Style;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Exception = System.Exception;

namespace Insurance_app.Communications
{
    public class BleManager
    {


        private IAdapter adapter;
        public Ble ble;
        private StepDetector stepDetector;
        private ICharacteristic chara=null;
        public EventHandler<RawDataArgs> InfferEvent = delegate {  };

        private int readingDelay = 0;
        private int conErrDelay = 0;
        private bool bleState = false;
        private bool isMonitoring = false;
        private bool firstSet = true;
        private static BleManager bleManager =null;
        
        private BleManager()
        {
            ble = new Ble();
            adapter = CrossBluetoothLE.Current.Adapter;
            RegisterEventHandlers();
            stepDetector = new StepDetector();
            bleState=ble.BleCheck();

        }
        public static BleManager GetInstance()
        {
            if (bleManager is null)
            {
                return new BleManager();
            }

            return bleManager;
        }
        
        private void RegisterEventHandlers()
        {
            
            ble.ble.StateChanged += (s,e) =>
            {
                Console.WriteLine($"Ble state changed {e.NewState.ToString()}");
                if (e.NewState == BluetoothState.On)
                {
                    bleState = true;
                    ConnectToDevice();
                }else if (e.NewState == BluetoothState.Off || e.NewState == BluetoothState.TurningOff)
                {
                    bleState = false;
                }
            };
            adapter.DeviceConnected += (s, e) =>
            {
                Console.WriteLine($"device connected : {e.Device.Name}");
                ConnectToService(e.Device);
            };
        }

        private async Task ReadAsync()
        {
            if(chara is null || !chara.CanRead) return;
                //HomePageViewModel.MovDataCommandToggle.Execute(null);
            try
            {
                var data = await chara.ReadAsync();

                string str = " ";
                str = Encoding.Default.GetString(data);
                if (str.Equals(" "))
                {
                    if (!isMonitoring) return;

                    readingDelay += 3000;
                    Console.WriteLine($"reading empty : wait {readingDelay / 1000}sec > try again");
                    Task t = Task.Run(async () =>
                    {
                        await Task.Delay(readingDelay);
                        ReadAsync();
                    });
                    return;
                }

                readingDelay = 0;
                //Console.WriteLine($"Read complete, values are : > {str}");
                Infer(str);
                await ReadAsync();
            }
            catch (CharacteristicReadException readException)
            {
                Console.WriteLine("char exception");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                conErrDelay += 3000;
                Console.WriteLine($"[read disturbed] wait {conErrDelay/1000}s: reconnect to device");

                Task t = Task.Run(async () =>
                {
                    await Task.Delay(conErrDelay);
                   await ConnectToDevice();
                    
                });
            }
            
        }

        private void Infer(string rawData)
        {
            try
            {
                var splitedData = rawData.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var timeStamp = Convert.ToInt64(splitedData[0]);
                var x = Converter.StringToFloat(splitedData[1]);
                var y = Converter.StringToFloat(splitedData[2]);
                var z = Converter.StringToFloat(splitedData[1]);
                
                var isStep= stepDetector.updateAccel(timeStamp, x, y, z);
                InfferEvent?.Invoke(this,new RawDataArgs()
                {
                    x = x,y = y,z = z,
                    Type = isStep,
                    TimeOffset = Converter.ToDTOffset(timeStamp)
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem pre-paring data for inferring {e}");
            }
        }


        private async Task GetCharaAsync(IService service)
        {
            try
            {
                chara = await service.GetCharacteristicAsync(ble.SERVER_GUID);
                if (chara!=null)
                {
                    chara.ValueUpdated += (o, e) =>
                    {
                        
                        Console.WriteLine("characteristic changed ----------------------------");
                    };
                    await chara.StartUpdatesAsync();
                    
                    firstSet = false;
                    Console.WriteLine("characteristic found ");
                    ReadAsync();
                }
                else
                {
                    ConnectToDevice();
                }

            }
            catch (Exception e)
            {
                await ConnectToDevice();
                Console.WriteLine($"GetCharacteristic error : {e}");
            }
        }
        

        private async Task ConnectToService(IDevice device)
        {
            try
            {
                var service = await device.GetServiceAsync(ble.SERVER_GUID);
                    if (service == null && firstSet)
                    {
                        MainThread.BeginInvokeOnMainThread( async () =>
                        {
                         await Shell.Current.DisplayAlert("Error"
                             , "Please install & turn on the watch app", "close");
                        });

                        return;
                    }
                    if (service != null)//when app is 
                    {
                        Console.WriteLine("Service Found");
                        await GetCharaAsync(service);
                        return;
                    }
            }
                catch (Exception e)
                {
                    Console.WriteLine($"Service error: {e.Message} ");
                    ConnectToDevice();
                }
        }
        
        private async Task ConnectToDevice()
        {
            if (!ble.IsAvailable() || !await ble.GetPremissionsAsync())
            {
                MainThread.BeginInvokeOnMainThread( async () =>
                {
                    await Shell.Current.DisplayAlert("Error",
                        "Type of Bluetooth not available and app needs your permissions", "close");
                   
                });
                return;
            }
            else if (bleState)
            {
                try
                {
                    var list = adapter.GetSystemConnectedOrPairedDevices(new Guid[] {ble.SERVER_GUID});
                    await adapter.ConnectToDeviceAsync(list[0]);
                    conErrDelay = 0;
                }
                catch
                {
                    //dont need to see an error message, since this is depends connection loss
                    conErrDelay += 3000;
                    Console.WriteLine($" Device Conn Fail : wait {conErrDelay/1000}s , Reconnect");
                    Task t = Task.Run(async ()=>
                    {
                        await Task.Delay(conErrDelay);
                        ConnectToDevice();
                    });
                    
                }
               
            }
        }
        public async Task ToggleMonitoring()
        {
            string monitoringString=" ";
            if (isMonitoring)
            {
                monitoringString = "Disconnected";
                isMonitoring = false;
            }
            else
            {
                monitoringString = "Connected";
                isMonitoring = true;
                await ConnectToDevice();
            }
            if (chara is null || !(chara.CanWrite)) return;
            try
            {
                await chara.WriteAsync(Encoding.ASCII.GetBytes(monitoringString));
            }
            catch (Exception e)
            {
                Console.WriteLine($"stop monitoring sending exception {e}");
            }
            

        }
    }
    public partial class RawDataArgs : EventArgs
    {
        public int Type { get; set; }
        public DateTimeOffset TimeOffset { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}