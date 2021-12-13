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
{        /*
        private IReadOnlyList<IDevice> list;
        private ConnectParameters cancellationToken;
        private ICharacteristic crst;
        private EventHandler<IReadOnlyList<IService>> serviceHandler;
        private EventHandler<IReadOnlyList<ICharacteristic>> crstHandler;
        private EventHandler startCrst;
        private IReadOnlyList<IService> service;
        private EventHandler<Byte> readData;
        //cancellationToken = new ConnectParameters(true, false);
        */
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
            else
            {

            }
            adapter.DeviceConnected += async (s, e) =>
             {
                 var d = e.Device;
                 ICharacteristic chara = null;
                 Console.WriteLine("-----------------------deviceHander : "+ d.Name);
                 var service = await d.GetServiceAsync(SERVER_GUID);
                 if (service != null)
                 {
                     Console.WriteLine("-----------Got service");

                     chara = await service.GetCharacteristicAsync(SERVER_GUID);
                 }
                 if (chara != null)
                 {
                     Console.WriteLine("-----------Got charasterstic");

                     read(chara);
                 }
             };
            readCompleted += (s, e) =>
            {
                string str = Encoding.Default.GetString(bytes);
                Console.WriteLine("--------------------- read complete, values are : >"+str);
            };
        }
        public async void connectToKnow(object sender, EventArgs e)
        {
            try
            {
                Guid[] temp = { SERVER_GUID };
                IReadOnlyList<IDevice> list = adapter.GetSystemConnectedOrPairedDevices(temp);
                await adapter.ConnectToDeviceAsync(list[0]);
                   
                //deviceHandler?.Invoke(this, device);
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









        //-----------------------------------------------------------------------support methods
        private async Task<bool> getPremissionsAsync()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            if (locationPermissionStatus != PermissionStatus.Granted)
            {
                var status = await Permissions.RequestAsync<Permissions.LocationAlways>();
                return status == PermissionStatus.Granted;
            }
            return true;
        }
        private void alert(string title,string msg, string btn)
        {
            MainThread.BeginInvokeOnMainThread(() => this.DisplayAlert(title, msg, btn));

        }
        private Guid Getguid(string uuid) => Guid.Parse(uuid);
    }
}
