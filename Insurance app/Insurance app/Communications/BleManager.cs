﻿using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Text.Style;
using Insurance_app.Logic;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Exception = System.Exception;

namespace Insurance_app.Communications
{
    public class BleManager
    {


        private IAdapter adapter;
        public Ble ble;
        private ICharacteristic chara=null;
        public EventHandler InfferEvent = delegate {  };
        public event EventHandler ToggleSwitch =delegate {  };

        private int readingDelay = 5000; // reading delay every 5 sec (incase empty read.)
        private int conErrDelay = 0;
        private bool bleState = false;
        public bool IsMonitoring = false;
        private static BleManager _bleManager =null;
        private UserManager userManager;
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
        
        private void RegisterEventHandlers()
        {
            
            ble.ble.StateChanged += (s,e) =>
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

        private async Task ReadAsync()
        {
            try
            {
                if (!IsMonitoring) return;

                var data = await chara.ReadAsync();
                
                string str = " ";
                str = Encoding.Default.GetString(data);
                if (str.Equals(" "))
                {
                    if (!IsMonitoring) return;

                    Console.WriteLine($"reading empty : wait {readingDelay / 1000}sec > try again");
                    Task t = Task.Run(async () =>
                    {
                        await Task.Delay(readingDelay);
                        return ReadAsync();
                    });
                    return;
                }

                //Console.WriteLine($"Read complete, values are : > {str}");
                Infer(str);
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

        private void Infer(string rawData)
        {
            try
            {
                //var split = rawData.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                //var x = Converter.StringToFloat(split[0]);
                //var y = Converter.StringToFloat(split[1]);
                //var z = Converter.StringToFloat(split[2]);
                InfferEvent.Invoke(this,EventArgs.Empty);
                /*
                Task.Run(async () =>
                {
                    //await rewardManager.addNewMovDate(x, y, z,App.RealmApp.CurrentUser);
                });*/

            }
            catch (Exception e)
            {
                Console.WriteLine($"problem pre-paring data for inferring {e}");
            }
        }
        private async Task GetService(IDevice device)
        {
            try
            {
                IService service = null;
                 service = await device.GetServiceAsync(ble.SERVER_GUID);
                if (service is null)
                {
                    Console.WriteLine("service is null ");
                    IsMonitoring = false;
                    MainThread.BeginInvokeOnMainThread(MessageUser);
                    return;
                }
                chara = null;
                chara = await service.GetCharacteristicAsync(ble.SERVER_GUID);
                if (chara is null)
                {
                    Console.WriteLine("characteristic is null ");
                    IsMonitoring = false;
                    MainThread.BeginInvokeOnMainThread(MessageUser);
                    return;
                }
                if (start)
                {
                    IsMonitoring = true;
                    start = false;
                    await WriteToCharacteristic($"{App.RealmApp.CurrentUser.Id}|{Email}|{Pass}");
                   await UpdateCustomerSwitch(true);
                }
                else if (stop)
                {
                    turnInprocess = false;
                    stop = false;
                    IsMonitoring = false;
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


        private async void MessageUser()
        {
            turnInprocess = false;
            ToggleSwitch.Invoke(this,EventArgs.Empty);
            await Shell.Current.DisplayAlert("Error", "Please install & turn on the watch app", "close");
            
            
        }

        private async Task ConnectToDevice()
        {
            if (!ble.IsAvailable() || !await ble.GetPremissionsAsync())
            {
                IsMonitoring = false;
                MainThread.BeginInvokeOnMainThread(Action1);
            }
            else if (bleState)
            {
                try
                {
                    var list = adapter.GetSystemConnectedOrPairedDevices(new Guid[] {ble.SERVER_GUID});
                    await adapter.ConnectToDeviceAsync(list[0]);
                    conErrDelay = 0;
                }
                catch
                {
                    if (start)
                    {
                        IsMonitoring = false;
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

        private async void Action1()
        {
            turnInprocess = false;
            ToggleSwitch.Invoke(this,EventArgs.Empty);
            await Shell.Current.DisplayAlert("Error", "Type of Bluetooth not available and app needs your permissions", "close");
        }

        private async void NoBluetooth()
        {
            turnInprocess = false;
            ToggleSwitch.Invoke(this,EventArgs.Empty);
            await Shell.Current.DisplayAlert("Error", "Bluetooth is off", "close");
        }

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
                IsMonitoring = false;
                return false;
            }
            ConnectToDevice();
            
            return state;
            
        }
    }
    public class RawDataArgs : EventArgs
    {
        public int Type { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}