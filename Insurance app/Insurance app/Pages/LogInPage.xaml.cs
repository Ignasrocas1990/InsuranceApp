using System;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage
    {
        public LogInPage()
        {
            InitializeComponent();
            /*
            var vModel = ((LogInViewModel) ShellViewModel.GetInstance()
                .GetViewModel(Converter.LogInViewModel));
         */
            BindingContext = new LogInViewModel();
        }
        
        public async Task Notify(string title, string message, string button)
        {
           await DisplayAlert(title, message, button);
            //return Task.CompletedTask;
        }

        public Task<bool> NotifyOption(string title, string message, string accept, string close)
        {
            return DisplayAlert(title, message, accept, close);

        }
    }
}