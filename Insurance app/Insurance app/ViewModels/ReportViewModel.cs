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
    public class ReportViewModel : ObservableObject,IDisposable
    {
        private readonly ReportManager reportManager;
        public ReportViewModel()
        {
            reportManager = new ReportManager();
        }
        
        public async Task SetUp()
        {
            int emptyDaysCount = 0;
            var entries = new Stack<ChartEntry>();
            
            var r = new Random();
            bool today = true;
            
            var chartEntries = await reportManager.GetWeeksMovData(App.RealmApp.CurrentUser);
                
                if (chartEntries != null)
                {

                    foreach (KeyValuePair<string, int> i in chartEntries)
                    {
                        var label = i.Key;
                        float value = i.Value;
                        //value = r.Next(0, 20000);//TODO To show that it works this can be uncommented
                        var color = StaticOpt.ChartColors[r.Next(0, StaticOpt.ChartColors.Length - 1)];
                        if (today)
                        {
                            label = "Today";
                            today = false;
                        }
                        if (value==0)
                        {
                            value = 0.0001f;
                            color = StaticOpt.White;
                            emptyDaysCount++;
                        }
                        entries.Push(new ChartEntry(value)
                            {
                                Color = color,
                                ValueLabel = $"{(int)value}",
                                Label = label
                            });
                    }
                }
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
       
        private LineChart lineChart;
        public LineChart LineChart
        {
            get => lineChart;
            set => SetProperty(ref lineChart, value);
        }

        private bool isRunning;
        public bool CircularDisplay
        {
            get => isRunning;
            set => SetProperty(ref isRunning, value);
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

        public void Dispose()
        {
           // chartEntries = null;
            //Entries = null;
            reportManager.Dispose();
        }
    }
}