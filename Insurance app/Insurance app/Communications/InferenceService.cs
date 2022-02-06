using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Insurance_app.Communications
{
    
    public class InferenceService
    {
        
        //private const string Url = "http://ec2-34-251-148-246.eu-west-1.compute.amazonaws.com/predict";
        private const string PredictUrl = "https://testRESTapi.pythonanywhere.com/predict";
        //private const string EmailUrl = "https://testRESTapi.pythonanywhere.com/email";
        private const string SecretCode = "#F12sd1";

        private HttpClient client;
        private Stopwatch w = new Stopwatch();
        private StringContent content=null;
        //Func<String,double>convertToDouble =  x => double.Parse(x, CultureInfo.InvariantCulture);

        public InferenceService()
        {
            client = new HttpClient();
        }
        

        public Task<HttpResponseMessage> Email(String email)
        {
            if (!App.Connected)return null;
            
            content = new StringContent(JsonConvert
                .SerializeObject(new Dictionary<string,string>(){{"email",email},{SecretCode,"#F12sd1"}})
                ,Encoding.UTF8, "application/json");
            
            if (content!=null && App.Connected)
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
                    App.Connected = false;
                       
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
            if (!App.NetConnection()) return null;

            content = new StringContent(JsonConvert.SerializeObject(quote),Encoding.UTF8, "application/json");
            if (content!=null && App.NetConnection())
            {
                try
                {
                    return client.PostAsync(PredictUrl, content);

                }
                catch (Exception e)
                {
                    Console.WriteLine($"fail to send { e }");
                    App.Connected = false;
                       
                }
                    
            }
            else
            {
                Console.WriteLine("error not connected");
            }
            return null;
            

        }
    }
}