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
        private Dictionary<string, int> chartEntries = null;
        public Stack<ChartEntry> Entries = new Stack<ChartEntry>();

        private bool firstSetup = true;

        private readonly ReportManager reportManager;

        public ReportViewModel()
        {
            reportManager = new ReportManager();
        }


        public async Task SetUp()
        {
            if (!firstSetup) return;
            firstSetup = false;
            var r = new Random();
            bool today = true;
            chartEntries = await reportManager.GetWeeksMovData(App.RealmApp.CurrentUser);
                
                if (chartEntries != null)
                {
                    int index = 6;
                    foreach (KeyValuePair<string, int> i in chartEntries)
                    {
                        var label = i.Key;
                        float value = (float)i.Value;
                        var color = StaticOptions.ChartColors[r.Next(0, StaticOptions.ChartColors.Length - 1)];
                        if (today)
                        {
                            label = "Today";
                            today = false;
                        }
                        if (value==0)
                        {
                            value = 0.0001f;
                            color = StaticOptions.White;
                        }
                        Entries.Push(new ChartEntry(value)
                            {
                                Label = label,
                                ValueLabel = $"{(int)value}",
                                Color = color
                            });
                        
                        index--;
                    }
                }
        }

        public void Dispose()
        {
            chartEntries = null;
            Entries = null;
            reportManager.Dispose();
        }
    }
}