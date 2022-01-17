using System;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        private Stopwatch w = new Stopwatch();
        private StringContent content=null;
        private Quote quote=null;
        //Func<String,double>convertToDouble =  x => double.Parse(x, CultureInfo.InvariantCulture);

        public InferenceService()
        {
            client = new HttpClient();
            connected=IsConnected();
            SubNetworkChange();
        }

        public void SubNetworkChange()
        {
            Connectivity.ConnectivityChanged += (s,e) =>
            {
                connected=IsConnected();
                Console.WriteLine($"network connection : : {connected}");
            };
        }
        /**
         * Take raw data from sensor and pass it by http to predict
         * if customer walking
         */
        public Task<HttpResponseMessage> Predict()
        {
            if (!connected)
            {
                return null;
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
            //Console.WriteLine(content);
            if (content!=null && connected)
            {
                try
                {
                    //HttpResponseMessage response = await client.PostAsync(Url, content);
                    return client.PostAsync(Url, content);
                    //return Task.FromResult(response);
                    //finRequest?.Invoke(this,response);
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
            return null;
            

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