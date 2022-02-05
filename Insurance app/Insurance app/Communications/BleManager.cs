﻿using System;
using System.Globalization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.Text.Style;
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
        public EventHandler<RawDataArgs> InfferEvent = delegate {  };

        private int readingDelay = 0;
        private int conErrDelay = 0;
        private bool bleState = false;
        private bool isMonitoring = false;
        private static BleManager bleManager =null;
        
        private BleManager()
        {
            ble = new Ble();
            adapter = CrossBluetoothLE.Current.Adapter;
            RegisterEventHandlers();
            bleState=ble.BleCheck();

        }
        public static BleManager GetInstance()
        {
            if (bleManager is null)
            {
                return new BleManager();
            }

            return bleManager;
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
                if (!isMonitoring) return;

                var data = await chara.ReadAsync();
                
                string str = " ";
                str = Encoding.Default.GetString(data);
                if (str.Equals(" "))
                {
                    if (!isMonitoring) return;

                    readingDelay += 3000;
                    Console.WriteLine($"reading empty : wait {readingDelay / 1000}sec > try again");
                    Task t = Task.Run(async () =>
                    {
                        await Task.Delay(readingDelay);
                        return ReadAsync();
                    });
                    return;
                }

                readingDelay = 0;
                //Console.WriteLine($"Read complete, values are : > {str}");
                Infer(str);
                await ReadAsync();
            }
           /* catch (CharacteristicReadException readException)
            {
                reading = false;
                Console.WriteLine("char exception");
            }*/
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
                var split = rawData.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
                var timeStamp = Convert.ToInt64(split[0]);
                var x = Converter.StringToFloat(split[1]);
                var y = Converter.StringToFloat(split[2]);
                var z = Converter.StringToFloat(split[3]);
                
                    InfferEvent?.Invoke(this,new RawDataArgs()
                    {
                        x = x,y = y,z = z,
                        Type = 1,
                        TimeOffset = Converter.ToDTOffset(timeStamp)
                    });
                
                
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
                var service = await device.GetServiceAsync(ble.SERVER_GUID);
                if (service is null)
                {
                    MainThread.BeginInvokeOnMainThread(Action);
                    return;
                }
                else
                {
                    chara = await service.GetCharacteristicAsync(ble.SERVER_GUID);
                }

                if (chara != null)
                {
                    ReadAsync();
                }
            }
            catch //fail to connect
            {
                
                ConnectToDevice();
            }
        }
        private async void Action()
        {
            await Shell.Current.DisplayAlert("Error", "Please install & turn on the watch app", "close");
        }

        private async Task ConnectToDevice()
        {
            if (!ble.IsAvailable() || !await ble.GetPremissionsAsync())
            {
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
            await Shell.Current.DisplayAlert("Error", "Type of Bluetooth not available and app needs your permissions", "close");
        }

        public Task ToggleMonitoring()
        {
            if (isMonitoring)
            {
                isMonitoring = false;
                return Task.CompletedTask;
            }
            else
            {
                isMonitoring = true;
                return Task.FromResult(ConnectToDevice());
            }
           
        }
    }
    public class RawDataArgs : EventArgs
    {
        public int Type { get; set; }
        public DateTimeOffset TimeOffset { get; set; }
        public float x { get; set; }
        public float y { get; set; }
        public float z { get; set; }
    }
}