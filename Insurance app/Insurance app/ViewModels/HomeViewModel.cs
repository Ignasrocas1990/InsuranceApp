using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Java.Lang;
using Realms;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Exception = System.Exception;

namespace Insurance_app.ViewModels
{
    public class HomeViewModel : ObservableObject
    {

        public ICommand StepCommand { get; }
        public bool ToggleState = false;
        private BleManager bleManager;
        private UserManager userManager;
        //private ConcurrentBag<MovData> newMovDataList;
        private ConcurrentQueue<MovData> newMovDataList;
        //private List<MovData> newMovDataList;
        private Customer customer;




        private double currentSteps = 0;// set this to len of the mov data array
        private double currentProgressBars=0.0;
        private double max = 0 ;
        private double min = StaticOptions.StepNeeded;
        
        public HomeViewModel()
        {
            StepCommand = new Command(Step);
            bleManager = BleManager.GetInstance();
            bleManager.InfferEvent += InferredRawData;
            userManager = new UserManager();
        }

        public async Task Setup()
        {
            try
            {
                newMovDataList = new ConcurrentQueue<MovData>();
                customer = await userManager.GetCustomer(App.RealmApp.CurrentUser);
                if (customer is null)
                {
                    await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                    userManager.realmDb.StopSync();
                    await Shell.Current.GoToAsync($"//{nameof(LogInPage)}",false);
                    return;
                }

                if (customer.Reward.Count > 0)//TODO need to test this (when has more then 1 reward)
                {
                    var reward = customer.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                    if (reward != null)
                    {
                        double movLen = Convert.ToDouble(reward.MovData.Count());
                        SetUpView(movLen);
                        return;
                    }
                }
                await customer.CreateReward();
                ProgressBarDisplay = StaticOptions.StepNeeded;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        private void InferredRawData(object s, RawDataArgs e)
        {
            Step();
           // AddMov(e.x, e.y,e.z, e.Type, e.TimeOffset);
            Task.Run(async () =>
            {
                try
                {
                    if (ProgressBarDisplay <= 0)
                    {
                        await customer.CreateReward();
                        
                        MainThread.BeginInvokeOnMainThread(() =>
                        {
                            ProgressBarDisplay = StaticOptions.StepNeeded;
                        
                        });
                    }
                    var currMovData = new MovData()
                                    {
                                        AccData = new Acc()
                                        {
                                            X = e.x,
                                            Y = e.y,
                                            Z = e.z
                                        },
                                        DateTimeStamp = e.TimeOffset,
                                        Type = "step",
                                        Partition = App.RealmApp.CurrentUser.Id
                                    };
                    newMovDataList.Enqueue(currMovData);//-----------------------------------
                    if (newMovDataList.Count > 4)
                    {
                        //or re-create new Realm
                        RealmDb realmDb = new RealmDb();
                        await realmDb.AddMovData2(new ConcurrentQueue<MovData>(newMovDataList),App.RealmApp.CurrentUser);
                        newMovDataList = new ConcurrentQueue<MovData>();
                    }

                }
                catch (Exception exception)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        Console.WriteLine(exception);
                        
                    });
                }
                
            });
        }
        
        public async void StartDataReceive(bool newValue)
        {
            if (newValue)
            {
                try
                {
                     await bleManager.ToggleMonitoring();
                    Console.WriteLine("started to receive data");
                    //StepDetector.StepCounted += StepDisplayUpdate;
                }
                catch (Exception e)
                {
                    Console.WriteLine("something went wrong starting BLE");
                }
                
            }
            else if(bleManager != null)
            {
                  await bleManager.ToggleMonitoring();
                Console.WriteLine("stopped to receive data");
            }

            ToggleStateDisplay = newValue;
        }

        public void SetUpView(double steps)
        {
            while (steps !=0)
            {
                steps--;
                Step();
            }
        }

        private void StepDisplayUpdate(object s, EventArgs e)
        {
            ProgressBarDisplay--;
            currentSteps++;
            StepsDisplay++;
            if (ProgressBarDisplay < max)
            {
                ProgressBarDisplay = min;
                StepsDisplay = 0;
            }
        }
        
        private void Step()
        {
            ProgressBarDisplay--;
            currentSteps++;
            StepsDisplay++;
            if (ProgressBarDisplay < max)
            {
                ProgressBarDisplay = min;
                StepsDisplay = 0;
            }
        }
        public bool ToggleStateDisplay
        {
            get => ToggleState;
            set => SetProperty(ref ToggleState, value);
        }
        public void UpdateSwitchDisplay()
        {
            ToggleStateDisplay = ToggleState;
        }
        public double ProgressBarDisplay // progress bar display
        {
            get => currentProgressBars;
            set =>  SetProperty(ref currentProgressBars, value);
        }
        public double StepsDisplay //the percentages label in the middle
        {
            
            get => currentSteps / StaticOptions.StepNeeded * 100;
            set => SetProperty(ref  temp, value);
        }
        private double temp = 0;
    }
}