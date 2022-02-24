using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels.ClientViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.ClientPages
{
   // [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientRegistration : ContentPage
    {
        public ClientRegistration()
        {
            InitializeComponent();
        }

        private async void OnClickedRegister(object sender, EventArgs e)
        {
            try
            {
                var vm = (ClientRegViewModel) BindingContext;
                if (CodeValidator.IsValid && vm.CodeReadOnly == false)
                {
                    vm.CircularWaitDisplay = true;
                    await vm.ValidateCode();
                    vm.CircularWaitDisplay = false;
                }

                if (EmailValidation.IsValid && PasswordValidator.IsValid && NameValidator.IsValid && LNameValidator.IsValid)
                {
                    await vm.Register();
                }
                else
                {
                    var errBuilder = new StringBuilder();
                    if (!EmailValidation.IsNotValid)
                    {
                        errBuilder.AppendLine("Email is not valid");
                    }

                    if (PasswordValidator.IsNotValid)
                    {
                        if (PasswordValidator.Errors != null)
                            foreach (var err in PasswordValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err.ToString());
                            }
                    }

                    if (NameValidator.IsNotValid)
                    {
                        if (NameValidator.Errors != null)
                            foreach (var err in NameValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err.ToString());
                            }
                    }

                    if (LNameValidator.IsNotValid)
                    {
                        if (LNameValidator.Errors != null)
                            foreach (var err in LNameValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err.ToString());
                            }
                    }

                    if (CodeValidator.IsNotValid)
                    {
                        if (CodeValidator.Errors != null)
                            foreach (var err in CodeValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err.ToString());
                            }
                    }

                    await Application.Current.MainPage.DisplayAlert("Error", errBuilder.ToString(), "close");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}