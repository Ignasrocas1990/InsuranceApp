﻿using System;
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

        private bool collectingData;
        private BleManager bleManager;
        private UserManager userManager;
        private RewardManager rewardManager;
        private ConcurrentQueue<MovData> newMovDataList;

        //private Customer customer;



        private double stepsDisplayValue = 0;
        private double currentProgressBars = 0.0;
        private double max = 0 ;
        private double min = StaticOpt.StepNeeded;
        private bool wasOn;
        private bool FirstSetup = true;
        private int counter = 0;

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
                //DelFlag == true (reward has been used)
                var rewards = customer.Reward.Where(r => r.DelFlag == false).ToList();
                if (rewards.Count==0)
                {
                    await customer.CreateReward();
                    ProgressBarDisplay = StaticOpt.StepNeeded;
                }
                var currentDate = DateTimeOffset.Now;

                //get all rewards finish in this month
                var currentMonthRewards = rewards.Count(r => r.FinDate != null 
                                                             && r.FinDate.Value.Month == currentDate.Month
                                                             && r.FinDate.Value.Year == currentDate.Year);
                
                if (currentMonthRewards < 26)
                {
                    var reward = customer.Reward.FirstOrDefault(r => r.FinDate == null);
                    if (reward != null)
                    {
                        double movLen = Convert.ToDouble(reward.MovData.Count());
                        //TODO uncomment to show
                        //Random rand = new Random();
                        //movLen = rand.NextDouble() * 10000;
                        SetUpView(movLen);
                    }
                    else
                    {
                        await customer.CreateReward();
                        ProgressBarDisplay = StaticOpt.StepNeeded;
                    }
                }
                else
                {
                    ProgressBarDisplay = StaticOpt.StepNeeded;
                    MaxRewardIsVisible = true;
                }
                await SetUpEarningsDisplay();
                

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
                                        Type = "step"
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
        
        public async Task StartDataReceive()
        {
            counter++;
            if (counter % 2 != 1) return;
            
            if (!collectingData)
            {
                try
                {
                    collectingData = await bleManager.ToggleMonitoring();
                    Console.WriteLine("started to receive data");
                    //StepDetector.StepCounted += StepDisplayUpdate;
                }
                catch (Exception e)
                {
                    Console.WriteLine("something went wrong starting BLE");
                }
                
            }
            else if(collectingData && bleManager !=null)
            {
                collectingData = await bleManager.ToggleMonitoring();
                Console.WriteLine("stopped to receive data");
            }
            ToggleStateDisplay = collectingData;
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
        private bool toggleState;
        public bool ToggleStateDisplay
        {
            get => toggleState;
            set => SetProperty(ref toggleState, value);
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

        private bool maxReward;
        public bool MaxRewardIsVisible
        {
            get => maxReward;
            set => SetProperty(ref maxReward, value);
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