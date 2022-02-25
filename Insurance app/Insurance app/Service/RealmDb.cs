﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Insurance_app.Models;
using Realms;
using Realms.Exceptions;
using Realms.Sync;
using Xamarin.Essentials;
using Policy =Insurance_app.Models.Policy;

namespace Insurance_app.Service
{
    public class RealmDb
    {
        private Realm realm = null;
        //readonly Thread curThread = Thread.CurrentThread;
        private static RealmDb _db = null;
        private readonly string partition = "CustomerPartition";

        private RealmDb() {}

        public static RealmDb GetInstance()
        {
            return _db ??= new RealmDb();
        }
        
        
//------------------------- app Access Methods ---------------------------------------
        public async Task<string> Register(String email, String password)
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
//------------------------------------- Customer methods ---------------------
        
        public async Task AddCustomer(Customer c,User user)
        {
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("AddCustomer, real is null #########################");
                
                realm.Write(() =>
                {
                     realm.Add(c,true);
                });
                Console.WriteLine("customer added");
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem adding customer : \n {e}");
            }
        }

        public async Task<Customer> FindCustomer(User user,string id)
        {
            Customer c = null;
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("FindCustomer, real is null");
                realm.Write(() =>
                {
                    c = realm.All<Customer>().FirstOrDefault(u => u.Id == id && u.DelFlag == false);
                    
                });
                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong : \n {e}");
                return null;

            }
        }
        public async Task UpdateCustomer(string name, string lastName, 
            string phoneNr, Address address, User user,string customerId)
        {
            try
            {
                await SubmitActivity(customerId, user,"UpdateCustomer");
                
                
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("UpdateCustomer, real is null");
                realm.Write(() =>
                {
                    var customer = realm.All<Customer>().FirstOrDefault(c => c.Id == customerId);
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
        public async Task<DateTimeOffset> GetCustomersDob(string customerId,User user)
        {
            DateTimeOffset dob;
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("GetCustomersDob, real is null");
                realm.Write(() =>
                {
                    var dateTimeOffset = realm.Find<Customer>(customerId).Dob;
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
        
// --------------------------- Mov Data  methods --------------------------------     
        public async Task AddMovData(ConcurrentQueue<MovData> movList,User user)
        {
            try
            {
                await GetRealm(partition,user);
                if (realm is null)
                {
                    Console.WriteLine("AddMovData2 realm is null");
                    return;
                }
                realm.Write(() =>
                {
                    var customer = realm.Find<Customer>(user.Id);
                    if (customer is null) throw new Exception("AddMvData ::: Customer is null");
                    var currentDate = DateTimeOffset.Now;
                    int rewardCount = customer.Reward.Count(r => r.FinDate != null 
                                                       && r.FinDate.Value.Month == currentDate.Month
                                                       && r.FinDate.Value.Year == currentDate.Year);
                    if (rewardCount >= 25)
                    {
                        realm.Add(movList);
                    }
                    
                    var reward = customer.Reward.FirstOrDefault(r => r.DelFlag == false && r.FinDate == null);

                    if (reward == null) throw new Exception("AddMvData ::: reward is null");
                    foreach (var mov in movList)
                    {
                        reward.MovData.Add(mov);
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<Dictionary<string,int>> GetWeeksMovData(User user)
        {
            Dictionary<string, int> chartEntries = new Dictionary<string, int>();
            try
            {
                var hourDif = 24;
                var now = DateTime.Now;
                var prev = DateTime.Now.AddHours(-hourDif);
                
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("GetWeeksMovData ::::::: real null");
                realm.Write(() =>
                {
                    int count = 0;
                    for (int i = 0; i < 7; i++)
                    {
                        var prev1 = prev;
                        var now1 = now;
                        count = realm
                            .All<MovData>().Count(m => m.Owner == user.Id && m.DateTimeStamp <= now1 && 
                                                       m.DateTimeStamp > prev1 && m.DelFlag == false);
                        chartEntries.Add(now.DayOfWeek.ToString(),count);
                        now = prev;
                        prev = prev.AddHours(-hourDif);
                    }
                });
                return chartEntries;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
            
        }



//------------------------------------------------   reward methods ----------------------
        public async Task<Reward> AddNewReward(User user)
        {
            Reward reward = null;
            try
            {
                await GetRealm(partition,user);
                if (realm is null)
                    throw new Exception("real, AddNewReward, null");
                realm.Write(()=>
                {
                    var customer = realm.All<Customer>().FirstOrDefault(c => c.Id == user.Id && c.DelFlag == false);
                    if (customer == null) throw new Exception("AddNewReward ============== customer null");
                    var currentDate = DateTimeOffset.Now;
                    var currentMonthRewards = customer.Reward.Count(r => r.FinDate != null && r.DelFlag==null
                                                                 && r.FinDate.Value.Month == currentDate.Month
                                                                 && r.FinDate.Value.Year == currentDate.Year);
                    if (currentMonthRewards >= 25) return;
                    
                    var policies = customer.Policy
                        .Where(p=> p.DelFlag == false).ToList();
                    var policy = policies.OrderByDescending(z => z.ExpiryDate).First();
                    var cost = policy.Price / 100;
                    var newReward = realm.Add(new Reward()
                    {
                        Cost = cost
                    });
                   
                    customer.Reward.Add(newReward);
                    reward = realm.Find<Reward>(newReward.Id);
                });
                return reward;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public async Task<float> GetTotalRewards(User user,string id)
        {
            float totalEarnings = 0;
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("getTotalRewards realm null my exception");
                realm.Write(() =>
                {
                   var customer =  realm.Find<Customer>(id);
                   if (customer is null) throw new Exception("GetTotalRewards ::::::: customer is null");
                   var rewards = customer.Reward.Where(r => r.FinDate != null && r.DelFlag == false).ToList();
                   totalEarnings += rewards.Where(r => r.Cost != null).Sum(r =>
                    {
                        if (r.Cost != null) return (float) r.Cost;
                        return 0;
                    });
                });
                return totalEarnings;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return totalEarnings;
            }
        }

// ---------------------------- Claim methods --------------------------

        public async Task AddClaim( string hospitalCode,string patientNr,string type,bool openStatus,User user,string customerId)
        {
            try
            {
                await SubmitActivity(customerId, user, $"AddClaim");
                await GetRealm(partition,user);
                if (realm is null) throw new Exception(" AddClaim ::::::::::: realm null");
                realm.Write(() =>
                {
                    var customer = realm.Find<Customer>(customerId);

                    customer?.Claim.Add(realm.Add(new Claim()
                    {
                        HospitalPostCode = hospitalCode,
                        PatientNr = patientNr,
                        Type = type,
                        OpenStatus = openStatus,
                        Owner = user.Id

                    },true));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
        }
        
        public async Task ResolveClaim(string customerId,User user)
        {
            try
            {
                await SubmitActivity(customerId, user,"ResolveClaim");
                
                await GetRealm(partition,user);
                if (realm is null)
                    throw new Exception(" ResolveClaim ::::::::::::::; realm null");
                realm.Write(() =>
                {
                    var claim = realm.Find<Customer>(customerId).Claim
                        .FirstOrDefault(c => c.CloseDate == null && c.DelFlag == false);
                    if (claim != null)
                    {
                        claim.CloseDate = DateTimeOffset.Now.Date;
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<List<Claim>> GetClaims(User user,string customerId)
        {
            List<Claim> claims = new List<Claim>();
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception(" GetClaims ::::::::: realm null");
                realm.Write(() =>
                {
                    //claims = realm.All<Claim>().Where(c => c.Partition == user.Id && c.DelFlag == false).ToList();
                    var customer = realm.Find<Customer>(customerId);
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
//---------------------------------------------------- policy methods --------------------------------------
/// <summary>
/// 
/// </summary>
/// <param name="customerId">customer id </param>
/// <param name="user">Customer or client</param>
/// <returns>Dictionary with current policy and 0/1 value which tells user
/// if the policy can be updated. 1=can/0=cant (be updated)</returns>
/// <exception cref="Exception"></exception>
public async Task<Dictionary<int,Policy>> FindPolicy(string customerId,User user)
        {
         

           //Policy currentPolicy = new Policy();
           Dictionary<int, Policy> dictionaryP = new Dictionary<int, Policy>();
           try
           {
               await GetRealm(partition,user);
               if (realm is null) throw new Exception("FindPolicy realm null");
               
               realm.Write(() =>
               {
                   var c = realm.Find<Customer>(customerId);
                   var latestUpdatedPolicy = c?.Policy?.Where(p => p.UpdateDate != null)
                       .OrderByDescending(d => d.UpdateDate).FirstOrDefault();
                   
                  var currentPolicy = c?.Policy
                           ?.Where(p=> p.DelFlag == false)
                           .OrderByDescending(z => z.ExpiryDate).First();
                  
                  if (latestUpdatedPolicy is null) dictionaryP.Add(1, currentPolicy);
                  else if (latestUpdatedPolicy.ExpiryDate.Value.CompareTo((DateTimeOffset) currentPolicy.ExpiryDate) == 0)
                  {
                      dictionaryP.Add(0,latestUpdatedPolicy);
                  }
                  else if (latestUpdatedPolicy.UpdateDate?.AddMonths(2) < currentPolicy.ExpiryDate)
                  {
                      dictionaryP.Add(1,currentPolicy);
                  }
                  else
                  {
                      dictionaryP.Add(0,latestUpdatedPolicy);
                  }
                  
               });
           }
           catch (Exception e)
           {
               Console.WriteLine(e);
           }
           return dictionaryP;
        }
        public async Task<List<Policy>> GetPreviousPolicies(string customerId, User user)
        {
            List<Policy> previousPolicies = null;
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("GetPreviousPolicies :::::::::::::::::::::: realm null");
                realm.Write(()=>
                {
                    previousPolicies = realm.Find<Customer>(customerId)
                        .Policy?.Where(policy => policy.UnderReview == false).ToList();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return previousPolicies ??= new List<Policy>();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Client only</param>
        /// <param name="allowUpdate"> true = allow update/false = dont allow update </param>
        /// <exception cref="Exception"></exception>
        public async Task ResolvePolicyUpdate(string customerId, User user,bool allowUpdate)
        {
            try
            {
                await SubmitActivity(customerId, user, $"ResolvePolicyUpdate,Allow={allowUpdate}");
                
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("AllowPolicyUpdate :::::::::::::::::::::: realm null");
                realm.Write(()=>
                {
                    var policy = realm.Find<Customer>(customerId)?.Policy?
                        .Where(p => p.UpdateDate != null && p.UnderReview == true && p.DelFlag == false)
                        .FirstOrDefault();
                    if (policy is null) throw new Exception("AllowPolicyUpdate  :::::::::::::::::::::: policy null");
                    
                    if (!allowUpdate) policy.DelFlag = true;
                    policy.UnderReview = false;
                });
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task UpdatePolicy(string customerId,User user,Policy newPolicy)
        {
            try
            {
                await SubmitActivity(customerId, user, "UpdatePolicy");
                
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("UpdatePolicy :::::::::::::::::::::: realm null");
                realm.Write(() =>
                {
                    
                    realm.Find<Customer>(customerId)?.Policy?.Add(realm.Add(newPolicy));
                    // if (policies.Count==0) throw new Exception("UpdatePolicy::::: policies empty");
                    // var oldPolicy = policies.OrderByDescending(z => z.ExpiryDate).First();
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
                await GetRealm(user.Id,user);
                if (realm is null) throw new Exception("CreateClient ::::::::::: realm was null");
                realm.Write(() =>
                {
                    realm.Add(new Client()
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
        public async Task<string> FindTypeUser(User user)
        {
            string userType = "";
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("FindTpeUser,Realm return null");
                Customer c = null;
                realm.Write(() =>
                {
                    var now = DateTimeOffset.Now;
                    c = realm.Find<Customer>(user.Id);
                    if (c!=null)
                    {
                        var p = c.Policy.FirstOrDefault(policy => policy.ExpiryDate < now);//check if policy expired
                        if (p !=null)
                        {
                            userType = "NCustomer";
                        }
                        else
                        {
                            userType = "Customer";
                        }
                    }
                });
                if (userType == "")
                {
                    await GetRealm(user.Id,user);
                    var client = realm.Find<Client>(user.Id);
                    if (client != null)
                    {
                        userType = "Client";
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return userType;
        }
        private async Task SubmitActivity(string customerId,User user,string type)
        {
            try
            {
                if (customerId == user.Id) return;

                await GetRealm(user.Id, user);
                if (realm is null)
                    throw new Exception(" SubmitActivity ::::::::::::::; realm null");
                realm.Write(() =>
                {
                   var c = realm.Find<Client>(user.Id);
                    if (c is null) throw new Exception(" SubmitActivity ::::::::::::::; no client found");
                    c.Activities.Add(realm.Add(new ClientActivity()
                    {
                        ActivityOwnerId = customerId,
                        Type = type
                    }));
                });
                realm = null;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<List<Customer>>GetAllCustomer(User user)
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                realm = null;
                await GetRealm(partition, user);
                
                realm.Write(() =>
                {
                    customers = realm.All<Customer>().Where(c => c.DelFlag == false).ToList();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            return customers;

        }
// -------------------------------- support methods ---------------------------------        
       
        private async Task GetRealm(string p,User user)//ne
        {
            try
            {
                var config = new SyncConfiguration(p,user);
                realm = await Realm.GetInstanceAsync(config);
                /*
                if (!curThread.Equals(Thread.CurrentThread))
                {
                    realm = null;
                    Console.WriteLine("different thread");
                }
                */
                
            }
            catch (Exception e)
            {
                realm = null;
                Console.WriteLine($"GetRealm,realm error : \n {e.Message}");
                Console.WriteLine($"GetRealm, inner exception : {e.InnerException}");
            }
        }

        private async Task<Realm> GetOtherRealm(string partitionId,User user)
        {
            try
            {
                var config = new SyncConfiguration(partitionId,user);
                return await Realm.GetInstanceAsync(config);
                /*
                if (!curThread.Equals(Thread.CurrentThread))
                {
                    realm = null;
                    Console.WriteLine("different thread");
                }
                */
                
            }
            catch (Exception e)
            {
                Console.WriteLine($"GetRealm,realm error : \n {e.Message}");
                Console.WriteLine($"GetRealm, inner exception : {e.InnerException}");
                return null;

            }
        }
        
        public void Dispose()
        {
            try
            {
                if (realm is null) return;
                realm.SyncSession.Stop();
               
                if (!realm.IsClosed)
                {
                    realm.Dispose();
                }
                realm = null;
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
                await GetRealm(partition,user);
                if (realm is null)
                    throw new Exception(" CleanDatabase ::::::::::::::; realm null");

                realm.Write(() =>
                {
                    realm.RemoveAll();
                });
                /*
                await GetRealm(user.Id, user);
                if (realm is null)
                    throw new Exception(" CleanClient ::::::::::::::; realm null");
                realm.Write(() =>
                {
                    realm.RemoveAll();
                });
                */
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}