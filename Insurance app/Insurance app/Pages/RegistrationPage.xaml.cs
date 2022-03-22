/*    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage:LoadingPage
    {
        public RegistrationPage(Dictionary<string, string> tempQuote, string price)
        {
            InitializeComponent(); 
            BindingContext = new RegistrationViewModel(tempQuote,price);
        }
        /// <summary>
        /// Registration user input validation
        /// </summary>
        private async void Button_OnClicked(object sender, EventArgs e)
        {
             try
             {
                 var vm = (RegistrationViewModel)BindingContext;
                
                 if (EmailValidation.IsValid && PasswordValidator.IsValid && NameValidator.IsValid &&
                     LNameValidator.IsValid && PhoneNrValidator.IsValid 
                     && AddressValidator.Text.Equals(vm.AddressSText) && vm.EmailConfirmedDisplay)
                 {
                     await vm.Register();
                 }
                 else
                 {
                     var errBuilder = new StringBuilder();
                     if (!vm.EmailConfirmedDisplay)
                     {
                         errBuilder.AppendLine("Email is not confirmed");
                     }
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
                     await Application.Current.MainPage.DisplayAlert(Msg.Error, errBuilder.ToString(), "close");
                 }
             }
             catch (Exception exception)
             {
                 Console.WriteLine(exception);
             }
        }

        private void VisualElement_OnUnfocused(object sender, FocusEventArgs e)
        {
            var vm = (RegistrationViewModel) BindingContext;
            if (EmailValidation.IsNotValid)
            {
                vm.EmailNotConfirmedDisplay = false;
                return;
            }
            vm.EmailNotConfirmedDisplay = true;
        }
    }
}