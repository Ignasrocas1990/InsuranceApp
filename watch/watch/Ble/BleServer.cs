using System;
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Util;
using Java.Lang;
using Java.Util;
using Plugin.BLE.Abstractions.Contracts;
using watch.Sensors;
using Exception = System.Exception;
using String = System.String;

namespace watch.Ble
{
    public class BleServer : IDisposable
    {
        private const string DefaultUuid = "a3bb5442-5b61-11ec-bf63-0242ac130002";
        public const string TAG = "mono-stdout";
        private readonly UUID serverUuid;
        private BluetoothManager bltManager;
        private BluetoothAdapter bltAdapter;
        public BleServerCallback BltCallback;
        private BluetoothGattServer bltServer;
        private BluetoothGattCharacteristic bltCharac;
        private readonly BluetoothLeAdvertiser bltAdvertiser;
        public readonly Queue<string> SensorData;

        
        //public event EventHandler ToggleSensorsEventHandler;
        private readonly BleAdvertiseCallback bltAdvertiserCallback;
        private BluetoothGattService service;

        public BleServer(Context context )
        {
            
            SensorData = new Queue<string>();
            serverUuid = GetUUID(DefaultUuid);
            CreateServer(context);
            BltCallback.ReadHandler += SendData;
            bltAdvertiserCallback = new BleAdvertiseCallback();
            bltAdvertiser = bltAdapter.BluetoothLeAdvertiser;
            StartAdvertising();

        }

        public void SendData(object s, BleEventArgs e)
        {
            var data = " ";
            if (SensorData.Count > 0)
            {
                data =  SensorData.Dequeue();
            }
            e.Characteristic.SetValue(data);
            bltServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Characteristic.GetValue() ?? throw new InvalidOperationException());
            bltServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
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

            service = new BluetoothGattService(serverUuid, GattServiceType.Primary);
            bltCharac = new BluetoothGattCharacteristic(serverUuid, 
                GattProperty.Read| GattProperty.Write | GattProperty.Notify ,
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
                    if (service != null)
                    {
                        if (service.Characteristics !=null)
                        {
                            service.Characteristics.Clear();
                        }
                    }
                    if (bltServer.Services != null) bltServer.Services.Clear();
                    BltCallback.Dispose();
                }
                catch (Exception e)
                {
                    Log.Verbose(TAG, $"Fail to stop the server...{e}");
                }
                
            }
        }

        public void Dispose()
        {
            serverUuid?.Dispose();
            bltManager?.Dispose();
            bltAdapter?.Dispose();
            BltCallback?.Dispose();
            bltServer?.Dispose();
            bltCharac?.Dispose();
            bltAdvertiser?.Dispose();
            bltAdvertiserCallback?.Dispose();
            service?.Dispose();
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