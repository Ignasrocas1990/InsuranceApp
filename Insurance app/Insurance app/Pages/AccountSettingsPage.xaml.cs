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

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            var vm = (AccountSettingsViewModel)BindingContext;
                
            if ( PasswordValidator.IsValid && RePasswordValidator.IsValid)
            {
                vm.ChangePassCommand.Execute(null);
            }
            else
            {
                var errBuilder = new StringBuilder();

                if (RePasswordValidator.IsNotValid)
                {
                    errBuilder.AppendLine("Passwords do not match");
                   
                }
                if (PasswordValidator.IsNotValid)
                {
                    if (PasswordValidator.Errors != null)
                        foreach (var err in PasswordValidator.Errors.OfType<string>())
                        {
                            errBuilder.AppendLine(err);
                        }
                }
                await Application.Current.MainPage.DisplayAlert(
                    "Error", errBuilder.ToString(), "close");
            }
        }
    }
}