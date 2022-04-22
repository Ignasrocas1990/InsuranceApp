/*   Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Microcharts;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    /// <summary>
    /// Class used to store and manipulate Report Page UI components in real time via BindingContext and its properties
    /// </summary>
    [QueryProperty(nameof(TransferredCustomerId), "TransferredCustomerId")]
    public class ReportViewModel : ObservableObject,IDisposable
    {
        private readonly ReportManager reportManager;
        private string customerId = "";
        public ReportViewModel()
        {
            reportManager = new ReportManager();
        }
        
        /// <summary>
        /// Loads in data using ReportManager class via database,
        /// and set it to Bindable properties(UI)
        /// </summary>
        public async Task SetUp()
        {
            DailyChartIsVisible = false;
            WeeklyChartIsVisible = false;

            if (TransferredCustomerId.Equals(""))
            {
                customerId = App.RealmApp.CurrentUser.Id;
            }
            else
            {
                customerId = TransferredCustomerId;
            }
            var allMovData = await reportManager.GetAllMovData(customerId, App.RealmApp.CurrentUser);
            if (customerId == App.RealmApp.CurrentUser.Id)
            {
                var dailyMovData =  reportManager.CountDailyMovData(allMovData);
                var (emptyDaysCount, entries) = reportManager.CreateDailyLineChart(dailyMovData);

                if (emptyDaysCount==7)
                {
                    DailyChartLabel = "No step has been taken yet this week";
                    return;
                }
                LineChart = new LineChart()
                    {Entries = entries, LabelTextSize = 30f,ValueLabelTextSize = 30f};
                
                DailyChartLabel = "Step done last 7 days";
                DailyChartIsVisible = true;
            }

            if ((DailyChartIsVisible && TransferredCustomerId == App.RealmApp.CurrentUser.Id) || TransferredCustomerId != App.RealmApp.CurrentUser.Id)
            {
                var weeklyMovData =reportManager.CountWeeklyMovData(allMovData);
                var (emptyWeekCount, weeklyEntries) = reportManager.CreateWeeklyLineChart(weeklyMovData);
            
                if (emptyWeekCount==4)
                {
                    WeeklyChartLabel = "No steps has been taken this month"; 
                    return;
                }
                WeeklyLineChart = new LineChart()
                    {Entries = weeklyEntries, LabelTextSize = 30f,ValueLabelTextSize = 30f};
                
                WeeklyChartLabel = "Steps in the last month";
                WeeklyChartIsVisible = true;  
            }
        }
        
        //------------------------- Bindable properties below ----------------------------------
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

        private string dailyChartLabel;
        public string DailyChartLabel
        {
            get => dailyChartLabel;
            set => SetProperty(ref dailyChartLabel, value);

        }

        private bool dailyChartIsVisible;
        public bool DailyChartIsVisible
        {
            get => dailyChartIsVisible;
            set => SetProperty(ref dailyChartIsVisible, value);
        }
        private bool wait;
        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        private string transferredId="";
        public string TransferredCustomerId
        {
            get => transferredId;
            set =>  transferredId = Uri.UnescapeDataString(value ?? string.Empty);

        }

        public void Dispose()
        {
            reportManager.Dispose();
        }
    }
}