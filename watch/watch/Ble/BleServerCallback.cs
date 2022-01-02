﻿using System;
using Android.Bluetooth;
using Android.Util;

namespace watch
{
    public class BleServerCallback : BluetoothGattServerCallback
    {
        private const string TAG = "mono-stdout";

        //public event EventHandler<BleEventArgs> DataRecievedNotifier;
        public EventHandler<BleEventArgs> DataWriteHandler;

        public event EventHandler<BleEventArgs> ReadHandler;
        public EventHandler DisconectHandler;
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
            DataWriteHandler?.Invoke(this, new BleEventArgs() { Device = device, Characteristic = characteristic, Value = value, RequestId = requestId, Offset = offset });
        }

        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);
            
            if (newState == ProfileState.Disconnected || newState == ProfileState.Connected)
            {
                Log.Verbose(TAG, $"State changed to : {newState}");

                DisconectHandler?.Invoke(this,EventArgs.Empty);
            }
            

        }
/*
        public override void OnNotificationSent(BluetoothDevice device, GattStatus status)
        {
            base.OnNotificationSent(device, status);

            if (DataRecievedNotifier != null)
            {
                DataRecievedNotifier(this, new BleEventArgs() { Device = device,GattStatus = status });
            }
        }
*/
        private BleEventArgs createArgs(BluetoothDevice device, BluetoothGattCharacteristic chara, int requestId, int offset)
        {
            return new BleEventArgs() { Device = device, Characteristic = chara, RequestId = requestId, Offset = offset };
        }


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