using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Exception = System.Exception;

namespace Insurance_app.Communications
{
    public class BleManager
    {


        private readonly IAdapter adapter;
        public Ble ble;

        private readonly EventHandler<byte[]> readCompleted;
        private ICharacteristic chara=null;
        private int serviceDelay = 0;
        private int readingDelay = 0;
        private int conErrDelay = 0;
        private bool bleState = false;
        private StepDetector stepDetector;
        Func<String,float>convertToFloat =  x => float.Parse(x, CultureInfo.InvariantCulture.NumberFormat);

        public EventHandler<string> InferEvent;

        public BleManager()
        {
            ble = new Ble();
            adapter = CrossBluetoothLE.Current.Adapter;
            RegisterEventHandlers();
            bleState=ble.BleCheck();
            ConnectToDevice();//<-------------------------will be different later
            stepDetector = new StepDetector();

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
           // Console.WriteLine("Reading...");
            try
            {
                var data =  await chara.ReadAsync();

                string str = " ";
                str = Encoding.Default.GetString(data);
               if (str.Equals(" "))
               {
                   readingDelay += 3000;
                   Console.WriteLine($"reading empty : wait {readingDelay/1000}sec > try again");
                   Task t = Task.Run(async () =>
                   {
                       await Task.Delay(readingDelay);
                       await ReadAsync();
                   });
                   return;
               }
               readingDelay = 0;
               Console.WriteLine($"Read complete, values are : > {str}");
               Infer(str);
               await ReadAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                conErrDelay += 3000;
                Console.WriteLine($"[read exception] wait {conErrDelay/1000}s: reconnect to device");

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
                long timeStamp = Convert.ToInt64(splitedData[0]);
                stepDetector.updateAccel(timeStamp,convertToFloat(splitedData[1]),
                    convertToFloat(splitedData[2]),
                    convertToFloat(splitedData[3]));
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
                    Console.WriteLine("characteristic found ");
                    await ReadAsync();
                }
                else
                {
                    await ConnectToDevice();
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
                    if (service != null)
                    {
                        Console.WriteLine("Service Found");
                        serviceDelay = 0;
                        await GetCharaAsync(service);
                        return;
                    }

                    serviceDelay += 3000;
                    Console.WriteLine($"[no Service] wait {serviceDelay/1000} s, try again");
                    Task t = Task.Run( async () =>
                    {
                        await Task.Delay(serviceDelay);
                        ConnectToService(device);
                    });
                }
                catch (Exception e)
                {
                    serviceDelay = 0;
                    Console.WriteLine($"Service error: {e.Message} ");
                    await ConnectToDevice();
                }
            
        }
        
        private async Task ConnectToDevice()
        {
            if (!ble.IsAvailable())
            {
                Console.WriteLine("Bluetooth type needed for app is not available");
                return;
            }
            if (!await ble.GetPremissionsAsync())
            {
                Console.WriteLine("Permissions needed : closed");
                return;
            }

            if (bleState)
            {
                try
                {
                    var list = adapter.GetSystemConnectedOrPairedDevices(new Guid[] {ble.SERVER_GUID});
                    await adapter.ConnectToDeviceAsync(list[0]);
                   
                }
                catch (DeviceConnectionException err)
                {
                    conErrDelay += 3000;
                    Console.WriteLine($" Device Conn Fail : wait {conErrDelay/1000}s , Reconnect");
                    Task t = Task.Run(async ()=>
                    {
                        await Task.Delay(conErrDelay);
                        await ConnectToDevice();
                    });
                    
                }

                conErrDelay = 0;
                return;
            }
            Console.WriteLine("Bluetooth is not connected");

        }

        public async void StopDataSend()
        {
            if (chara!=null)
            {
                try
                {
                    await chara.WriteAsync(Encoding.Default.GetBytes("stop"));

                }
                catch (Exception e)
                {
                    Console.WriteLine($"could not stop advertising {e}");
                }
            }
        }
    }
}