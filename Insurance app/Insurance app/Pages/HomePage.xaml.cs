﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage
    {
        public static Switch MySwitch;

        public HomePage()
        {
            InitializeComponent();

            var viewModel = (HomeViewModel) ShellViewModel.GetInstance()
                .GetViewModel(Converter.HomeViewModel);
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
        }

        private void Switch_OnToggled(object sender, ToggledEventArgs e)
        {
            var vm = (HomeViewModel) BindingContext;
            vm.StartDataReceive(e.Value);
        }
    }
}