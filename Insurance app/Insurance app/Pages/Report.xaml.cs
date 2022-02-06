using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Microcharts;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Report : ContentPage
    {
        private ReportViewModel vm;
        public Report()
        {
            InitializeComponent();
            vm = new ReportViewModel();
            BindingContext = vm;
        }

        protected override async void OnAppearing()
        {
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