using System;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : LoadingPage
    {
        public LogInPage()
        {
            InitializeComponent();
            BindingContext = new LogInViewModel();

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (LogInViewModel)BindingContext;
            vm.SetUpWaitDisplay = true;
            await vm.ExistUser();
            vm.SetUpWaitDisplay = false;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((LogInViewModel)BindingContext).Dispose();
        }
    }
}