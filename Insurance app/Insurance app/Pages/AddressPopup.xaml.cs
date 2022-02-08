﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.ViewModels;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
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