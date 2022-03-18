using System;
using System.Threading.Tasks;
using Insurance_app.SupportClasses;

namespace Insurance_app.Service
{
    using Stripe;
    public static class PaymentService
    {
        public static async Task<bool> PaymentAsync(string number,int expYear,int expMonth,string cvc,string zip,double price,string name,string email)
        {
            try
            {
                var token = await CreateToken(number,expYear,expMonth,cvc,zip,name);
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
        
        private static async Task<bool> Pay(double price,string email,IHasId stripeToken)
        {
            try
            {
                
                var roundedPrice = (long)Math.Round(price,2)*100;
                // value is 49.1 euro

                StripeConfiguration.ApiKey =
                    (await App.RealmApp.CurrentUser.Functions.CallAsync("getKey")).AsString;
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

        private static async Task<Token> CreateToken(string number,int expYear,int expMonth,string cvc,string zip,string name)
        {
            try
            {
                StripeConfiguration.ApiKey = 
                    (await App.RealmApp.CurrentUser.Functions.CallAsync("getPKey")).AsString;
                //var service = new ChargeService();
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
                //return stripeToken.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
    }
}