using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuotePage : ContentPage,INotification
    {
        public QuotePage()
        {
            InitializeComponent();
            var vModel = (QuoteViewModel)ShellViewModel.GetInstance()
                .GetViewModel(nameof(QuoteViewModel));
            vModel.notification = this;
            BindingContext = vModel;
        }
        public async Task Notify(string title, string message, string close)
        {
            await DisplayAlert(title, message, close);
            //return Task.CompletedTask;
        }

        public async Task<bool> NotifyOption(string title, string message, string accept, string close)
        {
           return await DisplayAlert(title, message, accept, close);
        }
    }
}