﻿
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClaimPage : ContentPage
    {
        public ClaimPage()
        {
            InitializeComponent();
            
            BindingContext = (ClaimViewModel) ShellViewModel.GetInstance()
                .GetViewModel(Converter.ClaimViewModel);
        }

        protected override async void OnAppearing()
        {
            var vm = (ClaimViewModel)BindingContext;
            vm.CircularWaitDisplay = true;
             await vm.SetUp();
            vm.CircularWaitDisplay = false;
            base.OnAppearing();
        }
    }
}