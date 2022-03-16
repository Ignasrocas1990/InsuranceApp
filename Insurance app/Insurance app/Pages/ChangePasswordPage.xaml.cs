using System;
using System.Linq;
using System.Text;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePasswordPage : LoadingPage
    {
        public ChangePasswordPage()
        {
            InitializeComponent();
            BindingContext = new ChangePassViewModel();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var vm = (ChangePassViewModel)BindingContext;
            vm.Dispose();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            var vm = (ChangePassViewModel)BindingContext;
                
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