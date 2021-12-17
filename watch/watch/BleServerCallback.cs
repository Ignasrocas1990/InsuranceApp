using System;

using Android.Bluetooth;

namespace watch
{
    public class BleEventArgs : EventArgs
    {
        public BluetoothDevice Device { get; set; }
        public GattStatus GattStatus { get; set; }
        public BluetoothGattCharacteristic Characteristic { get; set; }
        public byte[] Value { get; set; }
        public int RequestId { get; set; }
        public int Offset { get; set; }
    }

    public class BleServerCallback : BluetoothGattServerCallback
    {

        public event EventHandler<BleEventArgs> dataRecievedNotifier;
        public event EventHandler<BleEventArgs> readHandler;
        public EventHandler DisconectedHandler;

        public BleServerCallback() { }


        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic chara)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, chara);
            
            if (readHandler != null) 
                readHandler(this, createArgs(device, chara, requestId, offset));
        }

        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
            if (newState == ProfileState.Disconnected)
            {
                // stop sensor data from queuing
                DisconectedHandler?.Invoke(this,EventArgs.Empty);
            }
            Console.WriteLine($"State changed to : {newState}");
            

        }

        public override void OnNotificationSent(BluetoothDevice device, GattStatus status)
        {
            base.OnNotificationSent(device, status);

            if (dataRecievedNotifier != null)
            {
                dataRecievedNotifier(this, new BleEventArgs() { Device = device,GattStatus = status });
            }
        }
        private BleEventArgs createArgs(BluetoothDevice device, BluetoothGattCharacteristic chara, int requestId, int offset)
        {
            return new BleEventArgs() { Device = device, Characteristic = chara, RequestId = requestId, Offset = offset };
        }


    }
}