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
    public partial class AccountSettingsPage : LoadingPage
    {
        public AccountSettingsPage()
        {
            InitializeComponent();
            BindingContext = new AccountSettingsViewModel{SetUpWaitDisplay = true};
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (AccountSettingsViewModel)BindingContext;
            await vm.SetUp();
        }
    }
}