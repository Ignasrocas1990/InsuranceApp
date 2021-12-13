using System;
using System.Diagnostics;
using System.Linq;

using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Util;
using Java.Util;
using Random = System.Random;

namespace watch
{
    public class BleServer
    {

        private string TAG = "BleServer";
        private UUID SERVER_UUID;



        //private Context context;
        private BluetoothManager bltManager;
        private BluetoothAdapter bltAdapter;
        private BleServerCallback bltCallback;
        private BluetoothGattServer bltServer;
        private BluetoothGattCharacteristic bltCharac;
        private BluetoothLeAdvertiser bltAdvertiser;
        private const string defaultUUID = "a3bb5442-5b61-11ec-bf63-0242ac130002";
        public string dataToSend = "empty";


        public BleServer(Context context,string uuid)
        {
            if (uuid == "")
            {
                SERVER_UUID = GetUUID(defaultUUID);
            }
            else
            {
                SERVER_UUID = GetUUID(uuid);
            }

            createServer(context);

            //Use notificationHandler to see if data recieved before sending different one (after recieve turn off service)
            bltCallback.dataRecievedNotifier += (s,e) =>
            {
                Log.Debug(TAG,e.GattStatus.ToString()+" data recieved , device:  "+e.Device.Name);

            };
            bltCallback.readHandler += sendData;

            bltAdvertiser = bltAdapter.BluetoothLeAdvertiser;
            startAdvertising();
        }
        public void sendData(object s, BleEventArgs e)
        {
               // SensorManager sensorManager = new SensorManager();
                
                e.Characteristic.SetValue(dataToSend);
                bltServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset,e.Characteristic.GetValue());
                bltServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
                Log.Info(TAG, "---------------> Data sent");

        }
        private void createServer(Context context)
        {
            bltManager = (BluetoothManager)context.GetSystemService(Context.BluetoothService);
            bltAdapter = bltManager.Adapter;

            bltCallback = new BleServerCallback();
            bltServer = bltManager.OpenGattServer(context, bltCallback);

            var service = new BluetoothGattService(SERVER_UUID, GattServiceType.Primary);
            bltCharac = new BluetoothGattCharacteristic(SERVER_UUID, GattProperty.Read | GattProperty.Notify,
                GattPermission.Read);
            var descriptor = new BluetoothGattDescriptor(SERVER_UUID, GattDescriptorPermission.Read);
            bltCharac.AddDescriptor(descriptor);

            service.AddCharacteristic(bltCharac);

            bltServer.AddService(service);
        }
        private void startAdvertising()
        {
            var builder = new AdvertiseSettings.Builder()
            .SetAdvertiseMode(AdvertiseMode.LowLatency)
            .SetConnectable(true)
            //.SetTimeout(0)
            .SetTxPowerLevel(AdvertiseTx.PowerHigh);

            AdvertiseData.Builder dataBuilder = new AdvertiseData.Builder()
            .SetIncludeDeviceName(true)
            .SetIncludeTxPowerLevel(true);

            bltAdvertiser.StartAdvertising(builder.Build(), dataBuilder.Build(), new BleAdvertiseCallback());
        }
        private UUID GetUUID(string uuid) => UUID.FromString(uuid);

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