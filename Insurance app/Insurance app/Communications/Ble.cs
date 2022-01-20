using System;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;
using String = System.String;

namespace Insurance_app.BLE
{
    public class Ble
    {
        public IBluetoothLE ble;
        public Guid SERVER_GUID { get; set; }
        private const string uuidString = "a3bb5442-5b61-11ec-bf63-0242ac130002";
        private readonly Func<String,Guid> setGuid = s => Guid.Parse(s);
        public Ble()
        {
            ble = CrossBluetoothLE.Current;
            SERVER_GUID = setGuid(uuidString);
        }
        public bool BleCheck()
        {
            if (ble.IsOn || ble.State == BluetoothState.TurningOn)
            {
                Console.WriteLine("Bluetooth is on");
                return true;
            }

            Console.WriteLine("ble is off");
            return false;
        }
        public async Task<bool> GetPremissionsAsync()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            var sensorsPermission =  await Permissions.CheckStatusAsync<Permissions.Sensors>();
            var granted = PermissionStatus.Granted;
            if (locationPermissionStatus == granted && sensorsPermission == granted) return true;
            
            var locStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
            var sensorStatus =await Permissions.RequestAsync<Permissions.Sensors>();
            return (locStatus == granted && sensorStatus == granted);
        }
        public bool IsAvailable() => ble.IsAvailable;
    }
}