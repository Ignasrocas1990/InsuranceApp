using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Util;
using Java.Util;
using String = System.String;

namespace watch
{
    public class BleServer
    {
        private const string DefaultUuid = "a3bb5442-5b61-11ec-bf63-0242ac130002";
        public const string TAG = "mono-stdout";
        private readonly UUID SERVER_UUID;
        private BluetoothManager _bltManager;
        private BluetoothAdapter _bltAdapter;
        public BleServerCallback BltCallback;
        private BluetoothGattServer _bltServer;
        private BluetoothGattCharacteristic _bltCharac;
        private readonly BluetoothLeAdvertiser _bltAdvertiser;
        public Queue<String> SensorData;

        private SensorManager _sensorManager;
        
        //public event EventHandler ToggleSensorsEventHandler;
        private readonly BleAdvertiseCallback _bltAdvertiserCallback;
        
        public BleServer(Context context )
        {
            SensorData = new Queue<String>();
            SERVER_UUID = GetUUID(DefaultUuid);
            CreateServer(context);
            //BltCallback.dataRecievedNotifier += (s,e) => { dataRecieved = true; };
            BltCallback.ReadHandler += SendData;
            _bltAdvertiserCallback = new BleAdvertiseCallback();
            _bltAdvertiser = _bltAdapter.BluetoothLeAdvertiser;
            StartAdvertising();


            _sensorManager = new SensorManager();
        }
        public void SendData(object s, BleEventArgs e)
        {
            var stringValue = " ";
            if (SensorData.Count>2)
            {
                stringValue =  SensorData.Dequeue()+" "+SensorData.Dequeue(); 
            }
            e.Characteristic.SetValue(stringValue);
            _bltServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Characteristic.GetValue() ?? throw new InvalidOperationException());
            _bltServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
        }
        private void CreateServer(Context context)
        {
            _bltManager = (BluetoothManager)context.GetSystemService(Context.BluetoothService);
            if (_bltManager != null)
            {
                _bltAdapter = _bltManager.Adapter;

                BltCallback = new BleServerCallback();
                _bltServer = _bltManager.OpenGattServer(context, BltCallback);
            }

            var service = new BluetoothGattService(SERVER_UUID, GattServiceType.Primary);
            _bltCharac = new BluetoothGattCharacteristic(SERVER_UUID, GattProperty.Read| GattProperty.Write | GattProperty.Notify ,
                GattPermission.Read | GattPermission.Write);
            var descriptor = new BluetoothGattDescriptor(SERVER_UUID, GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            _bltCharac.AddDescriptor(descriptor);

            service.AddCharacteristic(_bltCharac);

            if (_bltServer != null) _bltServer.AddService(service);
        }
        private void StartAdvertising()
        {
            var builder = new AdvertiseSettings.Builder()
            .SetAdvertiseMode(AdvertiseMode.LowLatency)
            ?.SetConnectable(true)
            ?.SetTxPowerLevel(AdvertiseTx.PowerHigh);

            AdvertiseData.Builder dataBuilder = new AdvertiseData.Builder()
            .SetIncludeDeviceName(true)
            ?.SetIncludeTxPowerLevel(true);

            if (builder != null && dataBuilder != null)
                _bltAdvertiser.StartAdvertising(builder.Build(), dataBuilder.Build(), _bltAdvertiserCallback);
        }
        private UUID GetUUID(string uuid) => UUID.FromString(uuid);
        public void StopAdvertising()
        {
            if (_bltAdvertiser != null)
            {
                _bltAdvertiser.StopAdvertising(_bltAdvertiserCallback);
            }
        }
    }
    public class BleAdvertiseCallback : AdvertiseCallback
    {
        public override void OnStartFailure(AdvertiseFailure errorCode)
        {
            Log.Verbose(BleServer.TAG, $"advertise err : {errorCode}");

            base.OnStartFailure(errorCode);
        }

        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            Log.Verbose(BleServer.TAG, "Advertise started ");

            base.OnStartSuccess(settingsInEffect);
        }
        
    }
}