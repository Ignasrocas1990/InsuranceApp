/*
    Copyright 2020,Ignas Rocas

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
using System.Linq;
using System.Text;
using Insurance_app.ViewModels.ClientViewModels;
using Xamarin.Forms;

namespace Insurance_app.Pages.ClientPages
{
    public partial class ClientRegistration : LoadingPage
    {
        public ClientRegistration()
        {
            InitializeComponent();
        }
        /// <summary>
        /// When leaving page release Realm instance
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((ClientRegViewModel) BindingContext).Dispose();
        }

        /// <summary>
        /// Register page validation onclick
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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