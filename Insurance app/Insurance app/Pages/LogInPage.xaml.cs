using System.Threading.Tasks;
using Insurance_app.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LogInPage : ContentPage,INotification
    {
        public LogInPage()
        {
            InitializeComponent();
            BindingContext = new LogInViewModel(this,Navigation);
            
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