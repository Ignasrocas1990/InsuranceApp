using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace Insurance_app.BLE
{
    
    public class InferenceService
    {
        
        //private const String Url = "http://ec2-34-251-148-246.eu-west-1.compute.amazonaws.com/predict";
        private const String Url = "https://testRESTapi.pythonanywhere.com/predict";
        private bool connected = false;
        private HttpClient client;
        private EventHandler<HttpResponseMessage> finRequest;
        private Stopwatch w = new Stopwatch();
        private StringContent content=null;
        private Quote quote=null;
        //Func<String,double>convertToDouble =  x => double.Parse(x, CultureInfo.InvariantCulture);

        public InferenceService()
        {
            client = new HttpClient();
            connected=IsConnected();
            SubNetworkChange();
            SubResponseReceived();
        }

        public void SubNetworkChange()
        {
            Connectivity.ConnectivityChanged += (s,e) =>
            {
                connected=IsConnected();
                Console.WriteLine($"network connection : : {connected}");
            };
        }

        private void SubResponseReceived()
        {
            finRequest += (s, e) =>
            {
                w.Stop();
                Console.WriteLine($"time it took to predict : {w.Elapsed.Milliseconds }");
                try
                {
                   
                    string jsonContent = e.Content.ReadAsStringAsync().Result;
                   
                    Console.WriteLine($"Prediction result is : {jsonContent}");
                   
                   
                   
                    // use below if response is sent in json 
                    //JObject json = JObject.Parse(jsonContent);
                    //Console.WriteLine(json["Prediction"]);
                }
                catch (Exception exception)
                {
                    Console.WriteLine($"error while reading the content {exception}");
                   
                }
                //DisplayAlert("notice", e.ToString(), "close");
            };
        }
        
        
        
        
        /**
         * Take raw data from sensor and pass it by http to predict
         * if customer walking
         */
        public async void Predict()
        {
            if (!connected)
            {
                return;
            }
            
            
            //var temp = rawData.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries);
            //if (temp.Length != 6) return;
            

            try
            {
                quote = new Quote()
                {
                    Hospitals = 0,
                    Age = 18,
                    Cover = 0,
                    Hospital_Excess = 150,
                    Plan = 0,
                    Smoker = 0
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"error {e} ");
                throw;
            }
            content = new StringContent(JsonConvert.SerializeObject(quote),Encoding.UTF8, "application/json");
            Console.WriteLine( await content.ReadAsStringAsync());
            SendRequestAsync();

        }

        private async void SendRequestAsync()
        {
            if (content!=null && connected)
            {
                try
                {
                    w.Start();
                    HttpResponseMessage response = await client.PostAsync(Url, content);
                    finRequest?.Invoke(this,response);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"fail to send { e }");
                    connected = false;
                       
                }
                    
            }
            else
            {
                Console.WriteLine("error not connected");

            }
        }
        private bool IsConnected() => (Connectivity.NetworkAccess == NetworkAccess.Internet);
    }
    class Quote
    {
        public int Hospitals { get; set; }
        public int Age { get; set; }
        public int Cover { get; set; }
        public int Hospital_Excess { get; set; }
        public int Plan { get; set; }
        public int Smoker  { get; set; }
        public float Price { get; set; }
    }
}