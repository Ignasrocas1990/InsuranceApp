using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Switch;
using Switch.Helpers;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : LoadingPage
    {

        public HomePage()
        {
            InitializeComponent();

            //BindingContext = (HomeViewModel) ShellViewModel.GetInstance()
              //  .GetViewModel(Converter.HomeViewModel);

              BindingContext = new HomeViewModel() {SetUpWaitDisplay = true};
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (HomeViewModel)BindingContext;
            await vm.Setup();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
        }
    }
}