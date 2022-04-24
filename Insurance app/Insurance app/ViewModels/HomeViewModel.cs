/*   Copyright 2020,Ignas Rocas

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
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Logic;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms.Sync;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    /// <summary>
    /// Class used to store and manipulate HomePage UI components
    /// in real time via BindingContext and its properties
    /// </summary>
    public class HomeViewModel : ObservableObject,IDisposable
    {

        private readonly BleManager bleManager;
        private readonly RewardManager rewardManager;
        private double currentProgressBars;
        private bool firstSetup = true;
        private bool previousState;
        private double startUpSteps;
        private float rewardCost;
        private float totalRewardCount;
        public ICommand SwitchCommand { get; }
        public ICommand LogoutCommand { get; }
        
        private User user;
        public HomeViewModel()
        {
            bleManager = BleManager.GetInstance();
            rewardManager = new RewardManager();
            SwitchCommand = new AsyncCommand(StartDataReceive);
            LogoutCommand = new AsyncCommand(Logout);
        }
        /// <summary>
        /// Loads in data using manager classes via database and set it to Bindable properties(UI)
        /// </summary>
        public async Task Setup()
        {
            try
            {
                user = App.RealmApp.CurrentUser;
                SetUpWaitDisplay = true;
              var reward = await rewardManager.FindReward(user);
              if (reward is null)
                {
                    ProgressBarDisplay = StaticOpt.StepNeeded;
                    MaxRewardIsVisible = true;
                }
                else
                {
                    WatchService.GetInstance().CurrentRewardId = reward.Id;
                    if (reward.Cost != null) rewardCost = reward.Cost.Value;
                    ProgressBarDisplay = 0;
                    startUpSteps = Convert.ToDouble(reward.MovData.Count);
                    ProgressBarDisplay = StaticOpt.PercentPerStep*startUpSteps;
                }
                await SetUpEarningsDisplay();
                bleManager.ToggleSwitch += (o, e) => ToggleStateDisplay = false;
                WatchService.GetInstance().StepCheckedEvent += UpdateSteps;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetUpWaitDisplay = false;
            firstSetup = false;
        }

        /// <summary>
        /// loops through number nearly recorded by Cloud Database
        /// </summary>
        /// <param name="sender"/>
        /// <param name="e">number of steps that is recorded by the cloud db </param>
        private void UpdateSteps(object sender, StepArgs e)
        {
            Console.WriteLine($" old setups:{ProgressBarDisplay}, new steps:{StaticOpt.PercentPerStep*e.Steps}");
            if (e.Steps == -1)
            {
                ProgressBarDisplay = 0;
                totalRewardCount +=rewardCost;
                TotalEarnedDisplay = totalRewardCount.ToString("F");
            }
            else
            {
                while (ProgressBarDisplay < StaticOpt.PercentPerStep*e.Steps)
                {
                    Step();
                }
            }
            
        }

        /// <summary>
        /// gets sum of total earned rewards & toggle switch
        /// for theUI elements
        /// </summary>
        private async Task SetUpEarningsDisplay()
        {
            (var toggle, totalRewardCount) = await rewardManager
                   .GetTotalRewards(user,user.Id);
               TotalEarnedDisplay = totalRewardCount.ToString("F");
               if (firstSetup)
               {
                   previousState = toggle.Switch;
                   if (toggle.Switch)
                   {
                       await StartDataReceive();
                   }
               }else if (WatchService.State)
                   WatchService.StartListener();
        }
        
        /// <summary>
        /// Starts/Stops connect/receiving data from the BleManager/Watch
        /// </summary>
        private async Task StartDataReceive()
        {
            CircularWaitDisplay = true;
            WatchService.ToggleListener();
            await bleManager.ToggleMonitoring(toggleState,previousState);//previousState is DB state
            CircularWaitDisplay = false;
        }

        /// <summary>
        /// Increments the progress bar view and the label display
        /// </summary>
       // private async Task Step()
        private void Step()
        {
            ProgressBarDisplay+=StaticOpt.PercentPerStep;
            if (ProgressBarDisplay == 100)
            {
                ProgressBarDisplay = 0;
                totalRewardCount +=rewardCost;
                TotalEarnedDisplay = totalRewardCount.ToString("F");
            }
        }
        //--------------------- Bindable Properties below ---------------------------------
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

        private bool circularWait;
        public bool CircularWaitDisplay
        {
            get => circularWait;
            set => SetProperty(ref circularWait, value);
        }

        private string totalEarnedDisplay;
        public string TotalEarnedDisplay
        {
            get => $"Total Earned : {totalEarnedDisplay}€";
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
        /// <summary>
        /// Log's out the current user
        /// </summary>
        private async Task Logout()
        {
            CircularWaitDisplay = true;
            rewardManager.Dispose();
            await StaticOpt.Logout();
            CircularWaitDisplay = false;
        }

        public void Dispose()
        {
            if (WatchService.State) WatchService.StopListener();
            rewardManager.Dispose();
        }
    }
}