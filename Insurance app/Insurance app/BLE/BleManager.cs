using System;
using System.Text;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using System.Threading;
using Plugin.BLE.Abstractions.Exceptions;
using Exception = System.Exception;

namespace Insurance_app.BLE
{
    public class BleManager
    {


        private readonly IAdapter adapter;
        public Ble ble;

        private readonly EventHandler<byte[]> readCompleted;
        private ICharacteristic chara=null;
        private int serviceDelay = 0;
        private int readingDelay = 0;
        private int connectionErrDelay = 0;
        private bool bleState = false;

        public EventHandler<string> InferEvent;

        public BleManager()
        {
            ble = new Ble();
            adapter = CrossBluetoothLE.Current.Adapter;
            RegisterEventHandlers();
            bleState=ble.BleCheck();
            ConnectToDevice();//<-------------------------will be different later
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

        private async void  ReadAsync()
        {
            Console.WriteLine("Reading...");
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
                       ReadAsync();
                   });
                   return;
               }
               readingDelay = 0;
               //Console.WriteLine("Read complete, values are : >"+str);
               InferEvent?.Invoke(this,str);
               ReadAsync();
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                connectionErrDelay += 3000;
                Console.WriteLine($"[read exception] wait {connectionErrDelay/1000}s: reconnect to device");

                Task t = Task.Run(async () =>
                {
                    await Task.Delay(connectionErrDelay);
                    ConnectToDevice();
                    
                });
            }
        }
        

        private async void GetCharaAsync(IService service)
        {
            try
            {
                chara = await service.GetCharacteristicAsync(ble.SERVER_GUID);
                if (chara!=null)
                {
                    ReadAsync();
                }
                else
                {
                    ConnectToDevice();
                }

            }
            catch (Exception e)
            {
                ConnectToDevice();
                Console.WriteLine($"GetCharacteristic error : {e}");
            }
            
        }

        private async void ConnectToService(IDevice device)
        {
            
                try
                {
                    var service = await device.GetServiceAsync(ble.SERVER_GUID);
                    if (service != null)
                    {
                        Console.WriteLine("Service Found");
                        serviceDelay = 0;
                        GetCharaAsync(service);
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
                    ConnectToDevice();
                }
            
        }
        
        private async void ConnectToDevice()
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
                    connectionErrDelay += 3000;
                    Console.WriteLine($" Device Conn Fail : wait {connectionErrDelay/1000}s , Reconnect");
                    Task t = Task.Run(async ()=>
                    {
                        await Task.Delay(connectionErrDelay);
                        ConnectToDevice();
                    });
                    
                }

                connectionErrDelay = 0;
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
                    Console.WriteLine(e);
                    throw;
                }
            }
        }
    }
}