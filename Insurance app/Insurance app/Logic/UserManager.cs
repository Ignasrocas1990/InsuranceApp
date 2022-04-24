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
/// <summary>
/// Class used to connect between Database and UI,
/// while processing Users
/// </summary>
    public class UserManager : IDisposable
    {
        private readonly RealmDb realmDb;

        public UserManager()
        {
            realmDb = RealmDb.GetInstancePerPage();

        }
        /// <summary>
        /// Register user with Realm/Mongo db
        /// </summary>
        /// <param name="email">email input</param>
        /// <param name="password">users password</param>
        /// <returns>error message</returns>
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
        /// <summary>
        /// Uses RealmDb helper to get Customer
        /// </summary>
        /// <param name="user">Realm user instance</param>
        /// <param name="userId">Realm userId string</param>
        /// <returns></returns>
        public async Task<Customer> GetCustomer(User user,string userId)
        {
            return await realmDb.FindCustomer(user,userId);
        }
        /// <summary>
        /// Creates customer instance
        /// </summary>
        /// <param name="dob">customers date of birth DateTimeOffset</param>
        /// <param name="fName">Customer first name string</param>
        /// <param name="lName">customer last name string</param>
        /// <param name="phoneNr">customer phone number string</param>
        /// <param name="email">customer email string</param>
        /// <param name="address">customer address instance</param>
        /// <returns>Customer Instance</returns>
        public Customer CreateCustomer(DateTimeOffset dob, string fName, string lName, string phoneNr, string email,Address address)
        {
            try
            {
                return new Customer()
                {
                
                    Dob = dob, Name = fName, LastName = lName, 
                    PhoneNr = phoneNr, Email=email,
                    DataSendSwitch = new DataSendSwitch(),
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
        /// <summary>
        /// Uses RealmDb helper to add customer to database
        /// </summary>
        /// <param name="customer">Newly created customer instance</param>
        /// <param name="user">Realm user instance</param>
        public async Task AddCustomer(Customer customer,User user)
        {
            await realmDb.AddCustomer(customer,user);
        }
        
        /// <summary>
        /// Passes data to RealmDb helper to update customer.
        /// </summary>
        /// <param name="name">customer name string</param>
        /// <param name="lastName">customer last name string</param>
        /// <param name="phoneNr">customer phone number string</param>
        /// <param name="address">customer Address instance</param>
        /// <param name="user">Realm user instance</param>
        /// <param name="customerId"></param>
        public async Task UpdateCustomer(string name, string lastName, 
            string phoneNr,Address address, User user,string customerId)
        {
           await realmDb.UpdateCustomer(name, lastName, phoneNr,  address, user,customerId);
        }

        /// <summary>
        /// Passes data to RealmDb helper to create client instance
        /// </summary>
        /// <param name="user">Realm User instance</param>
        /// <param name="email">Client email string</param>
        /// <param name="fname">Client first name string</param>
        /// <param name="lname">Client last name string</param>
        /// <param name="code">Client code string</param>
        /// <returns>true if everything went ok</returns>
        public async Task<bool> CreateClient(User user, string email, string fname, string lname, string code)
        {
            return await realmDb.CreateClient(user, email, fname, lname, code);
        }

        /// <summary>
        /// Finds type of user that is trying to log in.
        /// If customer is expired updates their policy
        /// </summary>
        /// <param name="user">Realm user instance</param>
        /// <returns>Client/Customer/UnpaidCustomer/OldCustomer/ExpiredCustomer string</returns>
        public async Task<string> FindTypeUser(User user)
        {
            try
            {
                var customer = await realmDb.FindCustomer(user,user.Id);

                if (customer!=null)
                {
                    var currentPolicy= FindLatestPolicy(customer);
                    if (currentPolicy is null)
                    {
                        return "";
                    }
                    Console.WriteLine(currentPolicy.PayedPrice.ToString());
                    var typeCustomer = TypeOfCustomer(currentPolicy);
                    /*
                    if (typeCustomer.Equals($"{UserType.ExpiredCustomer}"))
                    {
                        await realmDb.ChangePolicy(user,currentPolicy);
                    }*/
                    return typeCustomer;
                }
                if (await realmDb.IsClient(user))
                {
                    return  $"{UserType.Client}";
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return "";
        }
        
        /// <summary>
        /// Checks current customer Policy and returns
        /// if customer is expired/unpaid/old/normal
        /// </summary>
        /// <param name="currentPolicy">Latest found customer policy</param>
        /// <returns> Type of customer</returns>
        private string TypeOfCustomer(Policy currentPolicy)
        {
            var now = DateTimeOffset.Now;
            var expiredDate = currentPolicy.ExpiryDate.Value;
            if (Convert.ToDouble(currentPolicy.PayedPrice) < 1) return $"{UserType.UnpaidCustomer}";
            if (expiredDate < now.AddMonths(-2)) return $"{UserType.OldCustomer}";
            return expiredDate < now ? $"{UserType.ExpiredCustomer}" : $"{UserType.Customer}";
        }

        /// <summary>
        /// Finds latest policy
        /// </summary>
        /// <param name="customer">Current customer instance</param>
        /// <returns>Current policy instance</returns>
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
        
        /// <summary>
        ///  Passes data to RealmDb helper to get all the valid customers
        /// </summary>
        /// <param name="user">Realm user</param>
        /// <returns>List of valid Customer instances</returns>
        public async Task<List<Customer>>GetAllCustomer(User user)
        {
            try
            {
                var now = DateTimeOffset.Now;
                return (from customer in await realmDb.GetAllCustomer(user) 
                    let policy = FindLatestPolicy(customer) 
                    where policy != null && policy.ExpiryDate > now select customer).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return null;
        }
        
        /// <summary>
        /// Passes data to RealmDb helper to get customer date of birth
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Realm user instance</param>
        /// <returns>customers date of birth DateTimeOffset</returns>
        public async Task<DateTimeOffset> GetCustomersDob(string customerId,User user)
        {
            return await realmDb.GetCustomersDob(customerId,user);
        }
        
        /// <summary>
        /// Updates customer data monitoring switch
        /// </summary>
        /// <param name="user">Realm user instance</param>
        /// <param name="switchState">boolean state which the monitoring is going to be set to</param>
        public async Task UpdateCustomerSwitch(User user, bool switchState)
        { 
         await  realmDb.UpdateCustomerSwitch(user, switchState);
        }
        /// <summary>
        /// Create temporary password,
        /// Uses Realm app to reset Email password,
        /// And Using HttpService to send an email
        /// </summary>
        /// <param name="email">customer email string</param>
        /// <param name="name">customer name string</param>
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
        /// <summary>
        /// release Realm instance
        /// </summary>

        public void Dispose()
        {
            RealmDb.Dispose();
        }
    }
}