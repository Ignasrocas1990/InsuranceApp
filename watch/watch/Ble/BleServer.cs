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
using System.Collections.Generic;
using Android.Bluetooth;
using Android.Bluetooth.LE;
using Android.Content;
using Android.Util;
using Java.Util;

namespace watch.Ble
{
    /// <summary>
    /// Used to initialize a Bluetooth server that waits for connections
    /// so it can transmit the data 
    /// </summary>
    public class BleServer : IDisposable
    {
        private const string DefaultUuid = "a3bb5442-5b61-11ec-bf63-0242ac130002";
        public const string Tag = "mono-stdout";
        private UUID serverUuid;
        private BluetoothManager bltManager;
        private BluetoothAdapter bltAdapter;
        public BleServerCallback BltCallback;
        private BluetoothGattServer bltServer;
        private BluetoothGattCharacteristic bltCharac;
        private BluetoothLeAdvertiser bltAdvertiser;
        public Queue<string> SensorData;

        
        private readonly BleAdvertiseCallback bltAdvertiserCallback;
        private BluetoothGattService service;

        public BleServer(Context context )
        {
            
            SensorData = new Queue<string>();
            serverUuid = GetUuid(DefaultUuid);
            CreateServer(context);
            BltCallback.ReadHandler += SendData;
            bltAdvertiserCallback = new BleAdvertiseCallback();
            bltAdvertiser = bltAdapter.BluetoothLeAdvertiser;
            StartAdvertising();
            Log.Verbose(BleServer.Tag, $"service started with id {service.Uuid}");
        }

        /// <summary>
        /// Sends accelerometer data as the bluetooth client
        /// performs read request.
        /// </summary>
        /// <param name="s"/>
        /// <param name="e">Accelerometer String</param>
        private void SendData(object s, BleEventArgs e)
        {
            try
            {
                var data = "send";
                if (SensorData.Count > 0)
                {
                    data =  SensorData.Dequeue();
                }
                e.Characteristic.SetValue(data);
                bltServer.SendResponse(e.Device, e.RequestId, GattStatus.Success, e.Offset, e.Characteristic.GetValue() ?? throw new InvalidOperationException());
                bltServer.NotifyCharacteristicChanged(e.Device, e.Characteristic, false);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
        /// <summary>
        /// Creates server with its rules that allow
        /// to read,write,notify its properties such as Characteristic & Descriptor
        /// (only characteristic used at this movement)
        /// </summary>
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
        /// <summary>
        /// Start advertising the server connection after
        /// it is been created
        /// </summary>
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
        /// <summary>
        /// Converts chosen string as uuid to UUID instance
        /// </summary>
        /// <param name="uuid">server access code string</param>
        /// <returns>server access UUID Instance</returns>
        private static UUID GetUuid(string uuid) => UUID.FromString(uuid);
        
        /// <summary>
        /// Disables advertisement of servers connection
        /// </summary>
        public void StopAdvertising()
        {
            if (bltAdvertiser == null) return;
            try
            {
                bltAdvertiser.StopAdvertising(bltAdvertiserCallback);
                service?.Characteristics?.Clear();
                bltServer.Services?.Clear();
                BltCallback.Dispose();
            }
            catch (Exception e)
            {
                Log.Verbose(Tag, $"Fail to stop the server...{e}");
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
            Log.Verbose(BleServer.Tag, $"Advertise : Start error : {errorCode}");
        }
        public override void OnStartSuccess(AdvertiseSettings settingsInEffect)
        {
            base.OnStartSuccess(settingsInEffect);
            Log.Verbose(BleServer.Tag, "Advertise : Start Success ");
        }
    }
}