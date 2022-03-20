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
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Realms.Sync;

namespace Insurance_app.Logic
{
    public class UserManager : IDisposable
    {
        private RealmDb realmDb;

        public UserManager()
        {
            realmDb = RealmDb.GetInstancePerPage();

        }
        public async Task<string> Register(string email, string password)
        {
            try
            {
                await App.RealmApp.EmailPasswordAuth.RegisterUserAsync(email, password);
                return "success";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return e.Message;
            }
        }
        public async Task<Customer> GetCustomer(User user,string id)
        {
            return await realmDb.FindCustomer(user,id);
           // return currentCustomer;
        }
        
        public Customer CreateCustomer(DateTimeOffset dob, string fName, string lName, string phoneNr, string email,Address address)
        {
            try
            {
                return new Customer()
                {
                
                    Dob = dob, Name = fName, LastName = lName, 
                    PhoneNr = phoneNr, Email=email,
                    Address = new Address()
                    {
                        HouseN = address.HouseN,
                        City = address.City,
                        Country = address.Country,
                        County = address.County,
                        PostCode = address.PostCode,
                        Street = address.Street
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"customer creation error :\n {e}");
                return null;
            }

        }
        public async Task AddCustomer(Customer customer,User user)
        {
            await realmDb.AddCustomer(customer,user);
        }


        public async Task UpdateCustomer(string name, string lastName, 
            string phoneNr,Address address, User user,string customerId)
        {
           await realmDb.UpdateCustomer(name, lastName,
                phoneNr,  address, user,customerId);
        }

        public async Task<bool> CreateClient(User user, string email, string fname, string lname, string code)
        {
            return await realmDb.CreateClient(user, email, fname, lname, code);
        }

        public async Task<string> FindTypeUser(User user)
        {
            try
            {
                var now = DateTimeOffset.Now;
                var customer = await realmDb.FindCustomer(user,user.Id);

                if (customer!=null)
                {
                    // expired
                    var currentPolicy= FindLatestPolicy(customer);
                    if (currentPolicy != null && (Convert.ToDouble(currentPolicy.PayedPrice) < 1))
                    {
                        return "UnpaidCustomer";
                    }
                    if (currentPolicy != null && currentPolicy.ExpiryDate < now)
                    {
                        return currentPolicy.Id.ToString();
                    }
                    return "Customer";
                }
                if (await realmDb.IsClient(user))
                {
                    return "Client";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "";
        }

        private Policy FindLatestPolicy(Customer customer)
        {
            try
            {
                return customer.Policy
                    ?.Where(p=> p.DelFlag == false).OrderByDescending(z => z.ExpiryDate).FirstOrDefault();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return null;
        }
        
        public async Task<List<Customer>>GetAllCustomer(User user)
        {
            
            List<Customer> list = new List<Customer>();
            var now = DateTimeOffset.Now;
           foreach (var customer in await realmDb.GetAllCustomer(user))
           {
              var policy= FindLatestPolicy(customer);
              if (policy != null && policy.ExpiryDate > now)
              {
                  list.Add(customer);
              }
           }
           return list;
        }

        public async Task<DateTimeOffset> GetCustomersDob(string customerId,User user)
        {
            return await realmDb.GetCustomersDob(customerId,user);
        }

        public async Task UpdateCustomerSwitch(User user, bool switchState)
        { 
         await  realmDb.UpdateCustomerSwitch(user, switchState);
        }
        
        public async Task ResetPassword(string email,string name)
        {
            try
            {
                var tempPass = StaticOpt.TempPassGenerator(6,true);
                await App.RealmApp.EmailPasswordAuth.CallResetPasswordFunctionAsync(email,tempPass);
                HttpService.ResetPasswordEmail(email,name, DateTime.Now, tempPass);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void Dispose()
        {
            realmDb.Dispose();
        }

        public async Task CleanDatabase(User user) // TODO REMOVE when submitting
        {
          await realmDb.CleanDatabase(user);
        }
    }
}