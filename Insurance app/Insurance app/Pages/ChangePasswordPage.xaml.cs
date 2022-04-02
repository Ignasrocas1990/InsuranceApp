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

        protected override void OnAppearing()
        {
            base.OnAppearing();
            App.WasPaused = false;
        }

        /// <summary>
        /// When leaving page release Realm instance
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (App.WasPaused) return;
            ((ChangePassViewModel)BindingContext).Dispose();
        }
        /// <summary>
        /// user password validation
        /// </summary>
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