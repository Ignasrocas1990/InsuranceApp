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
using System.Text;
using System.Threading.Tasks;
using Insurance_app.Logic;
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
        public string Email;
        public string Pass;
        private bool stop;
        private bool turnInprocess;


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
                    Task.FromResult(ConnectToDevice());
                }else if (e.NewState == BluetoothState.Off || e.NewState == BluetoothState.TurningOff)
                {
                    bleState = false;
                }
            };
            adapter.DeviceConnected += (s, e) =>
            {
                Console.WriteLine($"device connected : {e.Device.Name}");
                Task.FromResult(GetService(e.Device));
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
            try
            {
                if (!isMonitoring) return;

                var data = await chara.ReadAsync();
                
                string str = " ";
                str = Encoding.Default.GetString(data);
                if (str.Equals(" "))
                {
                    if (!isMonitoring) return;

                    Console.WriteLine($"reading empty : wait {readingDelay / 1000}sec > try again");
                    Task t = Task.Run(async () =>
                    {
                        await Task.Delay(readingDelay);
                        return ReadAsync();
                    });
                    return;
                }
                InfferEvent.Invoke(this,EventArgs.Empty);
                Task task = Task.Run(ReadAsync);
               
            }
            catch
            {
                conErrDelay += 3000;
                Console.WriteLine($"[read disturbed] wait {conErrDelay/1000}s: reconnect to device");

                Task t = Task.Run(async () =>
                {
                    await Task.Delay(conErrDelay);
                   await ConnectToDevice();
                    
                });
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
                    MainThread.BeginInvokeOnMainThread(MessageUser);
                    return;
                }
                chara = null;
                chara = await service.GetCharacteristicAsync(ble.ServerGuid);
                if (chara is null)
                {
                    Console.WriteLine("characteristic is null ");
                    isMonitoring = false;
                    MainThread.BeginInvokeOnMainThread(MessageUser);
                    return;
                }
                if (start)
                {
                    isMonitoring = true;
                    start = false;
                    await WriteToCharacteristic($"{App.RealmApp.CurrentUser.Id}|{Email}|{Pass}");
                   await UpdateCustomerSwitch(true);
                }
                else if (stop)
                {
                    turnInprocess = false;
                    stop = false;
                    isMonitoring = false;
                    await WriteToCharacteristic("Stop");
                    await UpdateCustomerSwitch(false);
                    return;
                }
                Console.WriteLine("start Reading data from ble ");
                turnInprocess = false;
                await ReadAsync();
                
            }
            catch //fail to connect
            {
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
                    await chara.WriteAsync(Encoding.Default.GetBytes(message));
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
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
            turnInprocess = false;
            ToggleSwitch.Invoke(this,EventArgs.Empty);
            await Msg.AlertError("Please install & turn on the watch app");
        }

        /// <summary>
        /// Try to connect to a device given user permission
        /// </summary>
        private async Task ConnectToDevice()
        {
            if (!ble.IsAvailable() || !await ble.GetPremissionsAsync())
            {
                isMonitoring = false;
                MainThread.BeginInvokeOnMainThread(Action1);
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
                        MainThread.BeginInvokeOnMainThread(MessageUser);
                        return;
                    }
                    //dont need to see an error message, since this is depends connection loss
                    conErrDelay += 3000;
                    Console.WriteLine($" Device Conn Fail : wait {conErrDelay/1000}s , Reconnect");
                    Task t = Task.Run(async ()=>
                    {
                        await Task.Delay(conErrDelay);
                        return Task.FromResult(ConnectToDevice());
                    });
                    
                }
               
            }
        }

        /// <summary>
        /// Notify user with no permissions
        /// </summary>
        private async void Action1()
        {
            turnInprocess = false;
            ToggleSwitch.Invoke(this,EventArgs.Empty);
            await Msg.AlertError("Type of Bluetooth not available and app needs your permissions");
        }

        /// <summary>
        /// Notify user with no Bluetooth
        /// </summary>
        private async void NoBluetooth()
        {
            turnInprocess = false;
            ToggleSwitch.Invoke(this,EventArgs.Empty);
            await Msg.AlertError("Bluetooth is off");
        }

        /// <summary>
        /// Turn on/off try to connect to bluetooth
        /// </summary>
        /// <param name="state">on/off ble state</param>
        /// <returns>if turning on successful</returns>
        public async Task<bool> ToggleMonitoring(bool state)
        {
            if (!turnInprocess)
            {
                turnInprocess = true;
            }
            else
            {
                return state;
            }
            switch (state)
            {
                case true:
                    start = true;
                    stop = false;
                    break;
                
                case false:
                    stop = true;
                    start = false;
                    break;
            }
            if (!bleState)
            {
                MainThread.BeginInvokeOnMainThread(NoBluetooth);
                isMonitoring = false;
                return false;
            }
            await ConnectToDevice();
            
            return state;
            
        }
    }
}