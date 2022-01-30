using System;
using Android.Bluetooth;
using Android.Util;

namespace watch.Ble
{
    public class BleServerCallback : BluetoothGattServerCallback
    {
        private const string TAG = "mono-stdout";

        //public event EventHandler<BleEventArgs> DataRecievedNotifier;
        public EventHandler<BleEventArgs> DataWriteHandler;

        public event EventHandler<BleEventArgs> ReadHandler;
        public EventHandler<ConnectEventArgs> StateHandler;
        private bool firstTimeConnect = true;

        public BleServerCallback() { }


        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic chara)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, chara);
            ReadHandler?.Invoke(this, createArgs(device, chara, requestId, offset));
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic,
            bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);
            Log.Verbose(TAG, "onWrite request");
            if (firstTimeConnect)
            {
                Log.Verbose(TAG, "write connected");
                StateHandler?.Invoke(this,new ConnectEventArgs(){State ="Connected"});
                firstTimeConnect = false;
            }
            else
            {
                Log.Verbose(TAG, "write disconnected");

                firstTimeConnect = true;
                StateHandler?.Invoke(this,new ConnectEventArgs(){State ="Disconnected"});
            }
        }
        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
            if (firstTimeConnect) return;
  
            if (newState == ProfileState.Disconnected)
            {
                Log.Verbose(TAG, $"State changed to : {newState}");

                StateHandler?.Invoke(this,new ConnectEventArgs(){State ="Disconnected"});
                
            }else if (newState == ProfileState.Connected)
            {
                Log.Verbose(TAG, $"State changed to : {newState}");
                StateHandler?.Invoke(this,new ConnectEventArgs(){State ="Connected"});
            }
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