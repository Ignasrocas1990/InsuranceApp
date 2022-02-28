using System;
using Android.Bluetooth;
using Android.Util;

namespace watch.Ble
{
    public class BleServerCallback : BluetoothGattServerCallback
    {
        private const string TAG = "mono-stdout";

        public event EventHandler<BleEventArgs> ReadHandler;
        public event EventHandler<ConnectEventArgs> StateHandler;
        public event EventHandler<BleEventArgs> DataWriteHandler;

        public BleServerCallback() { }


        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic chara)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, chara);
            ReadHandler?.Invoke(this, createArgs(device, chara, requestId, offset));
        }
        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);

            switch (newState)
            {
                case ProfileState.Disconnected:
                    Log.Verbose(TAG, $"State changed to : {newState}");

                    StateHandler?.Invoke(this,new ConnectEventArgs(){State ="Disconnected"});
                    break;
                case ProfileState.Connected:
                    Log.Verbose(TAG, $"State changed to : {newState}");
                    StateHandler?.Invoke(this,new ConnectEventArgs(){State ="Connected"});
                    break;
            }
            Log.Verbose(TAG, $"State changed to : {newState}");
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic,
            bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);
            if (DataWriteHandler != null)
            {
                DataWriteHandler(this, new BleEventArgs()
                {
                    Device = device, Characteristic = characteristic, Value = value, RequestId = requestId, Offset = offset
                });
            }
        }
        public override void OnDescriptorWriteRequest(BluetoothDevice device, int requestId, BluetoothGattDescriptor descriptor,
            bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnDescriptorWriteRequest(device, requestId, descriptor, preparedWrite, responseNeeded, offset, value);
        }
        private BleEventArgs createArgs(BluetoothDevice device, BluetoothGattCharacteristic chara, int requestId, int offset)
        {
            return new BleEventArgs() { Device = device, Characteristic = chara, RequestId = requestId, Offset = offset };
        }


    }

    public class ConnectEventArgs : EventArgs
    {
        public string State { get; set; }
    }
    public class BleEventArgs : EventArgs
    {
        public BluetoothDevice Device { get; set; }
        public GattStatus GattStatus { get; set; }
        public BluetoothGattCharacteristic Characteristic { get; set; }
        public byte[] Value { get; set; }
        public int RequestId { get; set; }
        public int Offset { get; set; }
    }
}