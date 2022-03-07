using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.SupportClasses;
using Microcharts;
using SkiaSharp;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    [QueryProperty(nameof(CustomerId), "CustomerId")]
    public class ReportViewModel : ObservableObject,IDisposable
    {
        public readonly ReportManager ReportManager;
        private string customerId = "";
        public ReportViewModel()
        {
            ReportManager = new ReportManager();
        }
        
        public async Task SetUp()
        {
            if (CustomerId.Equals(""))
            {
                customerId = App.RealmApp.CurrentUser.Id;
            }
            var allMovData = await ReportManager.GetAllMovData(customerId, App.RealmApp.CurrentUser);
            if (customerId == App.RealmApp.CurrentUser.Id)
            {
                var dailyMovData =  ReportManager.CountDailyMovData(allMovData);
                var (emptyDaysCount, entries) = ReportManager.CreateDailyLineChart(dailyMovData);

                if (emptyDaysCount==7)
                {
                    WeekChartLabel = "No step has been taken yet this week";
                    WeekChartIsVisible = false;
                    return;
                }
                LineChart = new LineChart()
                    {Entries = entries, LabelTextSize = 30f,ValueLabelTextSize = 30f};
                
                WeekChartLabel = "Step done last 7 days";
                WeekChartIsVisible = true;
            }
            var weeklyMovData =ReportManager.CountWeeklyMovData(allMovData);
            //createWeeklyLineChart

        }
        
       
        private LineChart lineChart;
        public LineChart LineChart
        {
            get => lineChart;
            set => SetProperty(ref lineChart, value);
        }
        private bool setUpWait;
        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }

        private string weekChartLabel;
        public string WeekChartLabel
        {
            get => weekChartLabel;
            set => SetProperty(ref weekChartLabel, value);

        }

        private bool weekChartIsVisible;
        public bool WeekChartIsVisible
        {
            get => weekChartIsVisible;
            set => SetProperty(ref weekChartIsVisible, value);
        }
        private bool wait;
        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        public string CustomerId
        {
            get => customerId;
            set =>  customerId = Uri.UnescapeDataString(value ?? string.Empty);

        }

        public void Dispose()
        {
            ReportManager.Dispose();
        }
    }
}