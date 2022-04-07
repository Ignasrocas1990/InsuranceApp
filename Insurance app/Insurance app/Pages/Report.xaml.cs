﻿/*    Copyright 2020,Ignas Rocas

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

using Insurance_app.ViewModels;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    /// <summary>
    /// The class InitializeComponents GUI components and
    /// the sets up the view as it appears/disappears
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Report : LoadingPage
    {
        public Report()
        {
            InitializeComponent();
            BindingContext = new ReportViewModel();
        }
        /// <summary>
        /// Load in steps and create charts
        /// </summary>
        protected override async void OnAppearing()
        {
            var vm = (ReportViewModel) BindingContext;
            vm.SetUpWaitDisplay = true;
            await vm.SetUp();
            vm.SetUpWaitDisplay = false;
            base.OnAppearing();
            App.WasPaused = false;
        }
        /// <summary>
        /// When page disappearing checks if it was
        /// paused and disposes the realm instance. 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (App.WasPaused) return;
            ((ReportViewModel) BindingContext).Dispose();
        }
    }
}