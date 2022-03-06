using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insurance_app.Models;
using Insurance_app.ViewModels;
using Insurance_app.ViewModels.ClientViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;
using Exception = System.Exception;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage:LoadingPage
    {
        public RegistrationPage(Dictionary<string, string> tempQuote, string price)
        {
            try
            {
                InitializeComponent();
                BindingContext = new RegistrationViewModel(tempQuote,price);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var vm = (RegistrationViewModel)BindingContext;
            vm.UserManager.Dispose();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
             try
             {
                 var vm = (RegistrationViewModel)BindingContext;
                
                 if (EmailValidation.IsValid && PasswordValidator.IsValid && NameValidator.IsValid &&
                     LNameValidator.IsValid && PhoneNrValidator.IsValid && AddressValidator.Text.Equals(vm.AddressSText))
                 {
                     await vm.Register();
                 }
                 else
                 {
                     var errBuilder = new StringBuilder();
                     if (EmailValidation.IsNotValid)
                     {
                         errBuilder.AppendLine("Email is not valid");
                     }

                     if (PasswordValidator.IsNotValid)
                     {
                         if (PasswordValidator.Errors != null)
                             foreach (var err in PasswordValidator.Errors.OfType<string>())
                             {
                                 errBuilder.AppendLine(err);
                             }
                     }

                     if (NameValidator.IsNotValid)
                     {
                         if (NameValidator.Errors != null)
                             foreach (var err in NameValidator.Errors.OfType<string>())
                             {
                                 errBuilder.AppendLine(err);
                             }
                     }

                     if (LNameValidator.IsNotValid)
                     {
                         if (LNameValidator.Errors != null)
                             foreach (var err in LNameValidator.Errors.OfType<string>())
                             {
                                 errBuilder.AppendLine(err);
                             }
                     }
                     if (PhoneNrValidator.IsNotValid)
                     {
                         if (PhoneNrValidator.Errors != null)
                             foreach (var err in PhoneNrValidator.Errors.OfType<string>())
                             {
                                 errBuilder.AppendLine(err);
                             }
                     }

                     if (!AddressValidator.Text.Equals(vm.AddressSText))
                     {
                         errBuilder.AppendLine(vm.AddressText);
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