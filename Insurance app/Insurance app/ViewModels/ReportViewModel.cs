using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Microcharts;
using SkiaSharp;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Insurance_app.ViewModels
{
    public class ReportViewModel:ObservableObject
    {
        private Chart lineChart;
        private Dictionary<string, int> chartEntries = null;
        private static string[] labels = {"day 1", "day 2", "day 3", "day 4", "day 5", "day 6", "day 7"};
        private static float[] values = {0, 0, 0, 0, 0, 0, 0};
        private bool FirstSetup = true;

        private ReportManager reportManager;
        public ReportViewModel()
        {
            reportManager = new ReportManager();
        }

        public async Task SetUp()
        {
            if (!FirstSetup) return;
            FirstSetup = false;
            //circular wait
            chartEntries = await reportManager.GetWeeksMovData(App.RealmApp.CurrentUser);
            if (chartEntries != null)
            {
                int d = 6;
                foreach (KeyValuePair<string, int> i in chartEntries)
                {
                    Console.WriteLine("Key: {0}, Value: {1}", i.Key, i.Value);
                    labels[d] = i.Key;
                    values[d] = (float)i.Value;
                    d--;

                }
            }
            lineChart = new LineChart
            {
                Entries = _entry,
                LineMode = LineMode.Straight,
                LineSize = 8,
                PointMode = PointMode.Square,
                LabelOrientation = Orientation.Horizontal,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelTextSize = 30,
                PointSize = 18,
            };
            //circular wait----------------------------------------
        }
        public Chart ChartEntries
        {
            get => lineChart;
            set => SetProperty(ref lineChart, value);
        }
        private ChartEntry[] _entry = new[]
        {
            new ChartEntry(values[0])
            {
                Label = labels[0],
                ValueLabel = "112",
                Color = SKColor.Parse("#2c3e50")
            },
            new ChartEntry(values[1])
            {
                Label = labels[1],
                ValueLabel = "648",
                Color = SKColors.Yellow
            },
            new ChartEntry(values[2])
            {
                Label = labels[2],
                ValueLabel = "428",
                Color = SKColors.LightBlue
            },
            new ChartEntry(values[3])
            {
                Label = labels[3],
                ValueLabel = "214",
                Color = SKColors.Blue
            },
            new ChartEntry(values[4])
            {
            Label = labels[4],
            ValueLabel = "214",
            Color = SKColors.LimeGreen
            },
            new ChartEntry(values[5])
            {
            Label = labels[5],
            ValueLabel = "214",
            Color = SKColors.Orange
            },
            new ChartEntry(values[6])
            {
                Label = labels[6],
                ValueLabel = "214",
                Color = SKColors.Red
            }
            
        };

        

    }
}