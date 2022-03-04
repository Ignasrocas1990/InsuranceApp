
using System;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuotePage : LoadingPage
    {
        public QuotePage()
        {
            InitializeComponent();
            BindingContext = new QuoteViewModel();
        }
    }
}