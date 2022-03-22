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
    /// <summary>
    /// Service the processes the payemtns with
    /// User inputted card info
    /// </summary>
    public static class PaymentService
    {
        /// <summary>
        /// Uses token created and information provided, by the customer
        /// while using Stripe library calls its API to process the payment
        /// </summary>
        /// <param name="number">card number string</param>
        /// <param name="expYear">card expiry year int</param>
        /// <param name="expMonth">card expiry month int</param>
        /// <param name="cvc">card special code string</param>
        /// <param name="zip">customer zip code string</param>
        /// <param name="price">price that is needed to be processed double </param>
        /// <param name="name">customer name string</param>
        /// <param name="email">customer email string</param>
        /// <returns>try/false if translation has been successful</returns>
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

        /// <summary>
        /// Creates and process a Stripe charge
        /// </summary>
        /// <param name="price">calculated price double</param>
        /// <param name="email">customer email sting</param>
        /// <param name="stripeToken">Created Stripe token instance</param>
        /// <returns>true/false if it is created</returns>
        private static async Task<bool> Pay(double price, string email, IHasId stripeToken)
        {
            try
            {
                var roundedPrice = (long) Math.Round(price, 2) * 100;
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

        /// <summary>
        /// Creates a token which registers customer card details
        /// </summary>
        /// <param name="number">card number string</param>
        /// <param name="expYear">card expiry year int</param>
        /// <param name="expMonth">card expiry month int</param>
        /// <param name="cvc">card secret code string</param>
        /// <param name="zip">customer zip code string</param>
        /// <param name="name">customer name string</param>
        /// <returns>Stripe Toke instance</returns>
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