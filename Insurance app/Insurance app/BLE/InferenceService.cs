using System;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json;
using Xamarin.Essentials;

namespace Insurance_app.BLE
{
    
    public class InferenceService
    {
        //private cost String Url = "http://ec2-54-228-141-181.eu-west-1.compute.amazonaws.com/predict";
        private const String Url = "https://testRESTapi.pythonanywhere.com/predict";
        private bool connected = false;
        private HttpClient client;
        private EventHandler<HttpResponseMessage> finRequest;
        private Stopwatch w = new Stopwatch();
        private StringContent content=null;
        private Sdata f=null;
        private int i = 0;

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

        public async void SubResponseReceived()
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
        public async void Predict(String rawData)
        {
            if (!connected)
            {
                return;
            }
            //var temp = rawData.Split(',');
            f = new Sdata()
            {
                Ax = 7.091625,
                Ay = -0.5916671,
                Az = 8.195502,
                Gx = 0.3149441,
                Gy = -1.0222765,
                Gz = -0.3099616
            };

            content = new StringContent(JsonConvert.SerializeObject(f),Encoding.UTF8, "application/json");
            Console.WriteLine( await content.ReadAsStringAsync());
                
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
    class Sdata
    {
        public double Ax { get; set; }
        public double Ay { get; set; }
        public double Az { get; set; }
        public double Gx { get; set; }
        public double Gy { get; set; }
        public double Gz  { get; set; }
    }
}