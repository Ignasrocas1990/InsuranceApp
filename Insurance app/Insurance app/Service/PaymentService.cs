/*   Copyright 2020,Ignas Rocas

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
using System.Threading.Tasks;
using Stripe;

namespace Insurance_app.Service
{
    public static class PaymentService
    {
        public static async Task<bool> PaymentAsync(string number, int expYear, int expMonth, string cvc, string zip,
            double price, string name, string email)
        {
            try
            {
                var token = await CreateToken(number, expYear, expMonth, cvc, zip, name);
                if (token != null)
                {
                    return await Pay(price, email, token);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        private static async Task<bool> Pay(double price, string email, IHasId stripeToken)
        {
            try
            {
                var roundedPrice = (long) Math.Round(price, 2) * 100;
                // value is 49.1 euro
                StripeConfiguration.ApiKey = (await App.RealmApp.CurrentUser.Functions.CallAsync("getKey")).AsString;
                var options = new ChargeCreateOptions
                {
                    Amount = roundedPrice,
                    Currency = "eur",
                    Description = "Dynamic Insurance App subscription",
                    StatementDescriptor = "Dynamic Insurance App",
                    ReceiptEmail = email,
                    Source = stripeToken.Id
                };
                var service = new ChargeService();
                await service.CreateAsync(options);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        private static async Task<Token> CreateToken(string number, int expYear, int expMonth, string cvc, string zip,
            string name)
        {
            try
            {
                StripeConfiguration.ApiKey = (await App.RealmApp.CurrentUser.Functions.CallAsync("getPKey")).AsString;
                var option = new TokenCreateOptions()
                {
                    Card = new TokenCardOptions()
                    {
                        Number = number,
                        ExpYear = expYear,
                        ExpMonth = expMonth,
                        Cvc = cvc,
                        Name = name,
                        AddressZip = zip
                    }
                };
                var tokenService = new TokenService();
                return await tokenService.CreateAsync(option);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
    }
}