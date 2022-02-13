using Insurance_app.Models;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class RegistrationPage : ContentPage
    {
        public RegistrationPage()
        {
            InitializeComponent();
            //BindingContext = new RegistrationViewModel();
        }
    }
}