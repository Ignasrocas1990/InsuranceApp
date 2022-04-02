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
     Code based on : https://damian.fyi/xamarin/2020/08/07/xamarin-stripe.html
 */

using System;
using System.Linq;
using System.Text;
using Insurance_app.Models;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PaymentPage : LoadingPage
    {
        private bool back;
        public PaymentPage(Customer customer)
        {
            InitializeComponent();
            BindingContext = new PaymentViewModel(customer);
        }
        /// <summary>
        /// Set default image on appearing
        /// </summary>
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            HeroImage.Source = ImageService.Instance.CardFront;
            await ((PaymentViewModel)BindingContext).Setup();
            App.WasPaused = false;

        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            if (App.WasPaused) return;
            ((PaymentViewModel)BindingContext).Dispose();
        }

        /// <summary>
        /// When focused on code field use's animation to flip the card
        /// And display different image
        /// </summary>
        private void FieldFocused(object sender, FocusEventArgs e)
        {
            var oldValue = back;
            back = e.IsFocused && e.VisualElement == VerificationCode;

            if (oldValue == back) return;

            var newImage = back ? ImageService.Instance.CardBack : ImageService.Instance.CardFront;

            var animation = new Animation();
            var rotateAnimation1 = new Animation(r => HeroImage.RotationY = r, 0, 90, finished: () =>
                HeroImage.Source = newImage);
            var rotateAnimation2 = new Animation(r => HeroImage.RotationY = r, 90, 0);
            animation.Add(0, 0.5, rotateAnimation1);
            animation.Add(0.5, 1, rotateAnimation2);
            animation.Commit(this, "rotateCard");
        }
        /// <summary>
        /// Payment input validation
        /// </summary>
        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = (PaymentViewModel)BindingContext;
                var expiryDate = vm.Valid();
                if (NumberValidator.IsValid && MonthValidator.IsValid && YearValidator.IsValid &&
                    SecurityCodeValidator.IsValid && expiryDate.Length<5)
                {
                    vm.PayCommand.Execute(null);
                }
                else
                {
                    var errBuilder = new StringBuilder();
                    if (expiryDate.Length>5)
                    {
                        errBuilder.Append(expiryDate);
                    }
                    if (NumberValidator.IsNotValid)
                    {
                        if (NumberValidator.Errors != null)
                            foreach (var err in NumberValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                    if (MonthValidator.IsNotValid)
                    {
                        if (MonthValidator.Errors != null)
                            foreach (var err in MonthValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                    if (YearValidator.IsNotValid)
                    {
                        if (YearValidator.Errors != null)
                            foreach (var err in YearValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                    if (SecurityCodeValidator.IsNotValid)
                    {
                        if (SecurityCodeValidator.Errors != null)
                            foreach (var err in SecurityCodeValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                    await Application.Current.MainPage.DisplayAlert(Msg.Error, errBuilder.ToString(), "close");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
        
    }
}