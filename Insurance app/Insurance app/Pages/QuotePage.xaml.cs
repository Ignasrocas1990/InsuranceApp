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
            BindingContext = new QuoteViewModel(this, Navigation);
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