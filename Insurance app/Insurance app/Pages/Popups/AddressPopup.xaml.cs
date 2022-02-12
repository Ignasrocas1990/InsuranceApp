﻿using Insurance_app.Models;
using Insurance_app.ViewModels.Popups;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressPopup : Popup<Address>
    {
        public AddressPopup(Address address)
        {
            InitializeComponent();
            BindingContext = new AddressViewModel(this,address);
        }
    }
}