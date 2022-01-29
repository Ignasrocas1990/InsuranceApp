using System;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class HomeViewModel : ObservableObject
    {
        public static readonly double StepNeeded = 10000;
        
        public ICommand StepCommand { get; }
        public bool ToggleState = false;
        private BleManager bleManager;


        private double currentSteps = 0;
        private double currentProgressBars = StepNeeded;
        private double max = 0 ;
        private double min = StepNeeded;
        
        public HomeViewModel()
        {
            StepCommand = new Command(Step);
            bleManager = BleManager.GetInstance();
            bleManager.InfferEvent += InferredRawData;
            setup();
        }

        public void setup()
        {
            var customerVM =  (CustomerViewModel)ShellViewModel.GetInstance()
                .GetViewModel(Converter.CustomerViewModel);
            customerVM.Setup();
        }

        private void InferredRawData(object sender, RawDataArgs e)
        {
            //add to the collection
        }

        public void StartDataReceive(bool newValue)
        {
            if (newValue)
            {
                try
                {
                    bleManager.ToggleMonitoring();
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
                 bleManager.ToggleMonitoring();
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
        public double StepsDisplay
        {
            
            get => currentSteps / StepNeeded * 100;
            set => SetProperty(ref  temp, value);
        }
        private double temp = 0;

    }
}