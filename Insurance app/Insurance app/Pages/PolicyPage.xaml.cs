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
    public partial class PolicyPage : LoadingPage
    {
        public PolicyPage()
        {
            InitializeComponent();
            var vm = new PolicyViewModel {SetUpWaitDisplay = true};
            BindingContext = vm;
        }
        protected override async void OnAppearing()
        {
            var vm = (PolicyViewModel)BindingContext;
            await vm.Setup();
            base.OnAppearing();
        }
    }
}