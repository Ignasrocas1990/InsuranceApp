﻿
using System;
using System.Linq;
using System.Text;
using Insurance_app.SupportClasses;
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
            
            //BindingContext = (ClaimViewModel) ShellViewModel.GetInstance()
            //   .GetViewModel(Converter.ClaimViewModel);
            //BindingContext = new ClaimViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (ClaimViewModel)BindingContext;
             await vm.SetUp();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var vm = (ClaimViewModel)BindingContext;
            vm.ClaimManager.Dispose();
        }

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