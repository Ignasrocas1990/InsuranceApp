using System;
using System.Linq;
using System.Text;
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
        public PaymentPage(string customerId, double price, string zip)
        {
            InitializeComponent();
            BindingContext = new PaymentViewModel(customerId,price,zip);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (PaymentViewModel)BindingContext;
            HeroImage.Source = ImageService.Instance.CardFront;
            await vm.Setup();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var vm = (PaymentViewModel)BindingContext;
            vm.Dispose();
        }


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

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = (PaymentViewModel)BindingContext;
                var expiryDate = vm.IsValid();
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