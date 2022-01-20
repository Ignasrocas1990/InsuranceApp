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
    public partial class RegistrationPage : ContentPage,INotification
    {
        public RegistrationPage(float price, Dictionary<string,int> tempQuote)
        {
            InitializeComponent();
            BindingContext = new RegistrationViewModel(this,Navigation,price,tempQuote);
        }
        
        public Task Notify(string title, string message, string close)
        {
            DisplayAlert(title, message, close);
            return Task.CompletedTask;
        }
        public async Task<bool> NotifyOption(string title, string message, string leftBtn, string rightBtn)
        {
            return await DisplayAlert(title, message, leftBtn, rightBtn);
        }
    }
}