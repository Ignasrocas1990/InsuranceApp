/*
    Copyright 2020,Ignas Rocas

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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Microcharts;
using Realms.Sync;

namespace Insurance_app.Logic
{
    /// <summary>
    /// Class used to connect between Database
    /// and UI, while processing Report
    /// </summary>
    public class ReportManager : IDisposable
    {
        
        private readonly RealmDb realmDb;
        public ReportManager()
        {
            realmDb = RealmDb.GetInstancePerPage();
        }
        /// <summary>
        /// Create Chart entries which needed to display chart
        /// For daily steps
        /// </summary>
        /// <param name="allMovData">List of movement data found</param>
        /// <returns>Dictionary Chart entries and their labels</returns>
        public Dictionary<string,int> CountDailyMovData(List<MovData> allMovData)
        {
            var chartEntries = new Dictionary<string, int>();
            try
            {
                var hourDif = 24;
                var now = DateTime.Now;
                var prev = DateTime.Now.AddHours(-hourDif);
                for (int i = 0; i < 7; i++)
                {
                    int count = 0;
                    var prev1 = prev;
                    var now1 = now;
                    count = allMovData.Count(m => m.DateTimeStamp <= now1 && 
                                                  m.DateTimeStamp > prev1 && m.DelFlag == false);
                    chartEntries.Add(now.DayOfWeek.ToString(),count);
                    now = prev;
                    prev = prev.AddHours(-hourDif);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            return chartEntries;
        }
        /// <summary>
        /// Create Chart entries which needed to display chart
        /// For weekly steps
        /// </summary>
        /// <param name="allMovData">List of movement data found</param>
        /// <returns>Dictionary Chart entries and their labels</returns>
        public Dictionary<string,int> CountWeeklyMovData(List<MovData> allMovData)
        {
            var chartEntries = new Dictionary<string, int>();
            try
            {
                var weekString = "";
                var days =(7 + (DateTime.Now.DayOfWeek - DayOfWeek.Monday)) % 7;
                var startOfTheWeek = DateTime.Today.AddDays(-days);
                
                for (int i = 0; i < 4; i++)
                {
                    int count = 0;
                    var endOfTheWeek = startOfTheWeek.AddDays(7);
                    count = allMovData.Count(m => 
                        m.DateTimeStamp >= startOfTheWeek && m.DateTimeStamp < endOfTheWeek);

                    weekString = i == 0 ? "This week" : $"Week {i}";
                    chartEntries.Add(weekString,count);
                    startOfTheWeek = startOfTheWeek.AddDays(-7);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            return chartEntries;
        }
        /// <summary>
        /// Reverse Chart entries so the current day is last,
        /// & input colours
        /// </summary>
        /// <param name="chartEntries">Daily Labels and entries</param>
        /// <returns>Number of empty entries & Reversed ChartEntry</returns>
        public Tuple<int,Stack<ChartEntry>> CreateDailyLineChart(Dictionary<string, int> chartEntries)
        {
            var emptyDaysCount = 0;
            var r = new Random();
            var today = true;
            var entries = new Stack<ChartEntry>();
            if (chartEntries != null)
            {
                foreach (var keyValuePair in chartEntries)
                {
                    var label = keyValuePair.Key;
                    float value = keyValuePair.Value;
                    value = r.Next(0, 20000);//TODO To show that it works this can be uncommented
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
            return Tuple.Create(emptyDaysCount,entries);
        }
        /// <summary>
        /// Reverse Chart entries so the current week is last,
        /// & input colours
        /// </summary>
        /// <param name="weeklyMovData">Weekly Labels and entries</param>
        /// <returns>Number of empty entries & Reversed ChartEntry</returns>
        public Tuple<int,Stack<ChartEntry>> CreateWeeklyLineChart(Dictionary<string, int> weeklyMovData)
        {
            var emptyDaysCount = 0;
            var r = new Random();
            var thisWeek = true;
            var entries = new Stack<ChartEntry>();
            if (weeklyMovData != null)
            {
                foreach (KeyValuePair<string, int> i in weeklyMovData)
                {
                    var label = i.Key;
                    float value = i.Value;
                    value = r.Next(0, 20000);//TODO To show that it works this can be uncommented
                    var color = StaticOpt.ChartColors[r.Next(0, StaticOpt.ChartColors.Length - 1)];
                    if (thisWeek)
                    {
                        label = "This Week";
                        thisWeek = false;
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
            return Tuple.Create(emptyDaysCount,entries);
        }
        /// <summary>
        /// Uses RealmDb helper to get Step Data list
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Customer/Client</param>
        /// <returns>List of found Step data</returns>
        public Task<List<MovData>> GetAllMovData(string customerId, User user)
        {
            return realmDb.GetAllMovData(customerId, user);
        }
        /// <summary>
        /// release allocated Realm instance
        /// </summary>
        public void Dispose()
        {
            realmDb.Dispose();
        }


    }
}