using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;
using System.Threading;
using Plugin.BLE.Abstractions.Exceptions;

using Plugin.BLE.Abstractions.EventArgs;
using Exception = System.Exception;
using String = System.String;
using Thread = System.Threading.Thread;
using Google.Cloud.AIPlatform;
using Google.Cloud.AIPlatform.V1;
using Google.Protobuf;

namespace Insurance_app
{        
    public partial class MainPage : ContentPage
    {
        private readonly Guid SERVER_GUID;
        private const string uuidString = "a3bb5442-5b61-11ec-bf63-0242ac130002";

        private readonly IBluetoothLE ble;
        private readonly IAdapter adapter;


        //event EventHandler<IDevice> deviceHandler;
        //IReadOnlyList<IDevice> list;
        //private IDevice device = null;
        //private Queue<String> recievedData;
       // private byte[] bytes;
        private readonly EventHandler<byte[]> readCompleted;
        private bool canRead = false;
        private CancellationToken cancelT;
        int i = 0;
        private ICharacteristic chara=null;
        private int serviceDelay = 0;

        public MainPage()
        {
            InitializeComponent();
            SERVER_GUID = Getguid(uuidString);
            
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            //check if bluetooth is on
            if (ble.IsAvailable == false)
            {
                Alert("error","type of bluetooth not available","close");
            }
            BleCheck();
            ble.StateChanged += Ble_StateChanged;




            
            if (adapter != null)
            {
                Connect();
            }
            
            
            
            readCompleted += (s, e) =>
            {
                string str = " ";
                str = Encoding.Default.GetString(e);
                if (str.Equals(" "))
                {
                    Console.WriteLine("reading empty : wait 3sec > read again");
                    Task t = Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        ReadAsync();
                    });
                }
                else
                {
                    Console.WriteLine("Read complete, values are : >"+str);
                    ReadAsync();
                }
                
            };
        }
        private async void ReadAsync()
        {
            byte[] bytes = null;
            try
            {
                if (!(chara is null))
                {
                    Console.WriteLine("Reading ...");
                    bytes = await chara.ReadAsync(cancelT);
                    readCompleted?.Invoke(this, bytes);
                }
                else
                {
                    Console.WriteLine("characteristic is null, wait 3sec : retry to connect");
                    Task t = Task.Run(async () =>
                    {
                        await Task.Delay(3000);
                        ConnectToKnow();
                    });
                }
                
            }
            catch (Exception e)
            {
                
                Console.WriteLine("[read fail] we wait 3s: "+e.Message);
                Thread.Sleep(2000);
                canRead = false;
                BleCheck();
              
            }
            
        }
        public async void ConnectToKnow()
        {
            if (!await GetPremissionsAsync())
            {
                Alert("notice","Permissions needed","close");
                return;
            }
            try
            {
                Guid[] temp = { SERVER_GUID };
                IReadOnlyList<IDevice> list = adapter.GetSystemConnectedOrPairedDevices(temp);
                await adapter.ConnectToDeviceAsync(list[0]);
                   
            }
            catch (DeviceConnectionException err)
            {

                Console.WriteLine($" error, Cant connect to device {err.Message}");
            }

        }
        private void Ble_StateChanged(object sender, BluetoothStateChangedArgs e)
        {
            Console.WriteLine("State changed -----"+e.NewState);
            BleCheck();
        }

        public void Connect()
        {
            adapter.DeviceConnected += async (s, e) =>
            {
                //ICharacteristic chara = null;
                var device = e.Device;
                
                Console.WriteLine("-----------------------device Found : " + device.Name);
                var service = await device.GetServiceAsync(SERVER_GUID);
                if (service != null)
                {
                    serviceDelay = 0;
                    Console.WriteLine(" service found ");

                    chara = await service.GetCharacteristicAsync(SERVER_GUID);
                    if (chara != null)
                    {
                        serviceDelay = 0;
                        ReadAsync();
                    }
                    else
                    {
                        Console.WriteLine("chara null, Waiting 3s: Reconnect");
                        Task t = Task.Run(async () =>
                        {
                            
                            await Task.Delay(3000);
                            ConnectToKnow();
                        });
                    }

                }
                else
                {
                    serviceDelay += 3000;
                    Console.WriteLine($"Service not found,Wait {serviceDelay/1000} ms : reconnect ");
                    Task t = Task.Run(async () =>
                    {
                        await Task.Delay(serviceDelay);
                        ConnectToKnow();
                    });
                }
               

            };
        }
        //------------------------------------------------------------------------ blue-tooth on

        public void BleCheck()
        {
            Console.WriteLine("Checking Ble...");
            if (!ble.IsAvailable)
            {
                 Alert("Error", "Bluetooth LE is not available", "close");
               // ConnectBtn.IsEnabled = false;
            }
            else if (!ble.IsOn && ble.State != BluetoothState.TurningOn && ble.State != BluetoothState.TurningOff)
            {
                 Alert("Error", "Please turn on the Bluetooth", "close");
                 //ConnectBtn.IsEnabled = false;
            }
            else
            {
                ConnectToKnow();
                //ConnectBtn.IsEnabled = true;
                
            }
        }



        //-----------------------------------------------------------------------support methods
        private async Task<bool> GetPremissionsAsync()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            var sensorsPermission =  await Permissions.CheckStatusAsync<Permissions.Sensors>();
            var granted = PermissionStatus.Granted;
            if (locationPermissionStatus == granted && sensorsPermission == granted) return true;
            
            var locStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
            var sensorStatus =await Permissions.RequestAsync<Permissions.Sensors>();
            return (locStatus == granted && sensorStatus == granted);
        }
        private void Alert(string title,string msg, string btn)
        {
            MainThread.BeginInvokeOnMainThread(() => DisplayAlert(title, msg, btn));

        }
        private Guid Getguid(string uuid) => Guid.Parse(uuid);

        private void Button_OnClicked(object sender, EventArgs e)
        {
            
            // ReSharper disable once HeapView.BoxingAllocation
            Button.Text = $"Click {i++}";
            ConnectToKnow();
            
        }

        private void test(object sender, EventArgs e)
        {
            testBtn.Text = $"{i++}";
        }

        private async void StopBtn_OnClicked(object sender, EventArgs e)
        {
            if (chara!=null)
            {
                 await chara.WriteAsync(Encoding.Default.GetBytes("stop"));
            }
        }
    }
}
