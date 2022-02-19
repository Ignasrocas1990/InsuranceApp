﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.ViewModels.Popups;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ClientRegPopUp : Popup<String>
    {
        public ClientRegPopUp(string clientCode)
        {
            BindingContext = new ClientRegPopupViewModel(this,clientCode);
        }
    }
}