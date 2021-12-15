using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;
using System.Diagnostics;
using System.Threading;
using Android.Bluetooth;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions;
using Java.Util;
using Plugin.BLE.Abstractions.EventArgs;

namespace Insurance_app
{        
    public partial class MainPage : ContentPage
    {
        private string TAG = "BleClient";
        private readonly Guid SERVER_GUID;
        private const string uuidString = "a3bb5442-5b61-11ec-bf63-0242ac130002";

        private readonly IBluetoothLE ble;
        private readonly IAdapter adapter;


        event EventHandler<IDevice> deviceHandler;
        IReadOnlyList<IDevice> list;
        private IDevice device = null;
        private byte[] bytes;
        private readonly EventHandler readCompleted;
        private ICharacteristic chara = null;
        private bool canRead = false;
        private CancellationToken cancelT;
        private readonly Action readCanceledCallback = delegate { };

        public MainPage()
        {
            InitializeComponent();
            SERVER_GUID = Getguid(uuidString);
            
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;
            cancelT = new CancellationToken(false);
            cancelT.Register(readCanceledCallback);
            readCanceledCallback += () =>
            {
                Console.WriteLine("------------------------------------ read Cancellation token called");
            };
            //check if bluetooth is on
            if (ble.IsAvailable == false)
            {
                Alert("error","type of bluetooth not available","close");
            }

            BleCheck();
            ble.StateChanged += Ble_StateChanged;



            readCompleted += (s, e) =>
            {
                string str = Encoding.Default.GetString(bytes);
                Alert("message", str, "close");
                Console.WriteLine("--------------------- Read complete, values are : >"+str);
            };
            if (adapter != null)
            {
                Connect();
            }
        }

        private void Chara_ValueUpdated(object sender, CharacteristicUpdatedEventArgs e)
        {
            Console.WriteLine("Value has been updated ---------------------------");
            Read(e.Characteristic);
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
                device = e.Device;
                
                Console.WriteLine("-----------------------deviceHander : " + device.Name);
                var service = await device.GetServiceAsync(SERVER_GUID);
                if (service != null)
                {
                    Console.WriteLine("-----------Got service");

                    chara = await service.GetCharacteristicAsync(SERVER_GUID);
                }
                else
                {
                    Alert("error", "error to get service", "close");
                }
                if (chara != null)
                {
                    Console.WriteLine("-----------Got characteristic");
                    canRead = chara.CanRead;
                    Read(chara);
                }

            };
        }

        public async void ConnectToKnow(object sender, EventArgs e)
        {
            if (!await getPremissionsAsync())
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

                Console.WriteLine("----------------error connecting device :"+err.Message);
            }

        }

        



        private async void Read(ICharacteristic chara)
        {
            try
            {
                while (canRead)
                {
                    bytes = await chara.ReadAsync(cancelT);
                    Thread.Sleep(1000);
                    readCompleted?.Invoke(this, EventArgs.Empty);
                }
            }
            catch (Exception e)
            {
                canRead = false;
                Console.WriteLine("------------------------------read fail : "+e.Message);
              
            }
            
        }

        //------------------------------------------------------------------------ blue-tooth on

        public void BleCheck()
        {
            if (!ble.IsAvailable)
            {
                 Alert("Error", "Bluetooth LE is not available", "close");
                ConnectBtn.IsEnabled = false;
            }
            else if (!ble.IsOn && ble.State != BluetoothState.TurningOn && ble.State != BluetoothState.TurningOff)
            {
                 Alert("Error", "Please turn on the Bluetooth", "close");
                 ConnectBtn.IsEnabled = false;
            }
            else
            {
                ConnectBtn.IsEnabled = true;
            }
        }

        //-----------------------------------------------------------------------support methods
        private async Task<bool> getPremissionsAsync()
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
    }
}
