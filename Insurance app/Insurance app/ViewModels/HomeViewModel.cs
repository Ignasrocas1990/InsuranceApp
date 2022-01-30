using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Java.Lang;
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
        private MovViewModel movViewModel;


        private double currentSteps = 0;// set this to len of the mov data array
        private double currentProgressBars;
        private double max = 0 ;
        private double min = StaticOptions.StepNeeded;
        
        public HomeViewModel()
        {
            StepCommand = new Command(Step);
            bleManager = BleManager.GetInstance();
            bleManager.InfferEvent += InferredRawData;
            Setup();
        }

        public async void Setup()
        {
            movViewModel =  (MovViewModel)ShellViewModel.GetInstance()
                .GetViewModel(Converter.MovViewModel);
            await movViewModel.Setup();
            InitStepsDisplay();
        }

        private void InitStepsDisplay()
        {
            double movLen = Convert.ToDouble(movViewModel.MovDataDisplay.Count);
            
            while (ProgressBarDisplay>=(StaticOptions.StepNeeded - movLen ))
            {
                Step();
                Thread.Sleep(10);
            }

        }

        private async void InferredRawData(object s, RawDataArgs e)
        {
            await movViewModel.AddMov(e.x, e.y,e.z, e.Type, e.TimeOffset);
            Step();
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
        public double ProgressBarDisplay
        {
            get => currentProgressBars;
            set =>  SetProperty(ref currentProgressBars, value);
        }
        public double StepsDisplay //the persantages lable in the middle
        {
            
            get => currentSteps / StaticOptions.StepNeeded * 100;
            set => SetProperty(ref  temp, value);
        }
        private double temp = 0;

    }
}