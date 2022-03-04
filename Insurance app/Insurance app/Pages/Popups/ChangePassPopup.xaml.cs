using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels.Popups;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ChangePassPopup
    {
        public ChangePassPopup(string email)
        {
            InitializeComponent();
            BindingContext = new ChangePassViewModel(this,email);
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