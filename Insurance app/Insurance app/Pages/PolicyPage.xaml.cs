using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PolicyPage : ContentPage
    {
        public PolicyPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            var vm = (PolicyViewModel)BindingContext;
            vm.CircularWaitDisplay = true;
            vm.Setup();
            vm.CircularWaitDisplay = false;
            base.OnAppearing();
        }
    }
}