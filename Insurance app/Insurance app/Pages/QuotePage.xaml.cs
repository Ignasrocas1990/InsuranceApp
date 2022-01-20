using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Insurance_app.BLE;
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
            //inference = new InferenceService();

        }
        
        /*
        private async void GetQuoteBtn_OnClicked(object sender, EventArgs e)
        {
            //get data from entries, create a dictionary -> json  (look it up) and pass by predict
            // check the data before sending off
            var quote = new Dictionary<string, int>()
            {
                {"Hospitals",0},
                {"Age",18},
                {"Cover",0},
                {"Hospital_Excess",150},
                {"Plan",0},
                {"Smoker",0}
            };
            

                
            
            string price = "99.99 emptyf";

            CircularWait.IsVisible=true;
            
            var result = await inference.Predict(quote);
            price =  await result.Content.ReadAsStringAsync();
            
            CircularWait.IsVisible=false;
            bool action = await DisplayAlert("Price",$"{price}",  "Accept","Deny");
            if (action)
            {
                await Navigation.PushAsync(new RegistrationPage(price,quote));
            }
            Console.WriteLine(action);
        }
        */

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