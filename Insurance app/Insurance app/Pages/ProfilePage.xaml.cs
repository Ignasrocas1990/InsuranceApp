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
using System.Linq;
using System.Text;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    /// <summary>
    /// The class InitializeComponents GUI components and
    /// the sets up the view as it appears/disappears
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProfilePage : LoadingPage
    {
       
        public ProfilePage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }
        /// <summary>
        /// Load in customer details
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await ((ProfileViewModel) BindingContext).Setup();
            App.WasPaused = false;
        }
        /// <summary>
        /// When page disappearing checks if it was
        /// paused and disposes the realm instance. 
        /// </summary>
        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (App.WasPaused) return;
            ((ProfileViewModel) BindingContext).Dispose();
        }

        /// <summary>
        /// Validation of customer inputs
        /// </summary>
        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = (ProfileViewModel) BindingContext;
                if (NameValidator.IsValid && LNameValidator.IsValid && PhoneNrValidator.IsValid)
                {
                    vm.UpdateCommand.Execute(null);
                }
                else
                {
                    var errBuilder = new StringBuilder();
                    
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