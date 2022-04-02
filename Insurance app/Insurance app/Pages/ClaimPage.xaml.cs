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
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClaimPage : LoadingPage
    {
        public ClaimPage()
        {
            InitializeComponent();
            BindingContext  = new ClaimViewModel();
        }
        /// <summary>
        /// Load in current open claim
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            App.WasPaused = false;
            await ((ClaimViewModel)BindingContext).SetUp();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (App.WasPaused) return;
            ((ClaimViewModel)BindingContext).Dispose();
        }

        /// <summary>
        /// Validate user inputs
        /// </summary>
        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = (ClaimViewModel) BindingContext;
                if (HospitalCodeValidator.IsValid && PatientNrValidator.IsValid)
                {
                    vm.CreateClaimCommand.Execute(null);
                }
                else
                {
                    var errBuilder = new StringBuilder();
                    if (HospitalCodeValidator.IsNotValid && HospitalCodeValidator.Errors != null)
                    {
                        foreach (var err in HospitalCodeValidator.Errors.OfType<string>())
                        {
                            errBuilder.AppendLine(err);
                        }
                    }
                    if (PatientNrValidator.IsNotValid && PatientNrValidator.Errors != null)
                    {
                        foreach (var err in PatientNrValidator.Errors.OfType<string>())
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