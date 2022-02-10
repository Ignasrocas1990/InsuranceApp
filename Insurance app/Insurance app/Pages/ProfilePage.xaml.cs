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
            BindingContext = (ProfileViewModel) ShellViewModel.GetInstance()
                .GetViewModel(Converter.ProfileViewModel);
            
        }

        protected override async void OnAppearing()
        {
            var vm = (ProfileViewModel) BindingContext;
            await vm.Setup();
            base.OnAppearing();
        }
    }
}