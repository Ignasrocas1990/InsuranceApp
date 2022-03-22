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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Realms;
using Realms.Sync;

namespace Insurance_app.Service
{
    /// <summary>
    ///  Main Realm/Mongo database helper class
    /// Which is responsible connection to database
    /// It is generated one instance per page.
    /// (Since realm does not allow managed object across multiple threads.)
    /// </summary>
    public class RealmDb
    {
        private static Realm _realm;
        
        private static RealmDb _db;
        private RealmDb() {}

        public static RealmDb GetInstancePerPage()
        {
            if (_realm is null)
            {
                _db = new RealmDb();
            }

            return _db;
        }
//------------------------------------- Customer methods ---------------------
        /// <summary>
        /// Adds newly created customer instance to database
        /// </summary>
        /// <param name="customer">Customer object instance</param>
        /// <param name="user">Realm User instance</param>
        /// <exception cref="Exception">Throws when realm connection problem</exception>
        public async Task AddCustomer(Customer customer,User user)
        {
            try
            {
                await GetRealm(user);
                if (_realm is null) throw new Exception("AddCustomer, >>>>>>>>>>>>>>>>>>>>>>>> real is null");
                
                _realm.Write(() =>
                {
                     _realm.Add(customer,true);
                });
                Console.WriteLine("customer added");
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem adding customer > \n {e}");
            }
        }

        /// <summary>
        /// Finds a customer in Mongo cloud database
        /// </summary>
        /// <param name="user">Realm app user instance</param>
        /// <param name="id">customer id string</param>
        /// <returns>Customer instance</returns>
        public async Task<Customer> FindCustomer(User user,string id)
        {
            Customer c = null;
            try
            {
                await GetRealm(user);
                if (_realm is null) throw new Exception("FindCustomer, real is null");
                _realm.Write(() =>
                {
                    c = _realm.All<Customer>().FirstOrDefault(u => u.Id == id && u.DelFlag == false);

                });
                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong > \n {e}");
                return null;

            }
        }
        
        /// <summary>
        /// Updates pre-existing customer object from mongo database
        /// </summary>
        /// <param name="name">customer first name string</param>
        /// <param name="lastName">customer last name string</param>
        /// <param name="phoneNr">customer phone number string</param>
        /// <param name="address"> Customer address instance</param>
        /// <param name="user">Realm app user instance</param>
        /// <param name="customerId"></param>
        public async Task UpdateCustomer(string name, string lastName, 
            string phoneNr, Address address, User user,string customerId)
        {
            try
            {
                await SubmitActivity(customerId, user,"UpdateCustomer");
                
                
                await GetRealm(user);
                if (_realm is null) throw new Exception("UpdateCustomer, >>>>>>>>>>>>>>> real is null");
                _realm.Write(() =>
                {
                    var customer = _realm.All<Customer>().FirstOrDefault(c => c.Id == customerId);
                    if (customer == null) return;
                    customer.Name = name;
                    customer.LastName = lastName;
                    customer.PhoneNr = phoneNr;
                    customer.Address = address;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error when updating customer \n{e}");
            }
            
        }
        /// <summary>
        /// Get current customer date of birth from cloud database
        /// </summary>
        /// <param name="customerId"/>
        /// <param name="user">Authorized realm user instance</param>
        /// <returns>Date of birth DateTimeOffset</returns>
        public async Task<DateTimeOffset> GetCustomersDob(string customerId,User user)
        {
            DateTimeOffset dob;
            try
            {
                await GetRealm(user);
                if (_realm is null) throw new Exception("GetCustomersDob, real is null");
                _realm.Write(() =>
                {
                    var dateTimeOffset = _realm.Find<Customer>(customerId).Dob;
                    if (dateTimeOffset != null)
                        dob = (DateTimeOffset) dateTimeOffset;
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return dob;
        }
        /// <summary>
        /// Updates customer switch for collecting the sensor data
        /// </summary>
        /// <param name="user">Authorized realm user instance</param>
        /// <param name="switchState">bool describing if sensors on the watch to collect data</param>
        public async Task UpdateCustomerSwitch(User user, bool switchState)
        {
            try
            {
                var  otherRealm =  await GetOtherRealm((await App.RealmApp.CurrentUser.Functions.CallAsync("getPartition")).AsString,user);
                if (otherRealm is null)throw new Exception("UpdateCustomerSwitch >>>>>>>>>>> realm is null");
                otherRealm.Write(() =>
                {
                    var c =otherRealm.Find<Customer>(user.Id);
                    c.DataSendSwitch= switchState;
                });
                Console.WriteLine($"Updated Switch to {switchState} ");
            }
            catch (Exception e)
            {
                Console.WriteLine("UpdateCustomerSwitch >> "+e);
            }
        }
        
// --------------------------- Mov Data  methods --------------------------------     
        /// <summary>
        /// Gets all MoveMovement data for the report creation of the specified customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Authorized realm user instance</param>
        /// <returns>List of MovData object</returns>
        public async Task<List<MovData>> GetAllMovData(string customerId,User user)
        {
            List<MovData> movData = null;
            try
            {
                await GetRealm(user);
                if (_realm is null)
                    throw new Exception(" GetAllMovData >>>>>>>>>>>>>>>>>>>>>>>>>; real is null");
                _realm.Write(() =>
                {
                    movData = _realm.All<MovData>().Where(data => data.Owner == customerId && data.DelFlag == false).ToList();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return movData;
        }
        
//------------------------------------------------   reward methods ----------------------

        /// <summary>
        /// Updates cloud stored Reward objects.
        /// This used in-case of customer saving and not using their
        /// Rewards (Build up processing)
        /// </summary>
        /// <param name="price"></param>
        /// <param name="user">Authorized realm user instance</param>
        /// <param name="customerId"/>
        public async Task UpdateRewardsWithOverdraft(float price, User user, string customerId)
        {
            try
            {
                await GetRealm(user);
                if (_realm is null)
                    throw new Exception(" UpdateRewardsWithOverdraft >>>>>>>>>>>; real is null");
                _realm.Write(() =>
                {
                   var customer = _realm.Find<Customer>(customerId);
                    if (customer is null)
                        throw new Exception(" UpdateRewardsWithOverdraft >>>>>>>>>>>>>>; customer is null");
                    var rewards = customer.Reward.Where(r => r.FinDate != null && r.DelFlag == false);
                    float? payedPrice=0;
                    foreach (var reward in rewards)
                    {
                        if (payedPrice >= price) break;
                        payedPrice += reward.Cost;
                        reward.DelFlag = true;
                    }

                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        
        /// <summary>
        /// Sets Customer rewards to delete flag when it is used at the
        /// payment page.
        /// </summary>
        /// <param name="user">Authorized realm user instance</param>
        /// <param name="customerId"></param>
        public async Task UseRewards(User user, string customerId)
        {
            try
            {
                await GetRealm(user);
                if (_realm is null)
                    throw new Exception(" UseRewards >>>>>>>>>>>>>>>>>>>>>>>>>; real is null");
                _realm.Write(() =>
                {
                  var customer= _realm.Find<Customer>(customerId);
                   if (customer is null)
                       throw new Exception(" UseRewards >>>>>>>>>>>>>>; customer is null");
                   var rewards = customer.Reward.Where(r => r.FinDate != null && r.DelFlag == false);
                   foreach (var reward in rewards)
                   {
                       reward.DelFlag = true;
                   }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        /// <summary>
        /// Finds current reward, and creates it in-case it was finished and not created(by the watch)
        /// </summary>
        /// <param name="user">Authorized realm user instance</param>
        /// <returns>Reward current instance</returns>
        public async Task<Reward> FindReward(User user)
        {
            Reward reward = null;
            try
            {
                await GetRealm(user);
                if (_realm is null)
                    throw new Exception(" FindReward >>>>>>>>>>>>>>>>>>>>>>>>>; real is null");
                _realm.Write(()=>
                {
                    var customer = _realm.Find<Customer>(user.Id);
                    reward = customer.Reward.FirstOrDefault(r => r.FinDate == null && r.DelFlag == false);
                    
                    //find reward count this month (25 = 25%)
                    var currentDate = DateTimeOffset.Now;
                    var rewardCount = customer.Reward.Count(r => r.FinDate != null
                                                                 && r.FinDate.Value.Month == currentDate.Month
                                                                 && r.FinDate.Value.Year == currentDate.Year);
                    
                    if (reward == null && rewardCount <25)
                    {
                        var cost = customer.Policy.Where(p => p.DelFlag == false)
                            .OrderByDescending(p => p.ExpiryDate).First().Price / 100;
                      reward = _realm.Add(new Reward() {Cost = cost});
                      customer.Reward.Add(reward);
                      
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return reward;
        }
        /// <summary>
        /// Finds completed rewards and movement data collection switch.
        /// </summary>
        /// <param name="user">Authorized realm user instance</param>
        /// <param name="id">Customer id</param>
        /// <returns>bool collection switch and list of completed Reward instance </returns>
        public async Task<Tuple<bool, List<Reward>>> GetTotalRewards(User user,string id)
        {
            var rewards = new List<Reward>();
            var rewardsAndSwitch = new Tuple<bool, List<Reward>>(false,rewards);
            //float totalEarnings = 0;
            try
            {
                await GetRealm(user);
                if (_realm is null) throw new Exception("getTotalRewards >>>>>>>>>>>> realm  is null");
                _realm.Write(() =>
                {
                    var customer = _realm.Find<Customer>(id);
                    if (customer is null) throw new Exception("GetTotalRewards >>>>>>>>>>>>>>>> Customer is null");
                    rewards = customer.Reward.Where(r => r.FinDate != null && r.DelFlag == false).ToList();
                    /*
                    var sum = (from r in 
                        customer.Reward where r.FinDate != null && r.DelFlag == false && 
                                       r.Cost != null select r.Cost).Aggregate<float?, 
                        float?>(0, (current, f) => current + f.Value);

                    if (sum != null) totalEarnings = (float) sum;
                    */
                    rewardsAndSwitch = new Tuple<bool, List<Reward>>(customer.DataSendSwitch,rewards);
                    
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return rewardsAndSwitch;

        }

// ---------------------------- Claim methods --------------------------
        /// <summary>
        /// Creates and add new Claim instance to Mongo Cloud database
        /// while accounting with the customer 
        /// </summary>
        /// <param name="hospitalCode">user input string</param>
        /// <param name="patientNr">user input string</param>
        /// <param name="type">Health, (currently only health insurance)</param>
        /// <param name="user">Authorized realm user instance</param>
        /// <param name="customerId"></param>
        /// <param name="extraInfo">User input string, as comment</param>
        public async Task AddClaim( string hospitalCode,string patientNr,string type,User user,string customerId,string extraInfo)
        {
            try
            {
                await SubmitActivity(customerId, user, $"AddClaim");
                await GetRealm(user);
                if (_realm is null) throw new Exception(" AddClaim >>>>>>>>>>> realm null");
                _realm.Write(() =>
                {
                    var customer = _realm.Find<Customer>(customerId);

                    customer?.Claim.Add(_realm.Add(new Claim()
                    {
                        HospitalPostCode = hospitalCode,
                        PatientNr = patientNr,
                        Type = type,
                        Owner = customerId,
                        ExtraInfo = extraInfo

                    },true));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        /// <summary>
        /// Used to update Claim which is stored on mongo cloud database
        /// as it is being resolved by the client.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Authorized realm user instance</param>
        /// <param name="reason">Client reason for deny or customer comment if accepted string</param>
        /// <param name="action">accepted/deny boolean</param>
        /// <returns>Customer object instance</returns>
        public async Task<Customer> ResolveClaim(string customerId,User user,string reason,bool action)
        {
            Customer customer = null;
            try
            {
                await SubmitActivity(customerId, user,"ResolveClaim");
                
                await GetRealm(user);
                if (_realm is null)
                    throw new Exception(" ResolveClaim >>>>>>>>>>>>>>>>>>; realm null");
                _realm.Write(() =>
                {
                    customer = _realm.Find<Customer>(customerId);
                    var claim = customer.Claim
                        .FirstOrDefault(c => c.CloseDate == null && c.DelFlag == false);
                    if (claim == null) throw new Exception(" ResolveClaim >>>>>>>>>>>>>>>>>>; claim null");
                    
                    claim.CloseDate = DateTimeOffset.Now.Date;
                    claim.Accepted = action;
                    claim.ExtraInfo = reason;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return customer;
        }
        /// <summary>
        /// Gets All the claims that is associated with the customer & stored on Mongo Database
        /// </summary>
        /// <param name="user">Authorized realm user instance</param>
        /// <param name="customerId"></param>
        /// <returns>List of Claims</returns>
        public async Task<List<Claim>> GetClaims(User user,string customerId)
        {
            var claims = new List<Claim>();
            try
            {
                await GetRealm(user);
                if (_realm is null) throw new Exception(" GetClaims >>>>>>>>> realm null");
                _realm.Write(() =>
                {
                    //claims = realm.All<Claim>().Where(c => c.Partition == user.Id && c.DelFlag == false).ToList();
                    var customer = _realm.Find<Customer>(customerId);
                    if (customer !=null)
                    {
                        claims = customer.Claim.ToList();
                    }
                    
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return claims;
        }
        /// <summary>
        /// Get list of open claim by all the customers
        /// </summary>
        /// <param name="user">Authorized realm user instance (Client)</param>
        /// <returns>List of claim object instances</returns>
        public async Task<List<Claim>> GetAllOpenClaims(User user)
        {
            var openClaims = new List<Claim>();
            try
            {
                await GetRealm(user);
                if (_realm is null) throw new Exception("GetAllOpenClaims >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                _realm.Write(() =>
                {
                    openClaims = _realm.All<Claim>().Where(c => c.CloseDate == null && c.DelFlag == false).ToList();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return openClaims;
        }
//---------------------------------------------------- policy methods --------------------------------------
        /// <summary>
        /// Retrieves current policy and if user
        /// will be able to update it.
        /// </summary>
        /// <param name="customerId"/>
        /// <param name="user">Authorized realm user instance</param>
        /// <returns>Tuple with current policy & bool, can be updated or not.
        /// if the policy can be updated=true,false=cant </returns>
        public async Task<Tuple<bool,Policy>> FindPolicy(string customerId,User user)
        {
           var tuplePolicy = new Tuple<bool, Policy>(true,new Policy());
           try
           {
               await GetRealm(user);
               if (_realm is null) throw new Exception("FindPolicy realm null");
               
               _realm.Write(() =>
               {
                   var c = _realm.Find<Customer>(customerId);
                   var latestUpdatedPolicy = c?.Policy?.Where(p => p.UpdateDate != null)
                       .OrderByDescending(d => d.UpdateDate).FirstOrDefault();
                   
                  var currentPolicy = c?.Policy
                           ?.Where(p=> p.DelFlag == false)
                           .OrderByDescending(z => z.ExpiryDate).First();
                  
                  //check if current policy is updated already
                  if (latestUpdatedPolicy is null) tuplePolicy= new Tuple<bool, Policy>(true, currentPolicy);
                  else if (latestUpdatedPolicy.ExpiryDate.Value.CompareTo((DateTimeOffset) currentPolicy.ExpiryDate) == 0)
                  {
                      tuplePolicy= new Tuple<bool, Policy>(false,latestUpdatedPolicy);
                  }
                  else if (latestUpdatedPolicy.UpdateDate?.AddMonths(2) < currentPolicy.ExpiryDate)
                  {
                      tuplePolicy= new Tuple<bool, Policy>(true,currentPolicy);
                  }
                  else
                  {
                      tuplePolicy= new Tuple<bool, Policy>(false,latestUpdatedPolicy);
                  }
                  
               });
           }
           catch (Exception e)
           {
               Console.WriteLine(e);
           }
           return tuplePolicy;
        }
        /// <summary>
        /// Updates policy price after user has payed for it.
        /// </summary>
        /// <param name="policy">Current policy instance that is payed for</param>
        /// <param name="user">Authorized realm user instance</param>
        /// <param name="price">Calculated payed price float</param>
        public async Task UpdatePolicyPrice(Policy policy, User user, float price)
        {
            try
            {
                await GetRealm(user);
                _realm.Write(() =>
                {
                    policy.PayedPrice = price;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// Get all previous policies of the associated customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Authorized realm user instance</param>
        /// <returns>List of Policy instances</returns>
        public async Task<List<Policy>> GetPreviousPolicies(string customerId, User user)
        {
            List<Policy> previousPolicies = null;
            try
            {
                await GetRealm(user);
                if (_realm is null) throw new Exception("GetPreviousPolicies >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                _realm.Write(() =>
                {
                    previousPolicies = _realm.Find<Customer>(customerId)
                        .Policy.Where(p => p.UnderReview == false && p.DelFlag == false)
                        .OrderByDescending(d => d.ExpiryDate).ToList();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return previousPolicies ??= new List<Policy>();
        }
        /// <summary>
        /// Gets all updated policies that is stored on Mongo cloud Database.
        /// </summary>
        /// <param name="user">Authorized realm user instance</param>
        /// <returns>Group of update policy instances</returns>
        public async Task<IEnumerable<Policy>> GetAllUpdatedPolicies(User user)
        {
            IEnumerable<Policy> policies = new List<Policy>();
            try
            {
                await GetRealm(user);
                if (_realm is null) throw new Exception("GetAllUpdatedPolicies >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                _realm.Write(() =>
                {
                    var now = DateTimeOffset.Now;
                    policies= _realm.All<Policy>().Where(
                        p => p.UnderReview == true && p.DelFlag == false && p.ExpiryDate > now);
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return policies;
        }
        
        /// <summary>
        /// Update current policy that customer what to change which
        /// is stored on Mongo cloud database.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Authorized realm user instance(Client)</param>
        /// <param name="allowUpdate"> true = allow update/false = dont allow update </param>
        public async Task<Customer> ResolvePolicyUpdate(string customerId, User user,bool allowUpdate)
        {
            Customer customer =null;
            try
            {
                await SubmitActivity(customerId, user, $"ResolvePolicyUpdate,Allow={allowUpdate}");
                
                await GetRealm(user);
                if (_realm is null) throw new Exception("AllowPolicyUpdate >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                _realm.Write(()=>
                {
                     customer = _realm.Find<Customer>(customerId);
                    var policy = customer?.Policy?
                        .Where(p => p.UpdateDate != null && p.UnderReview == true && p.DelFlag == false)
                        .FirstOrDefault();
                    if (policy is null) throw new Exception("AllowPolicyUpdate  >>>>>>>>>>>>>>>>>>>>>>>>>> policy null");
                    
                    if (!allowUpdate) policy.DelFlag = true;
                    policy.UnderReview = false;
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return customer;

        }
        public async Task UpdatePolicy(string customerId,User user,Policy newPolicy)
        {
            try
            {
                await SubmitActivity(customerId, user, "UpdatePolicy");
                
                
                await GetRealm(user);
                if (_realm is null) throw new Exception("UpdatePolicy >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                _realm.Write(() =>
                {
                    _realm.Find<Customer>(customerId)?.Policy?.Add(_realm.Add(newPolicy));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        //--------------------------------- client methods -------------------------------
        public async Task<bool> CreateClient(User user, string email, string fname, string lname, string code)
        {
            try
            {
                var _realm = await GetOtherRealm(user.Id,user);
                if (_realm is null) throw new Exception("CreateClient >>>>>>>>>>> realm was null");
                _realm.Write(() =>
                {
                    _realm.Add(new Client()
                    {
                        Email = email,
                        FirstName = fname,
                        LastName = lname,
                        CompanyCode = code
                    });
                });
                await SubmitActivity(user.Id, user, "Register");
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return false;
        }

        public async Task<bool> IsClient(User user)
        {
            bool isClient = false;
            try
            {
                var otherRealm=await GetOtherRealm(user.Id,user);

                if (otherRealm is null) throw new Exception("IsClient,Realm return null");
                otherRealm.Write(()=>
                {
                   var c = otherRealm.All<Client>().FirstOrDefault(u => u.Id == user.Id && u.DelFlag == false);
                   if (c != null)
                   {
                       isClient = true;
                   }
                });
                otherRealm.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return isClient;
        }
        private async Task SubmitActivity(string customerId,User user,string type)
        {
            try
            {
                if (customerId == user.Id) return;

                var otherRealm = await GetOtherRealm(user.Id, user);
                if (otherRealm is null)
                    throw new Exception(" SubmitActivity >>>>>>>>>>>>>>>>>>; realm null");
                otherRealm.Write(() =>
                {
                   var c = otherRealm.Find<Client>(user.Id);
                    if (c is null) throw new Exception(" SubmitActivity >>>>>>>>>>>>>>>>>>; no client found");
                    c.Activities.Add(otherRealm.Add(new ClientActivity()
                    {
                        ActivityOwnerId = customerId,
                        Type = type
                    }));
                });
                otherRealm.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        public async Task<List<Customer>>GetAllCustomer(User user)
        {
            var customers = new List<Customer>();
            try
            {
                _realm = null;
                await GetRealm(user);

                _realm?.Write(() => { customers = _realm.All<Customer>().Where(c => c.DelFlag == false).ToList(); });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return customers;

        }
// -------------------------------- support methods ---------------------------------        
       
        private async Task GetRealm(User user)
        {
            try
            {
                if (_realm is null)
                {
                    var config = new SyncConfiguration(await GetPartition(),user);
                    _realm = await Realm.GetInstanceAsync(config);
                }
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetRealm,realm error > \n {e.Message}");
                Console.WriteLine($"GetRealm, inner exception > {e.InnerException}");
            }
        }

        private async Task<string> GetPartition() => (await App.RealmApp.CurrentUser.Functions.CallAsync("getPartition")).AsString;

        private async Task<Realm> GetOtherRealm(string partitionId,User user)
        {
            try
            {
                var config = new SyncConfiguration(partitionId,user);
                return await Realm.GetInstanceAsync(config);

            }
            catch (Exception e)
            {
                Console.WriteLine($"GetRealm,realm error > \n {e.Message}");
                Console.WriteLine($"GetRealm, inner exception > {e.InnerException}");
                return null;

            }
        }
        
        public void Dispose()
        {
            try
            {
                if (_realm is null) return;
                if (!_realm.IsClosed)
                {
                    _realm.Dispose();
                }
                _realm = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task CleanDatabase(User user)//TODO remove this when submitting
        {
            try
            {
                await GetRealm(user);
                if (_realm is null)
                    throw new Exception(" CleanDatabase >>>>>>>>>>>>>>>>>>; realm null");

                _realm.Write(() =>
                {
                    _realm.RemoveAll();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



    }
}