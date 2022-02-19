using System;
using System.Collections.Generic;
using Insurance_app.Models;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.Extensions;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegistrationPage
    {
        public RegistrationPage(Dictionary<string, int> tempQuote, string price)
        {
            try
            {
                InitializeComponent();
                BindingContext = new RegistrationViewModel(tempQuote,price);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
           
        }
    }
}