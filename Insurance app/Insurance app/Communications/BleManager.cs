﻿/*
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
using System.Text;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Xamarin.Essentials;

namespace Insurance_app.Communications
{
    /// <summary>
    /// Main class used for connecting to the watch
    /// And used to transfer details
    /// </summary>
    public class BleManager
    {
        private IAdapter adapter;
        private Ble ble;
        private ICharacteristic chara;
        public EventHandler InfferEvent = delegate {  };
        public event EventHandler ToggleSwitch =delegate {  };

        private readonly int readingDelay = 5000; // reading delay every 5 sec (incase empty read.)
        private int conErrDelay;
        private bool bleState;
        private bool isMonitoring;
        private static BleManager _bleManager;
        private readonly UserManager userManager;
        private bool start=true;
        public string Email="";
        public string Pass="";
        private bool firstTime = true;
        private bool previousState;
        private bool currentState;
        private int count = 0;


        private BleManager()
        {
            ble = new Ble();
            adapter = CrossBluetoothLE.Current.Adapter;
            RegisterEventHandlers();
            bleState=ble.BleCheck();
            userManager = new UserManager();
        }
        public static BleManager GetInstance()
        {
            return _bleManager ??= new BleManager();
        }
        /// <summary>
        /// Method used to register event handlers
        /// </summary>
        private void RegisterEventHandlers()
        {
            
            ble.BLE.StateChanged += (s,e) =>
            {
                Console.WriteLine($"Ble state changed {e.NewState.ToString()}");
                if (e.NewState == BluetoothState.On)
                {
                    bleState = true;
                }else if (e.NewState == BluetoothState.Off || e.NewState == BluetoothState.TurningOff)
                {
                    bleState = false;
                }
            };
            adapter.DeviceConnected += async (s, e) =>
            {
                Console.WriteLine($"device connected : {e.Device.Name}");
                await GetService(e.Device);
            };
        }
        /// <summary>
        /// Reads information from the
        /// Characteristic, in case of empty read
        /// Waits for certain time and reentry's again.
        /// After read completed notifies HomeViewModel to change UI
        /// </summary>
        private async Task ReadAsync()
        {
            Console.WriteLine("Reading data from ble ");
            try
            {
                if (!isMonitoring) return;
                firstTime = false;
                var data = await chara.ReadAsync();
                
                var str = " ";
                str = Encoding.Default.GetString(data);
                if (str.Equals(" ") && count<60)
                {
                    count+=1;
                    Console.WriteLine($"reading empty : wait {readingDelay / 1000}sec > try again");
                    await Task.Run(async () =>
                    {
                        await Task.Delay(readingDelay);
                        return ReadAsync();
                    });
                }else if(count>=60) {
                    count = 0;
                    isMonitoring = false;
                    ToggleSwitch.Invoke(this,EventArgs.Empty);
                }
                else
                {
                    count = 0;
                    InfferEvent.Invoke(this,EventArgs.Empty);
                   await ReadAsync();
                }
            }catch(Exception e) {
                Console.WriteLine("Exception"+e+" counter="+count);
                
                count += 1;
                if(count>=60)
                {
                    count = 0;
                    isMonitoring = false;
                    ToggleSwitch.Invoke(this,EventArgs.Empty);
                }
                else
                {
                    await ConnectToDevice();
                }
            }
        }
        /// <summary>
        /// Method tries to get service & Characteristic
        /// In order to read from it.
        /// </summary>
        /// <param name="device">Device that has been connected to.</param>
        private async Task GetService(IDevice device)
        {
            try
            {
                
                var service = await device.GetServiceAsync(ble.ServerGuid);
              if (service is null)
              {
                  Console.WriteLine("service is null ");
                  isMonitoring = false;
                  await MainThread.InvokeOnMainThreadAsync(MessageUser);
                  return;
              }
              chara = null;
              chara = await service.GetCharacteristicAsync(ble.ServerGuid);
              if (chara is null)
              {
                  Console.WriteLine("characteristic is null ");
                  isMonitoring = false;
                  await MainThread.InvokeOnMainThreadAsync(MessageUser);
                  return;
              }
              if (start)
              {
                  isMonitoring = true;
                  await WriteToCharacteristic($"{App.RealmApp.CurrentUser.Id}|{Email}|{Pass}");
                  WatchService.StartListener();
                  
              }
              else if (!start)
              {
                  isMonitoring = false;
                  await WriteToCharacteristic("Stop");
                  await UpdateCustomerSwitch(false);
                  WatchService.StopListener();
              }
             
              //await ReadAsync();
                
            }
            catch (Exception e) //fail to connect
            { 
                Console.WriteLine(e.Message);
               await ConnectToDevice();
            }
        }
        /// <summary>
        /// Method used to send message to the connected watch
        /// </summary>
        /// <param name="message">Start/Stop gathering data (with credentials)</param>
        private async Task WriteToCharacteristic(string message)
        {
            if (chara.CanWrite)
            {
                try
                {
                    Console.WriteLine("sending message : "+message);
                    await chara.WriteAsync(Encoding.Default.GetBytes(message));
                    firstTime = false;
                    
                }
                catch (Exception e)
                {
                    Console.WriteLine("write to characteristic exception "+e);
                }
                
            }
        }
        /// <summary>
        /// Updates connection switch on database
        /// So the watch can retrieve=> Start/Stop monitoring activity
        /// </summary>
        /// <param name="state">Start/Stop monitoring activity</param>
        private async Task UpdateCustomerSwitch(bool state)
        {
            try
            {
                await userManager.UpdateCustomerSwitch(App.RealmApp.CurrentUser, state);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        /// <summary>
        /// Notify user of a fault
        /// </summary>
        private async void MessageUser()
        {
            
            ToggleSwitch.Invoke(this,EventArgs.Empty);
            if (!wasOn())await Msg.AlertError("Please install & turn on the watch app");
        }

        /// <summary>
        /// Try to connect to a device given user permission
        /// </summary>
        private async Task ConnectToDevice()
        {
            if (!ble.IsAvailable() || !await ble.GetPremissionsAsync())
            {
                isMonitoring = false;
                await MainThread.InvokeOnMainThreadAsync(Action1);
            }
            else if (bleState)
            {
                try
                {
                    var list = adapter.GetSystemConnectedOrPairedDevices(new Guid[] {ble.ServerGuid});
                    await adapter.ConnectToDeviceAsync(list[0]);
                    conErrDelay = 0;
                }
                catch
                {
                    if (start)
                    {
                        isMonitoring = false;
                        await MainThread.InvokeOnMainThreadAsync(MessageUser);
                        return;
                    }
                    //dont need to see an error message, since this is depends connection loss
                    conErrDelay += 3000;
                    Console.WriteLine($" Device Conn Fail : wait {conErrDelay/1000}s , Reconnect");
                    Task t = Task.Run(async ()=>
                    {
                        await Task.Delay(conErrDelay);
                        await ConnectToDevice();
                    });
                    
                }
               
            }
        }

        /// <summary>
        /// Notify user with no permissions
        /// </summary>
        private async void Action1()
        {
            ToggleSwitch.Invoke(this,EventArgs.Empty);
            await Msg.AlertError("Type of Bluetooth not available and app needs your permissions");
        }

        /// <summary>
        /// Notify user with no Bluetooth
        /// </summary>
        private async void NoBluetooth()
        {
            if (!wasOn()) await Msg.AlertError("Bluetooth is off");
            ToggleSwitch.Invoke(this,EventArgs.Empty);
        }

        private bool wasOn()
        {
            if (previousState && !currentState && firstTime)
            {
                previousState = false;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Turn on/off try to connect to bluetooth
        /// </summary>
        /// <param name="currentState">on/off ble state</param>
        /// <param name="email">customer email string input</param>
        /// <param name="password">customers password string input</param>
        public async Task ToggleMonitoring(bool currentState,bool previousState,string email,string password)
        {
            if (Email=="" || Pass=="")
            {
                Email = email;
                Pass = password;
            }
            isMonitoring = false;
            this.previousState = previousState;
            this.currentState = currentState;
            if (previousState || currentState)
            {
                start = true;
            }
            else if(!currentState)
            {
                start = false;
            }

            if (!bleState)
            {
                await MainThread.InvokeOnMainThreadAsync(NoBluetooth);
                isMonitoring = false;
            }
            await ConnectToDevice();
        }
    }
}