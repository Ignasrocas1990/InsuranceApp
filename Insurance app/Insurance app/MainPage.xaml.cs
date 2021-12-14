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
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions;
using Java.Util;
using Plugin.BLE.Abstractions.EventArgs;

namespace Insurance_app
{        
    public partial class MainPage : ContentPage
    {
        private string TAG = "BleClient";
        private Guid SERVER_GUID;
        private const string uuidString = "a3bb5442-5b61-11ec-bf63-0242ac130002";

        private IBluetoothLE ble;
        private IAdapter adapter;


        event EventHandler<IDevice> deviceHandler;
        IReadOnlyList<IDevice> list;
        private IDevice device;
        private byte[] bytes;
        private EventHandler readCompleted;

        public MainPage()
        {
            InitializeComponent();
            SERVER_GUID = Getguid(uuidString);
            
            ble = CrossBluetoothLE.Current;
            adapter = CrossBluetoothLE.Current.Adapter;

            //check if bluetooth is on
            if (ble.IsAvailable == false)
            {
                alert("erro","type of bluetooth not available","close");
            }

            bleCheck();
            ble.StateChanged += Ble_StateChanged;



            readCompleted += (s, e) =>
            {
                string str = Encoding.Default.GetString(bytes);
                alert("message", str, "close");
                Console.WriteLine("--------------------- read complete, values are : >"+str);
            };
        }

        private void Ble_StateChanged(object sender, BluetoothStateChangedArgs e)
        {
            Console.WriteLine("State changed -----"+e.NewState);
            bleCheck();


        }

        public async void connect()
        {
            adapter.DeviceConnected += async (s, e) =>
            {
                var d = e.Device;
                ICharacteristic chara = null;
                Console.WriteLine("-----------------------deviceHander : " + d.Name);
                var service = await d.GetServiceAsync(SERVER_GUID);
                if (service != null)
                {
                    Console.WriteLine("-----------Got service");

                    chara = await service.GetCharacteristicAsync(SERVER_GUID);
                }
                else
                {
                    alert("error", "error to get service", "close");
                }
                if (chara != null)
                {
                    chara.ValueUpdated += (se, values) =>
                    {
                        read(values.Characteristic);
                    };
                    Console.WriteLine("-----------Got charasterstic");

                    read(chara);
                }

            };
        }
        public async void connectToKnow(object sender, EventArgs e)
        {
            if (!await getPremissionsAsync())
            {
                alert("notice","Premissions needed","close");
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

        



        private async void read(ICharacteristic chara)
        {
            bytes = await chara.ReadAsync();
            readCompleted?.Invoke(this, EventArgs.Empty);
        }

        //------------------------------------------------------------------------ bluetooth on

        public void bleCheck()
        {
            if (!ble.IsAvailable)
            {
                 alert("Error", "Bluetooth LE is not available", "close");
                ConnectBtn.IsEnabled = false;
            }
            else if (!ble.IsOn && ble.State != BluetoothState.TurningOn && ble.State != BluetoothState.TurningOff)
            {
                 alert("Error", "Please turn on the Bluetooth", "close");
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
            var granted = PermissionStatus.Denied;
            if (locationPermissionStatus != granted || sensorsPermission != granted)
            {
                var locStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
                var sensorStatus =await Permissions.RequestAsync<Permissions.Sensors>();
                return (locStatus != granted && sensorStatus != granted);
            }

            
            return true;
        }
        private void alert(string title,string msg, string btn)
        {
            MainThread.BeginInvokeOnMainThread(() => DisplayAlert(title, msg, btn));

        }
        private Guid Getguid(string uuid) => Guid.Parse(uuid);
    }
}
