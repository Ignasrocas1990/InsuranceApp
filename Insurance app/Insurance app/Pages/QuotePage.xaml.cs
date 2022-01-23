using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuotePage : ContentPage
    {
        public QuotePage()
        {
            InitializeComponent();
            var vModel = (QuoteViewModel)ShellViewModel.GetInstance()
                .GetViewModel(nameof(QuoteViewModel));
            BindingContext = vModel;
        }
    }
}