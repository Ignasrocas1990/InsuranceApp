using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.BLE;
using Insurance_app.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuotePage : ContentPage
    {
        private InferenceService inference;
        public QuotePage()
        {
            InitializeComponent();
            inference = new InferenceService();
        }
        

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

    }
}