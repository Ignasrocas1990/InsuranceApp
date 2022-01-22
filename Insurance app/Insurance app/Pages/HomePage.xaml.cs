using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HomePage : ContentPage, INotification
    {
        public HomePage()
        {
            InitializeComponent();
            
             var viewModel = ((HomePageViewModel) ShellViewModel.GetInstance()
                    .GetViewModel("HomePageViewModel"));
             viewModel.notification = this;
             BindingContext = viewModel;

        }

        public async Task Notify(string title, string message, string close)
        {
            await DisplayAlert(title, message, close);
        }

        public async Task<bool> NotifyOption(string title, string message, string accept, string close)
        {
           return await DisplayAlert(title, message, accept,close);
        }
    }
}