using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.ViewModels.ClientViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.ClientPages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PreviousPolicyPopup : Popup
    {
        public PreviousPolicyPopup(List<Policy> previousPolicies)
        {
            InitializeComponent();
            BindingContext = new PPolicyPopupViewModel(this,previousPolicies);
        }
    }
}