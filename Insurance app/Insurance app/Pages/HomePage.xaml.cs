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
        private HomeViewModel viewModel;

        public HomePage()
        {
            InitializeComponent();

             viewModel = (HomeViewModel) ShellViewModel.GetInstance()
                .GetViewModel(Converter.HomeViewModel);
             BindingContext = viewModel;
             viewModel.Setup();            //circular wait

        }

        protected override void OnAppearing()
        {           
          
            base.OnAppearing();
            
        }

        private void Switch_OnToggled(object sender, ToggledEventArgs e)
        {
            viewModel.StartDataReceive(e.Value);
        }
    }
}