using System;
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
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            var vm = (ProfileViewModel) BindingContext;
            await vm.Setup();
            base.OnAppearing();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = (ProfileViewModel) BindingContext;
                if (NameValidator.IsValid && LNameValidator.IsValid && PhoneNrValidator.IsValid)
                {
                    vm.UpdateCommand.Execute(null);
                }
                else
                {
                    var errBuilder = new StringBuilder();
                    
                    if (NameValidator.IsNotValid)
                    {
                        if (NameValidator.Errors != null)
                            foreach (var err in NameValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }

                    if (LNameValidator.IsNotValid)
                    {
                        if (LNameValidator.Errors != null)
                            foreach (var err in LNameValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }

                    if (PhoneNrValidator.IsNotValid)
                    {
                        if (PhoneNrValidator.Errors != null)
                            foreach (var err in PhoneNrValidator.Errors.OfType<string>())
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