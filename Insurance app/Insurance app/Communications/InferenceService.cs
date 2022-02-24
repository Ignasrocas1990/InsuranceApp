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
        //Func<String,double>convertToDouble =  x => double.Parse(x, CultureInfo.InvariantCulture);
        public InferenceService()
        {
            client = new HttpClient();
        }
        

        public Task<HttpResponseMessage> Email(String email)
        {
            if (!App.NetConnection())return null;
            
            var content = new StringContent(JsonConvert
                .SerializeObject(new Dictionary<string,string>(){{"email",email}})
                ,Encoding.UTF8, "application/json");
            
            if (App.NetConnection())
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
            var content = new StringContent(JsonConvert
                    .SerializeObject(new Dictionary<string,string>(){{"code",code}})
                ,Encoding.UTF8, "application/json");
            
            if (App.Connected)
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

        public async Task<string> SendQuoteRequest(int hospitals, int age, int cover, int hospitalExcess, int plan, int smoker)
        {
            var tempQuote = new Dictionary<string, int>()
            {
                {"Hospitals",hospitals},
                {"Age",age},
                {"Cover",cover},
                {"Hospital_Excess",hospitalExcess},
                {"Plan",plan},
                {"Smoker",smoker}
            };
            var result = await Predict(tempQuote);
            return await result.Content.ReadAsStringAsync();
        }
        /**
         * Take raw data from sensor and pass it by http to predict
         * if customer walking
         */
        private Task<HttpResponseMessage> Predict(Dictionary<String,int>quote)
        {
            if (!App.NetConnection()) return null;

            var content = new StringContent(JsonConvert.SerializeObject(quote),Encoding.UTF8, "application/json");
            if (App.NetConnection())
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