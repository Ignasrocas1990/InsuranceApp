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

using Insurance_app.ViewModels;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : LoadingPage
    {

        public HomePage()
        {
            InitializeComponent();

            //BindingContext = (HomeViewModel) ShellViewModel.GetInstance()
              //  .GetViewModel(Converter.HomeViewModel);

              BindingContext = new HomeViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (HomeViewModel)BindingContext;
            await vm.Setup();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var vm = (HomeViewModel)BindingContext;
            vm.Dispose();
        }
    }
}