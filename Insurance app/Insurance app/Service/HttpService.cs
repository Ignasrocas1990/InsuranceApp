/*
    Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;
using Newtonsoft.Json;

namespace Insurance_app.Service
{
    /// <summary>
    /// Class used to send HTTP requests to
    /// custom API which,
    /// 1) Using ML predicts the policy price.
    /// 2) Stores codes that identify user as Client.
    /// 3) Email service
    /// </summary>
    public static class HttpService
    {
        /// <summary>
        /// Sends http request to custom API so it
        /// can send customer confirm email code
        /// </summary>
        /// <param name="email">customers email string</param>
        /// <param name="date">today's date DateTime</param>
        /// <param name="code">Random sequence on characters string</param>
        public static void EmailConfirm(string email, DateTime date, string code)
        {
            if (!App.NetConnection()) return;

            var client = new HttpClient();
            var content = new StringContent(JsonConvert
                    .SerializeObject(new Dictionary<string,string>()
                    {
                        {"email",email},
                        {"code",code},
                        {"date",$"{date:D}"}
                    })
                ,Encoding.UTF8, "application/json");
            
            if (App.NetConnection())
            {
                try
                {
                    client.PostAsync(StaticOpt.EmailConfirm, content);
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
        }
        /// <summary>
        /// Sends http request to custom API
        /// so it can send customer an email about allowing policy update
        /// </summary>
        /// <param name="email">customer email string</param>
        /// <param name="name">customer name string</param>
        /// <param name="date">today's date DateTime</param>
        /// <param name="action">Accept/Reject string</param>
        public static void CustomerNotifyEmail(string email, string name, DateTime date, string action)
        {
            if (!App.NetConnection()) return;

            var client = new HttpClient();
            var content = new StringContent(JsonConvert
                .SerializeObject(new Dictionary<string,string>()
                {
                    {"email",email},
                    {"name",name},
                    {"date",$"{date:D}"},
                    {"action",action}
                })
                ,Encoding.UTF8, "application/json");
            
            if (App.NetConnection())
            {
                try
                {
                    client.PostAsync(StaticOpt.EmailUrl, content);
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
        }
        /// <summary>
        /// Sends http request to custom API
        /// so it can send customer email about open Claim action
        /// </summary>
        /// <param name="email">customer email string</param>
        /// <param name="name">customer name string</param>
        /// <param name="date">today's date DateTime</param>
        /// <param name="action">Accept/Deny boolean</param>
        /// <param name="reason">Reason for client to reject or if accepted is empty</param>
        public static void ClaimNotifyEmail(string email, string name, DateTime date,bool action,string reason)
        {
            if (!App.NetConnection()) return;
            var client = new HttpClient();
            var actionString = action ? "Accepted" : "Denied";
            var content = new StringContent(JsonConvert
                    .SerializeObject(new Dictionary<string,string>()
                    {
                        {"email",email},
                        {"name",name},
                        {"date",$"{date:D}"},
                        {"action",actionString},
                        {"reason",reason}
                    })
                ,Encoding.UTF8, "application/json");
            
            if (App.NetConnection())
            {
                try
                {
                    client.PostAsync(StaticOpt.ClaimEmailUrl, content);
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
        }
        
        /// <summary>
        /// Sends http request to custom API
        /// so it can send customer email reset temporary random password
        /// </summary>
        /// <param name="email">customer email string</param>
        /// <param name="name">customer name string</param>
        /// <param name="date">today's date DateTime</param>
        /// <param name="tempPass">random password string</param>
        public static void ResetPasswordEmail(string email, string name, DateTime date,string tempPass)
        {
            if (!App.NetConnection()) return ;
            var client = new HttpClient();
            var content = new StringContent(JsonConvert
                    .SerializeObject(new Dictionary<string,string>()
                    {
                        {"email",email},
                        {"name",name},
                        {"date",$"{date:D}"},
                        {"pass",tempPass}
                    })
                ,Encoding.UTF8, "application/json");
            Console.WriteLine(email+" "+name+" "+$"{date:D}"+tempPass);
            if (App.NetConnection())
            {
                try
                {
                    client.PostAsync(StaticOpt.PassResetEmailUrl, content);
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
        }
        /// <summary>
        /// Sends http request to custom API
        /// so it can check pre-set client codes
        /// </summary>
        /// <param name="code">client input code string</param>
        /// <returns>Http Response Message instance</returns>
        public static Task<HttpResponseMessage> CheckCompanyCode(string code)
        {
            if (!App.NetConnection()) return null;
            var client = new HttpClient();
            var content = new StringContent(JsonConvert
                    .SerializeObject(new Dictionary<string,string>(){{"code",code}})
                ,Encoding.UTF8, "application/json");
            
            if (App.Connected)
            {
                try
                {
                    return client.PostAsync(StaticOpt.CompanyCodeUrl, content);
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
        /// <summary>
        /// Creates dictionary for the quote and waits till
        /// Predict returns the price (sent by API)
        /// </summary>
        /// <param name="hospitals">customer selected option int</param>
        /// <param name="age">calculated customer age int</param>
        /// <param name="cover">customer selected option int</param>
        /// <param name="hospitalExcess">customer selected option int</param>
        /// <param name="plan">customer selected option int</param>
        /// <param name="smoker">customer selected option int</param>
        /// <returns>price string</returns>
        public static async Task<string> SendQuoteRequest(int hospitals, int age, int cover, int hospitalExcess, int plan, int smoker)
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
        /// <summary>
        /// Posts request request to custom API
        /// to predict the price which policy has to be payed in
        /// regards to customer selected options.
        /// </summary>
        /// <param name="quote">Combined from user inputs Dictionary</param>
        /// <returns>null or HttpResponseMessage (depending success)</returns>
        private static Task<HttpResponseMessage> Predict(Dictionary<string,int>quote)
        {
            if (!App.NetConnection()) return null;
            var client = new HttpClient();
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