using System;
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
        private List<MovData> newMovDataList;
        private Customer customer;




        private double currentSteps = 0;// set this to len of the mov data array
        private double currentProgressBars;
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
                newMovDataList = new List<MovData>();
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
                        ProgressBarDisplay = StaticOptions.StepNeeded - movLen;
                        StepsDisplay = movLen;
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
            Task.FromResult(AddMov(e.x, e.y,e.z, e.Type, e.TimeOffset));
            Step();
        }
        public async Task AddMov(float x, float y,float z, int type, DateTimeOffset time)
        {
            //here we need to count (if mov data is <=10000)
            try
            {
                if (ProgressBarDisplay <= 0)
                {
                    //cicular wait here
                    await customer.CreateReward();
                    ProgressBarDisplay = StaticOptions.StepNeeded;
                    //
                }
                var currMovData = new MovData()
                {
                    AccData = new Acc()
                    {
                        X = x,
                        Y = y,
                        Z = z
                    },
                    DateTimeStamp = time,
                    Type = "step",
                    Partition = App.RealmApp.CurrentUser.Id
                };
                newMovDataList.Add(currMovData);

                if (newMovDataList.Count > 4)
                {
                    //might need to initialize new MovData observable if still gives out-------------------------
                    //or re-create new Realm
                    RealmDb realmDb = new RealmDb();
                    realmDb.AddMovData2(new List<MovData>(newMovDataList),App.RealmApp.CurrentUser);
                    newMovDataList.Clear();
                    //movManager.AddMovData(newMovDataList,reward);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"failed to save mov data {e}");
            }
            
            
            //else we create new reward
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