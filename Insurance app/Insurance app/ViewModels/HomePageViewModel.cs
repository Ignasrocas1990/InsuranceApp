using System;
using System.Timers;
using Insurance_app.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class HomePageViewModel : ObservableObject
    {
        public INotification notification { set; get; }


        private Timer time = new Timer();
        private bool timerRunning;
        private double _ProgressValue;
        public HomePageViewModel()
        {
            StartTimer();
        }
        public double ProgressValue
        {
            get => _ProgressValue;
            set => SetProperty(ref _ProgressValue, value);
        }

        private double _Minimum;
        public double Minimum
        {
            get
            {
                return _Minimum;
            }
            set
            {
                _Minimum = value;
                OnPropertyChanged();
            }
        }
        private double _Maximum;
        public double Maximum
        {
            get
            {
                return _ProgressValue;
            }
            set
            {
                _ProgressValue = value;
                OnPropertyChanged();
            }
        }

        public void StartTimer()
        {
            Minimum = 0;
            Maximum = 60;
            ProgressValue = 60;
            timerRunning = true;
            time.Start();
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                if (ProgressValue > Minimum)
                {
                    ProgressValue--;
                    return true;
                }
                else if (ProgressValue == Minimum)
                {
                    time.Stop();
                    timerRunning = false;
                    //PerformOperationHere logic operation here.
                    return false;
                }
                else
                {
                    return true;
                }
            });
        }

    }
}