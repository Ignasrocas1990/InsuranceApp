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

using Insurance_app.ViewModels;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    /// <summary>
    /// The class InitializeComponents GUI components and
    /// the sets up the view as it appears/disappears
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : LoadingPage
    {
        public LogInPage()
        {
            InitializeComponent();
            BindingContext = new LogInViewModel();

        }
        /// <summary>
        /// Removes any previous log user.
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (LogInViewModel)BindingContext;
            vm.SetUpWaitDisplay = true;
            await vm.ExistUser();
            vm.SetUpWaitDisplay = false;
            App.WasPaused = false;

        }
        /// <summary>
        /// When page is disappearing(Switched)
        /// Dispose the Realm instance if
        /// the app was not paused
        /// </summary>
        protected override void OnDisappearing()
        {
            //check if it was paused
            base.OnDisappearing();
            if (App.WasPaused) return;
            ((LogInViewModel)BindingContext).Dispose();

        }
    }
}