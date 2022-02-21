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
    public class HomeViewModel : ObservableObject,IDisposable
    {

        private bool ToggleState = false;
        private BleManager bleManager;
        private UserManager userManager;
        private RewardManager rewardManager;
        private ConcurrentQueue<MovData> newMovDataList;
        //private Customer customer;



        private double stepsDisplayValue = 0;
        private double currentProgressBars = 0.0;
        private double max = 0 ;
        private double min = StaticOpt.StepNeeded;
        
        public HomeViewModel()
        {
            bleManager = BleManager.GetInstance();
            bleManager.InfferEvent += InferredRawData;
            userManager = new UserManager();
            rewardManager = new RewardManager();
        }

        public async Task Setup()
        {
            try
            {
                newMovDataList = new ConcurrentQueue<MovData>();
                var customer = await userManager.GetCustomer(App.RealmApp.CurrentUser);
                
                if (customer is null)
                {
                    await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                    userManager.Dispose();
                    await Shell.Current.GoToAsync($"//{nameof(LogInPage)}",false);
                    return;
                }
                

                if (customer.Reward.Count > 0)//TODO need to test this (when has more then 1 reward)
                {
                    var reward = customer.Reward.FirstOrDefault(r => r.FinDate == null);
                    if (reward != null)
                    {
                        double movLen = Convert.ToDouble(reward.MovData.Count());
                        //TODO uncomment to show
                        //Random rand = new Random();
                        //movLen = rand.NextDouble() * 10000;
                        SetUpView(movLen);
                        return;
                    }
                }
                await customer.CreateReward();
                ProgressBarDisplay = StaticOpt.StepNeeded;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async Task SetUpEarningsDisplay()
        {
            TotalEarnedDisplay = $"{await rewardManager.getTotalRewards(App.RealmApp.CurrentUser,App.RealmApp.CurrentUser.Id)}";
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
                        await rewardManager.CreateReward(App.RealmApp.CurrentUser);
                       //await customer.CreateReward();
                        
                        MainThread.BeginInvokeOnMainThread(ResetRewardDisplay);
                    }
                    var currMovData = new MovData()
                                    {
                                        AccData = new Acc()
                                        {
                                            X = e.x,
                                            Y = e.y,
                                            Z = e.z
                                        },
                                        Type = "step",
                                        Partition = App.RealmApp.CurrentUser.Id
                                    };
                    newMovDataList.Enqueue(currMovData);
                    if (newMovDataList.Count > 4)
                    {
                        await RealmDb.GetInstance().AddMovData(new ConcurrentQueue<MovData>(newMovDataList),App.RealmApp.CurrentUser);
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

        private void SetUpView(double steps)
        {
            ProgressBarDisplay = StaticOpt.StepNeeded - steps;
            StepsDisplayLabel = steps;
        }
// reset 
        private void ResetRewardDisplay()
        {
            ProgressBarDisplay = StaticOpt.StepNeeded;
            stepsDisplayValue = 0;
            StepsDisplayLabel = 0;

        }
        
        private void Step()
        {
            ProgressBarDisplay--;
            //stepsDisplayValue++;
            StepsDisplayLabel=stepsDisplayValue+1;
            if (ProgressBarDisplay < max)
            {
                ProgressBarDisplay = StaticOpt.StepNeeded;
                //stepsDisplayValue = 0;
                StepsDisplayLabel = 0;
            }
        }
        public bool ToggleStateDisplay
        {
            get => ToggleState;
            set => SetProperty(ref ToggleState, value);
        }
        
        public double ProgressBarDisplay // progress bar display
        {
            get => currentProgressBars;
            set =>  SetProperty(ref currentProgressBars, value);
        }
        public double StepsDisplayLabel //the percentages label in the middle
        {
            
            get => stepsDisplayValue / StaticOpt.StepNeeded * 100;
            set => SetProperty(ref  stepsDisplayValue, value);
        }

        private bool circularWait;
        public bool CircularWaitDisplay
        {
            get => circularWait;
            set => SetProperty(ref circularWait, value);
        }

        private string totalEarnedDisplay;
        public string TotalEarnedDisplay
        {
            get => $"Total Earned : {totalEarnedDisplay}";
            set => SetProperty(ref totalEarnedDisplay, value);
        }
        
        private bool setUpWait;
        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }
        
        public void Dispose()
        {
            userManager.Dispose();
            newMovDataList = null;
            rewardManager.Dispose();
            userManager = null;
            rewardManager = null;

        }
    }
}