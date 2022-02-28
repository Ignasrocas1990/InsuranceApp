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

        private readonly BleManager bleManager;
        private UserManager userManager;
        private RewardManager rewardManager;

        //private Customer customer;

        private double stepsDisplayValue = 0;
        private double currentProgressBars = 0.0;
        private double max = 0 ;
        private bool firstSetup = true;
        private int counter = 0;
        private double startUpSteps;
        private Reward reward;

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
                reward = await rewardManager.FindReward(App.RealmApp.CurrentUser);
                
                if (reward is null)
                {
                    ProgressBarDisplay = StaticOpt.StepNeeded;
                    MaxRewardIsVisible = true;
                }
                else
                {
                    //Random rand = new Random();
                    //movLen = rand.NextDouble() * 10000; //TODO uncomment to show
                    ResetView();
                    startUpSteps = Convert.ToDouble(reward.MovData.Count);
                    SetUpView(startUpSteps);
                    
                }
                await SetUpEarningsDisplay();
                bleManager.ToggleSwitch += (o, e) =>
                {
                    ToggleStateDisplay = false;
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetUpWaitDisplay = false;
            firstSetup = false;
        }

        private async Task SetUpEarningsDisplay()
        {
            
               var data = await rewardManager.getTotalRewards(App.RealmApp.CurrentUser,App.RealmApp.CurrentUser.Id);
               if (data is null) return;
               TotalEarnedDisplay = $"{data.Item2}";
               Console.WriteLine("ble manager SetUpEarningsDisplay");
               if (firstSetup)
               {
                   ToggleStateDisplay = data.Item1;
                   if (data.Item1)
                   {
                       await StartDataReceive(true);
                   }
               }
        }
        private async void InferredRawData(object s, EventArgs eventArgs)
        {
            await Step();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="state">Switch on the data receive (true=on/false=off)</param>
        public async Task StartDataReceive(bool state)
        {
            if (!firstSetup)
            {
                counter++;
                if (counter % 2 != 1) return;
            }
            CircularWaitDisplay = true;

            if (state is false && toggleState)
            {
               await userManager.UpdateCustomerSwitch(App.RealmApp.CurrentUser, false);
            }
            
            state = await bleManager.ToggleMonitoring(state);
            ToggleStateDisplay = state;
            CircularWaitDisplay = false;
        }

        private void ResetView()
        {
            ProgressBarDisplay = StaticOpt.StepNeeded;
            StepsDisplayLabel = 0;
            stepsDisplayValue = 0;
        }
        private void SetUpView(double steps)
        {
            ProgressBarDisplay = StaticOpt.StepNeeded - steps;
            StepsDisplayLabel = steps;
            stepsDisplayValue = steps;
        }
        
        private async Task Step()
        {
            ProgressBarDisplay--;
            StepsDisplayLabel=stepsDisplayValue+1;
            if (ProgressBarDisplay <= max)
            {
                CircularWaitDisplay = true;
                ProgressBarDisplay = StaticOpt.StepNeeded;
                StepsDisplayLabel = 0;
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