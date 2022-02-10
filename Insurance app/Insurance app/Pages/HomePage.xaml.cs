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

            BindingContext = (HomeViewModel) ShellViewModel.GetInstance()
                .GetViewModel(Converter.HomeViewModel);

             //BindingContext   = new HomeViewModel();
        }

        protected override async void OnAppearing()
        {
            var vm = (HomeViewModel)BindingContext;
             CircularWait.IsRunning = true;
             await vm.Setup();                      //circular wait
             CircularWait.IsRunning = false;
             base.OnAppearing();
            
        }

        private void Switch_OnToggled(object sender, ToggledEventArgs e)
        {
            var vm = (HomeViewModel)BindingContext;
            vm.StartDataReceive(e.Value);
        }
    }
}