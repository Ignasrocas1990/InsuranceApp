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
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms;
using Xamarin.CommunityToolkit.Extensions;
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
        private readonly UserManager userManager;
        private readonly RewardManager rewardManager;

        //private Customer customer;

        private double stepsDisplayValue = 0;
        private double currentProgressBars = 0.0;
        private double max = 0 ;
        private bool firstSetup = true;
        private int counter = 0;
        private double startUpSteps;
        private Reward reward;
        public ICommand SwitchCommand { get; }
        public ICommand LogoutCommand { get; }

        private bool switchState = false;

        public HomeViewModel()
        {
            bleManager = BleManager.GetInstance();
            bleManager.InfferEvent +=InferredRawData;
            rewardManager = new RewardManager();
            userManager = new UserManager();
            SwitchCommand = new AsyncCommand(StartDataReceive);
            LogoutCommand = new AsyncCommand(Logout);
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
            
               var (toggle, totalSum) = await rewardManager
                   .GetTotalRewards(App.RealmApp.CurrentUser,App.RealmApp.CurrentUser.Id);
               TotalEarnedDisplay = $"{totalSum}";
               Console.WriteLine("ble manager SetUpEarningsDisplay");
               if (firstSetup)
               {
                   ToggleStateDisplay = toggle;
                   if (toggle)
                   {
                       await StartDataReceive();
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
        private async Task StartDataReceive()
        {
            if (++counter % 2==0) return;
            
            CircularWaitDisplay = true;
            switchState = await bleManager.ToggleMonitoring(toggleState);
            ToggleStateDisplay = switchState;
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
                    bleManager.Email = Uri.UnescapeDataString(value ?? "");
            }
        }
        
        public string Pass
        {
            set
            {
                if (bleManager!=null)
                    bleManager.Pass = Uri.UnescapeDataString(value ?? "");
            }
        }
        private async Task Logout()
        {
          CircularWaitDisplay = true;
          await StaticOpt.Logout();
          CircularWaitDisplay = false;
        }

        public void Dispose()
        {
            userManager.Dispose();
        }
    }
}