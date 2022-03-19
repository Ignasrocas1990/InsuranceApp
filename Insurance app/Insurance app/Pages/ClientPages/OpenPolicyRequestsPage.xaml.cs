﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels.ClientViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.ClientPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OpenPolicyRequestsPage : LoadingPage
    {
        public OpenPolicyRequestsPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (OpenPolicyRViewModel)BindingContext;
            await vm.Setup();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            ((OpenPolicyRViewModel)BindingContext).Dispose();
        }
    }
}