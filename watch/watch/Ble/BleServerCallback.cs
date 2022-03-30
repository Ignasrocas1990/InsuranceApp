/*
    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using Android.Bluetooth;
using Android.Util;

namespace watch.Ble
{
    /// <summary>
    /// Implementation of BluetoothGattServerCallback
    /// Which is used for communication.
    /// (Like Broadcast receiver it listens to incoming requests to the server)
    /// ( from Client => bluetooth connection => (BleServerCallback) => server)
    /// </summary>
    public class BleServerCallback : BluetoothGattServerCallback
    {
        private const string Tag = "mono-stdout";

        public event EventHandler<BleEventArgs> ReadHandler;
        public event EventHandler<ConnectEventArgs> StateHandler;
        public event EventHandler<BleEventArgs> DataWriteHandler;
        

        public BleServerCallback() { }

        public override void OnCharacteristicReadRequest(BluetoothDevice device, int requestId, int offset,
            BluetoothGattCharacteristic chara)
        {
            base.OnCharacteristicReadRequest(device, requestId, offset, chara);
            Log.Verbose(Tag,"Got ReadRequest in BleServerCallback");
            ReadHandler?.Invoke(this, createArgs(device, chara, requestId, offset));
        }
        public override void OnConnectionStateChange(BluetoothDevice device, ProfileState status, ProfileState newState)
        {
            base.OnConnectionStateChange(device, status, newState);

            switch (newState)
            {
                case ProfileState.Disconnected:
                    Log.Verbose(Tag, $"State changed to : {newState}");

                    StateHandler?.Invoke(this,new ConnectEventArgs(){State ="Disconnected"});
                    break;
                case ProfileState.Connected:
                    Log.Verbose(Tag, $"State changed to : {newState}");
                    StateHandler?.Invoke(this,new ConnectEventArgs(){State ="Connected"});
                    break;
            }
            Log.Verbose(Tag, $"State changed to : {newState}");
        }

        public override void OnCharacteristicWriteRequest(BluetoothDevice device, int requestId, BluetoothGattCharacteristic characteristic,
            bool preparedWrite, bool responseNeeded, int offset, byte[] value)
        {
            base.OnCharacteristicWriteRequest(device, requestId, characteristic, preparedWrite, responseNeeded, offset, value);
            DataWriteHandler?.Invoke(this, new BleEventArgs()
            {
                Device = device, Characteristic = characteristic, Value = value, RequestId = requestId, Offset = offset
            });
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
        public BluetoothGattCharacteristic Characteristic { get; set; }
        public byte[] Value { get; set; }
        public int RequestId { get; set; }
        public int Offset { get; set; }
    }
}