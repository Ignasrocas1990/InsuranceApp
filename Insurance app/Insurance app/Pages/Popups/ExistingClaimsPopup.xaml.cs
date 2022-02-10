using System.Collections.Generic;
using Insurance_app.Models;
using Insurance_app.ViewModels;
using Insurance_app.ViewModels.Popups;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExistingClaimsPopup : Popup
    {
        public ExistingClaimsPopup(List<Claim> existingClaims)
        {
            InitializeComponent();
            BindingContext = new EcPopUpViewModel(this,existingClaims);
        }
    }
}