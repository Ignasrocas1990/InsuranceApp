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
       // private Dictionary<string, int> chartEntries = null;
       
        private bool firstSetup = true;

        private readonly ReportManager reportManager;

        public ReportViewModel()
        {
            reportManager = new ReportManager();
        }


        public async Task SetUp()
        {
            var entries = new Stack<ChartEntry>();
            
            if (!firstSetup) return;
            firstSetup = false;
            var r = new Random();
            bool today = true;
            CircularDisplay = true;

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
                        }
                        entries.Push(new ChartEntry(value)
                            {
                                Color = color,
                                ValueLabel = $"{(int)value}",
                                Label = label

                            });
                    }
                }
                CircularDisplay = false;
                LineChart = new LineChart()
                    {Entries = entries, LabelTextSize = 30f,ValueLabelTextSize = 30f};

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


        public void Dispose()
        {
           // chartEntries = null;
            //Entries = null;
            reportManager.Dispose();
        }
    }
}