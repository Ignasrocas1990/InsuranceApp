using System;
using System.Collections.Concurrent;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Java.Lang;
using Java.Util;
using String = System.String;

namespace watch
{
    public class BleServer
    {

        private readonly UUID SERVER_UUID;
        

        //private Context context;
        private BluetoothManager bltManager;
        private BluetoothAdapter bltAdapter;
        public BleServerCallback BltCallback;
        private BluetoothGattServer bltServer;
        private BluetoothGattCharacteristic bltCharac;
        private readonly BluetoothLeAdvertiser bltAdvertiser;
        private const string defaultUUID = "a3bb5442-5b61-11ec-bf63-0242ac130002";
        public readonly ConcurrentQueue<String> SensorData;
        public bool dataRecieved = true;
        private string dequeValue;
        //public bool IsConnected=false;
        public event EventHandler ToggleSensorsEventHandler;
        BleAdvertiseCallback bltAdvertiserCallback;



        public BleServer(Context context,string uuid)
        {
            SensorData = new ConcurrentQueue<String>();
            if (uuid == "")
            {
                SERVER_UUID = GetUUID(defaultUUID);
            }
            else
            {
                SERVER_UUID = GetUUID(uuid);
            }

            CreateServer(context);
            //data recieved
            BltCallback.dataRecievedNotifier += (s,e) =>
            {
                dataRecieved = true;

            };
            BltCallback.readHandler += SendData;

            
            bltAdvertiserCallback = new BleAdvertiseCallback();
            bltAdvertiser = bltAdapter.BluetoothLeAdvertiser;
            StartAdvertising();
        }
        public void SendData(object s, BleEventArgs e)
        {
            dequeValue = "empty";
            
            /*
            if (!IsConnected)
            {
                //ToggleSensorsEventHandler?.Invoke(this,EventArgs.Empty);
                IsConnected = true;
                Thread.Sleep(100);
            }
            */
            
            if (!dataRecieved) return;
            if (!SensorData.IsEmpty)
            {
                SensorData.TryDequeue(out dequeValue);
                e.Characteristic.SetValue(dequeValue);
                bltServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Characteristic.GetValue() ?? throw new InvalidOperationException());
                bltServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
            }
            dataRecieved = false;
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

            var service = new BluetoothGattService(SERVER_UUID, GattServiceType.Primary);
            bltCharac = new BluetoothGattCharacteristic(SERVER_UUID, GattProperty.Read| GattProperty.Write | GattProperty.Notify ,
                GattPermission.Read | GattPermission.Write);
            var descriptor = new BluetoothGattDescriptor(SERVER_UUID, GattDescriptorPermission.Read | GattDescriptorPermission.Write);
            bltCharac.AddDescriptor(descriptor);

            service.AddCharacteristic(bltCharac);

            if (bltServer != null) bltServer.AddService(service);
        }
        private void StartAdvertising()
        {
            var builder = new AdvertiseSettings.Builder()
            .SetAdvertiseMode(AdvertiseMode.LowLatency)
            ?.SetConnectable(true)
            //.SetTimeout(0)
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
                bltAdvertiser.StopAdvertising(bltAdvertiserCallback);
            }
        }
    }
    public class BleAdvertiseCallback : AdvertiseCallback
    {
        public override void OnStartFailure(AdvertiseFailure errorCode)
        {
            Console.WriteLine("Adevertise start failure {0}", errorCode);
            base.OnStartFailure(errorCode);
        }

        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            Console.WriteLine("Adevertise start success {0}", settingsInEffect.Mode);
            base.OnStartSuccess(settingsInEffect);
        }
        
    }
}