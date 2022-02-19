using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;
using Newtonsoft.Json;

namespace Insurance_app.Communications
{
    
    public class InferenceService
    {
        

        
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
            if (!App.NetConnection())return null;
            
            content = new StringContent(JsonConvert
                .SerializeObject(new Dictionary<string,string>(){{"email",email}})
                ,Encoding.UTF8, "application/json");
            
            if (content!=null && App.NetConnection())
            {
                try
                {
                    return client.PostAsync(StaticOpt.EmailUrl, content);
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

        public Task<HttpResponseMessage> CheckCompanyCode(string code)
        {
            if (!App.NetConnection()) return null;
            content = new StringContent(JsonConvert
                    .SerializeObject(new Dictionary<string,string>(){{"code",code}})
                ,Encoding.UTF8, "application/json");
            
            if (content!=null && App.Connected)
            {
                try
                {
                    //HttpResponseMessage response = await client.PostAsync(Url, content);
                    return client.PostAsync(StaticOpt.CompanyCodeUrl, content);
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
                    return client.PostAsync(StaticOpt.PredictUrl, content);

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