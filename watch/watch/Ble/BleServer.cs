using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Util;
using Java.Util;
using watch.Sensors;
using String = System.String;

namespace watch.Ble
{
    public class BleServer
    {
        private const string DefaultUuid = "a3bb5442-5b61-11ec-bf63-0242ac130002";
        public const string TAG = "mono-stdout";
        private readonly UUID serverUuid;
        private BluetoothManager bltManager;
        private BluetoothAdapter bltAdapter;
        public BleServerCallback BltCallback;
        private BluetoothGattServer bltServer;
        private BluetoothGattCharacteristic bltCharac;
        private BluetoothLeAdvertiser bltAdvertiser;
        public Queue<String> SensorData;

        private SensorManager sensorManager;
        
        //public event EventHandler ToggleSensorsEventHandler;
        private readonly BleAdvertiseCallback bltAdvertiserCallback;
        
        public BleServer(Context context )
        {
            SensorData = new Queue<String>();
            serverUuid = GetUUID(DefaultUuid);
            CreateServer(context);
            //BltCallback.dataRecievedNotifier += (s,e) => { dataRecieved = true; };
            BltCallback.ReadHandler += SendData;
            bltAdvertiserCallback = new BleAdvertiseCallback();
            bltAdvertiser = bltAdapter.BluetoothLeAdvertiser;
            StartAdvertising();


            sensorManager = new SensorManager();
        }
        public void SendData(object s, BleEventArgs e)
        {
            var stringValue = " ";
            if (SensorData.Count>=2)
            {
               
                stringValue =   PrepareData(SensorData.Dequeue(),SensorData.Dequeue());
                Log.Verbose(TAG,stringValue);
            }
            e.Characteristic.SetValue(stringValue);
            bltServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Characteristic.GetValue() ?? throw new InvalidOperationException());
            bltServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
        }

        private String PrepareData(String first,String second)
        {
            if (first[0].Equals('A') && second[0].Equals('G'))
            {
                return $"{first.Remove(0, 2)}{second.Remove(0, 1)}";
            }
            else if (first[0].Equals('G') && second[0].Equals('A'))
            {
                return $"{second.Remove(0, 2)}{first.Remove(0, 1)}";
            }
            return " "; 
        }
        private void CreateServer(Context context)
        {
            bltManager = (BluetoothManager)context.GetSystemService(Context.BluetoothService);
            if (bltManager != null)
            {
                bltAdapter = bltManager.Adapter;

                BltCallback = new BleServerCallback();
                bltServer = bltManager.OpenGattServer(context, BltCallback);
            }

            var service = new BluetoothGattService(serverUuid, GattServiceType.Primary);
            bltCharac = new BluetoothGattCharacteristic(serverUuid, GattProperty.Read| GattProperty.Write | GattProperty.Notify ,
                GattPermission.Read | GattPermission.Write);
            var descriptor = new BluetoothGattDescriptor(serverUuid, GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            bltCharac.AddDescriptor(descriptor);

            service.AddCharacteristic(bltCharac);

            if (bltServer != null) bltServer.AddService(service);
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
                bltAdvertiser.StartAdvertising(builder.Build(), dataBuilder.Build(), bltAdvertiserCallback);
        }
        private UUID GetUUID(string uuid) => UUID.FromString(uuid);
        public void StopAdvertising()
        {
            if (bltAdvertiser != null)
            {
                try
                {
                    bltAdvertiser.StopAdvertising(bltAdvertiserCallback);
                    
                    bltAdvertiser = null;
                    bltServer = null;

                }
                catch (Exception e)
                {
                    Log.Verbose(TAG, "Fail to stop the server...");
                }
                
            }
        }
    }
    public class BleAdvertiseCallback : AdvertiseCallback
    {
        public override void OnStartFailure(AdvertiseFailure errorCode)
        {
            base.OnStartFailure(errorCode);
            Log.Verbose(BleServer.TAG, $"Advertise : Start error : {errorCode}");
        }

        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            base.OnStartSuccess(settingsInEffect);
            Log.Verbose(BleServer.TAG, "Advertise : Start Success ");
        }

    }
}