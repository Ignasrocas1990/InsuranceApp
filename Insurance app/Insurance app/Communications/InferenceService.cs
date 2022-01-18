using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;

namespace Insurance_app.BLE
{
    
    public class InferenceService
    {
        
        //private const string Url = "http://ec2-34-251-148-246.eu-west-1.compute.amazonaws.com/predict";
        private const string PredictUrl = "https://testRESTapi.pythonanywhere.com/predict";
        //private const string EmailUrl = "https://testRESTapi.pythonanywhere.com/email";
        private const string SecretCode = "#F12sd1";

        private bool connected = false;
        private HttpClient client;
        private Stopwatch w = new Stopwatch();
        private StringContent content=null;
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

        public Task<HttpResponseMessage> Email(String email)
        {
            if (!connected)return null;
            
            content = new StringContent(JsonConvert
                .SerializeObject(new Dictionary<string,string>(){{"email",email},{SecretCode,"#F12sd1"}})
                ,Encoding.UTF8, "application/json");
            
            if (content!=null && connected)
            {
                try
                {
                    //HttpResponseMessage response = await client.PostAsync(Url, content);
                    return client.PostAsync(PredictUrl, content);
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
        /**
         * Take raw data from sensor and pass it by http to predict
         * if customer walking
         */
        public Task<HttpResponseMessage> Predict(Dictionary<String,int>quote)
        {
            if (!connected)
            {
                return null;
            }
            
            content = new StringContent(JsonConvert.SerializeObject(quote),Encoding.UTF8, "application/json");
            //Console.WriteLine(content);
            if (content!=null && connected)
            {
                try
                {
                    //HttpResponseMessage response = await client.PostAsync(Url, content);
                    return client.PostAsync(PredictUrl, content);
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
    /*
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
    */
}