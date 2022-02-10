﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Report : ContentPage
    {
        public Report()
        {
            InitializeComponent();
            BindingContext = (ReportViewModel) ShellViewModel.GetInstance()
                .GetViewModel(Converter.ReportViewModel);
        }

        protected override async void OnAppearing()
        {
            var vm = (ReportViewModel) BindingContext;
            CircularWait.IsRunning = true;
            await vm.SetUp();
            
            MyChartView.Chart = new BarChart()
            {
                Entries = vm.Entries,
                LabelOrientation = Orientation.Vertical, 
                ValueLabelOrientation = Orientation.Horizontal,
                LabelTextSize = 30,
            };
            CircularWait.IsRunning = false;

            base.OnAppearing();

        }
    }
}