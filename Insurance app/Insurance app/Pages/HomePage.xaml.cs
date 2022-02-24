using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {

        public HomePage()
        {
            InitializeComponent();

            //BindingContext = (HomeViewModel) ShellViewModel.GetInstance()
              //  .GetViewModel(Converter.HomeViewModel);

             //BindingContext   = new HomeViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (HomeViewModel)BindingContext;
             vm.SetUpWaitDisplay = true;
             await vm.Setup();
             await vm.SetUpEarningsDisplay();
             vm.SetUpWaitDisplay = false;

        }

        private async void Switch_OnToggled(object sender, ToggledEventArgs e)
        {
            var vm = (HomeViewModel)BindingContext;
            await vm.StartDataReceive();
        }
    }
}