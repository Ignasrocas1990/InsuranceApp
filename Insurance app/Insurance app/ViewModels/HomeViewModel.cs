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
using Insurance_app.Models;
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
    [QueryProperty(nameof(Email),"Email")]
    [QueryProperty(nameof(Pass),"Pass")]
    public class HomeViewModel : ObservableObject,IDisposable
    {

        private readonly BleManager bleManager;
        private readonly RewardManager rewardManager;
        
        private double stepsDisplayValue = 0;
        private double currentProgressBars = 0.0;
        private const double Max = 0;
        private bool firstSetup = true;
        private int c = 0;
        private double startUpSteps;
        private Reward reward;
        public ICommand SwitchCommand { get; }
        public ICommand LogoutCommand { get; }

        private bool switchState = false;
        private User user;

        public HomeViewModel()
        {
            bleManager = BleManager.GetInstance();
            bleManager.InfferEvent +=InferredRawData;
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
                reward = await rewardManager.FindReward(user);
                
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
        /// <summary>
        /// gets sum of total earned rewards & toggle switch
        /// for theUI elements
        /// </summary>
        private async Task SetUpEarningsDisplay()
        {
               var (toggle, totalSum) = await rewardManager
                   .GetTotalRewards(user,user.Id);
               
               TotalEarnedDisplay = totalSum.ToString("F");
               if (firstSetup)
               {
                   ToggleStateDisplay = toggle;
                   if (toggle)
                   {
                       await StartDataReceive();
                   }
               }
        }
        /// <summary>
        /// This method gets called by EventHandler when received info
        /// via bluetooth (BleManager).
        /// It increments Circular progress bar view
        /// </summary>
        private async void InferredRawData(object s, EventArgs eventArgs)
        {
            await Step();
        }

        /// <summary>
        /// Starts/Stops connect/receiving data from the BleManager/Watch
        /// </summary>
        private async Task StartDataReceive()
        {
            if (++c % 2 == 0) return;
            
            CircularWaitDisplay = true;
            switchState = await bleManager.ToggleMonitoring(toggleState);
            ToggleStateDisplay = switchState;
            CircularWaitDisplay = false;
        }
        /// <summary>
        /// Resets circular progress bar view
        /// </summary>
        private void ResetView()
        {
            ProgressBarDisplay = StaticOpt.StepNeeded;
            StepsDisplayLabel = 0;
            stepsDisplayValue = 0;
        }
        /// <summary>
        /// initializes the circular progress bar view
        /// </summary>
        /// <param name="steps">number of steps double</param>
        private void SetUpView(double steps)
        {
            ProgressBarDisplay = StaticOpt.StepNeeded - steps;
            StepsDisplayLabel = steps;
            stepsDisplayValue = steps;
        }
        /// <summary>
        /// Increments the progress bar view and the label display
        /// </summary>
        private async Task Step()
        {
            ProgressBarDisplay--;
            StepsDisplayLabel=stepsDisplayValue+1;
            if (ProgressBarDisplay <= Max)
            {
                CircularWaitDisplay = true;
                ProgressBarDisplay = StaticOpt.StepNeeded;
                StepsDisplayLabel = 0;
                await SetUpEarningsDisplay();
                CircularWaitDisplay = false;

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
            rewardManager.Dispose();
        }
    }
}