using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Exceptions;
using Xamarin.CommunityToolkit.Extensions;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
            BindingContext = new RegistrationViewModel();
        }
        
        private async void VisualElement_OnFocused(object sender, FocusEventArgs e)
        {
            var vm = (RegistrationViewModel)BindingContext;
            
            vm.AddressDisplay = await Navigation.ShowPopupAsync<string>(new AddressPopup());
        }
    }
}