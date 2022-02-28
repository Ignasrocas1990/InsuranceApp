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
using Realms;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Exception = System.Exception;

namespace Insurance_app.ViewModels
{
    [QueryProperty(nameof(Email),"Email")]
    [QueryProperty(nameof(Pass),"Pass")]
    public class HomeViewModel : ObservableObject,IDisposable
    {

        private bool collectingData;
        private readonly BleManager bleManager;
        private UserManager userManager;
        private RewardManager rewardManager;

        //private Customer customer;

        private double stepsDisplayValue = 0;
        private double currentProgressBars = 0.0;
        private double max = 0 ;
        private bool FirstSetup = true;
        private int counter = 0;

        public HomeViewModel()
        {
            bleManager = BleManager.GetInstance();
            bleManager.InfferEvent +=InferredRawData;
            rewardManager = new RewardManager();
            userManager = new UserManager();
        }
        public async Task Setup()
        {
            try
            {
                SetUpWaitDisplay = true;
                var reward = await rewardManager.FindReward(App.RealmApp.CurrentUser);
                if (reward is null)
                {
                    ProgressBarDisplay = StaticOpt.StepNeeded;
                    MaxRewardIsVisible = true;
                }
                else
                {
                    if (FirstSetup)
                    {
                        var stepsDouble = Convert.ToDouble(reward.MovData.Count());
                   
                        //Random rand = new Random();
                        //movLen = rand.NextDouble() * 10000; //TODO uncomment to show
                        SetUpView(stepsDouble);
                    }
                    
                }
                await SetUpEarningsDisplay();
                bleManager.ToggleSwitch += (o, e) =>
                {
                    ToggleStateDisplay = false;
                    collectingData = false;
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetUpWaitDisplay = false;
            FirstSetup = false;
        }

        private async Task SetUpEarningsDisplay()
        {
            
               var data = await rewardManager.getTotalRewards(App.RealmApp.CurrentUser,App.RealmApp.CurrentUser.Id);
               if (data is null) return;
               TotalEarnedDisplay = $"{data.Item2}";
               Console.WriteLine("ble manager SetUpEarningsDisplay");
               if (FirstSetup)
               {
                   ToggleStateDisplay = data.Item1;
               }
              

        }
        
        private async void InferredRawData(object s, EventArgs eventArgs)
        {
            await Step();
        }
        
        public async Task StartDataReceive()
        {
            counter++;
            if (counter % 2 != 1) return;
            CircularWaitDisplay = true;
            var switchState = collectingData;
            switch (collectingData)
            {
                case false:
                    collectingData = await bleManager.ToggleMonitoring();
                    break;
                case true:
                    collectingData = await bleManager.ToggleMonitoring();
                    Console.WriteLine("stopped to receive data");
                    break;
            }

            if (collectingData != switchState)
            {
                ToggleStateDisplay = collectingData;
            }
            CircularWaitDisplay = false;

           
        }
        private void SetUpView(double steps)
        {
            ProgressBarDisplay = StaticOpt.StepNeeded - steps;
            StepsDisplayLabel = steps;
            stepsDisplayValue = steps;
        }
// reset 
        private void ResetRewardDisplay()
        {
            ProgressBarDisplay = StaticOpt.StepNeeded;
            stepsDisplayValue = 0;
            StepsDisplayLabel = 0;

        }
        private async Task Step()
        {
            ProgressBarDisplay--;
            StepsDisplayLabel=stepsDisplayValue+1;
            if (ProgressBarDisplay < max)
            {
                CircularWaitDisplay = true;
                ProgressBarDisplay = StaticOpt.StepNeeded;
                StepsDisplayLabel = 0;
                Console.WriteLine("Step toggled");
                await SetUpEarningsDisplay();
                CircularWaitDisplay = false;

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

        public string Email
        {
            set
            {
                if (bleManager!=null)
                    bleManager.email = Uri.UnescapeDataString(value ?? "");
            }
        }
        
        public string Pass
        {
            set
            {
                if (bleManager!=null)
                    bleManager.pass = Uri.UnescapeDataString(value ?? "");
            }
        }


        public void Dispose()
        {
            userManager.Dispose();
            rewardManager.Dispose();
            userManager = null;
            rewardManager = null;

        }
    }
}