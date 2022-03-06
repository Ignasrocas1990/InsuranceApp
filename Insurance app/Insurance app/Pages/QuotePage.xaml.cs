
using System;
using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuotePage : LoadingPage
    {
        public QuotePage(string policyId)
        {
            InitializeComponent();
            BindingContext = new QuoteViewModel(policyId);
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var vm = (QuoteViewModel)BindingContext;
            await vm.SetUp();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var vm = (QuoteViewModel)BindingContext;
            vm.SetUpWaitDisplay = true;
            vm.UserManager.Dispose();
        }
    }
}