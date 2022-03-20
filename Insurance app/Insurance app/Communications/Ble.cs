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
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;

namespace Insurance_app.Communications
{
    /// <summary>
    /// Class used to get user permissions
    /// Store connection uuid and using plug in
    /// check if the Bluetooth Low Energy (BLE) connection is on
    /// </summary>
    public class Ble
    {
        public readonly IBluetoothLE BLE;
        public Guid ServerGuid { get; set; }
        private const string UuidString = "a3bb5442-5b61-11ec-bf63-0242ac130002";
        private readonly Func<string,Guid> setGuid = s => Guid.Parse(s);
        public Ble()
        {
            BLE = CrossBluetoothLE.Current;
            ServerGuid = setGuid(UuidString);
        }
        /// <summary>
        /// Checks if Bluetooth is on
        /// </summary>
        /// <returns>returns true if Ble is on</returns>
        public bool BleCheck() => BLE.IsOn || BLE.State == BluetoothState.TurningOn;
        
        /// <summary>
        /// Gets permission using xamarin essentials
        /// Code provided on the website 
        /// </summary>
        /// <returns>returns true if user gave permissions</returns>
        public async Task<bool> GetPremissionsAsync()
        {
            var locationPermissionStatus = await Permissions.CheckStatusAsync<Permissions.LocationAlways>();

            var sensorsPermission =  await Permissions.CheckStatusAsync<Permissions.Sensors>();
            var granted = PermissionStatus.Granted;
            if (locationPermissionStatus == granted && sensorsPermission == granted) return true;
            
            var locStatus = await Permissions.RequestAsync<Permissions.LocationAlways>();
            var sensorStatus =await Permissions.RequestAsync<Permissions.Sensors>();
            return (locStatus == granted && sensorStatus == granted);
        }
        /// <summary>
        /// checks if BLE type Bluetooth available
        /// </summary>
        /// <returns> true if available</returns>
        public bool IsAvailable() => BLE.IsAvailable;
    }
}