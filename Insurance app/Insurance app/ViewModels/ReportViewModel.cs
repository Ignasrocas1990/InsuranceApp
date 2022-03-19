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
        private readonly ReportManager reportManager;
        private string customerId = "";
        public ReportViewModel()
        {
            reportManager = new ReportManager();
        }
        
        public async Task SetUp()
        {
            if (CustomerId.Equals(""))
            {
                customerId = App.RealmApp.CurrentUser.Id;
            }
            var allMovData = await reportManager.GetAllMovData(customerId, App.RealmApp.CurrentUser);
            if (customerId == App.RealmApp.CurrentUser.Id)
            {
                var dailyMovData =  reportManager.CountDailyMovData(allMovData);
                var (emptyDaysCount, entries) = reportManager.CreateDailyLineChart(dailyMovData);

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

            if ((WeekChartIsVisible && CustomerId == App.RealmApp.CurrentUser.Id) || CustomerId != App.RealmApp.CurrentUser.Id)
            {
                var weeklyMovData =reportManager.CountWeeklyMovData(allMovData);
                var (emptyWeekCount, weeklyEntries) = reportManager.CreateWeeklyLineChart(weeklyMovData);
            
                if (emptyWeekCount>2)
                {
                    WeekChartLabel = "No steps has been taken this month"; 
                    WeekChartIsVisible = false;
                    return;
                }
                WeeklyLineChart = new LineChart()
                    {Entries = weeklyEntries, LabelTextSize = 30f,ValueLabelTextSize = 30f};
                
                WeeklyChartLabel = "Steps in the last month";
                WeeklyChartIsVisible = true;  
            }
            
            //createWeeklyLineChart

        }

        private bool weeklyChartVisible;
        public bool WeeklyChartIsVisible
        {
            get => weeklyChartVisible;
            set => SetProperty(ref weeklyChartVisible, value);
        }

        private string weeklyLabel;
        public string WeeklyChartLabel
        {
            get => weeklyLabel;
            set => SetProperty(ref weeklyLabel, value);
        }
        

        private LineChart lineChart;
        public LineChart LineChart
        {
            get => lineChart;
            set => SetProperty(ref lineChart, value);
        }
        private LineChart weeklyLineChart;
        public LineChart WeeklyLineChart
        {
            get => weeklyLineChart;
            set => SetProperty(ref weeklyLineChart, value);
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
            reportManager.Dispose();
        }
    }
}