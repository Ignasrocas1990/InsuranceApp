using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.SupportClasses;
using Realms;
using Realms.Exceptions;
using Realms.Sync;
using Xamarin.Essentials;
using Policy =Insurance_app.Models.Policy;

namespace Insurance_app.Service
{
    public class RealmDb
    {
        private static Realm realm = null;
        //readonly Thread curThread = Thread.CurrentThread;
        
        private readonly string partition = "CustomerPartition";
        
        private static RealmDb _db = null;
        private RealmDb() {}

        public static RealmDb GetInstancePerPage()
        {
            if (realm is null)
            {
                _db = new RealmDb();
            }

            return _db;
        }
//------------------------------------- Customer methods ---------------------
        
        public async Task AddCustomer(Customer c,User user)
        {
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("AddCustomer, >>>>>>>>>>>>>>>>>>>>>>>> real is null");
                
                realm.Write(() =>
                {
                     realm.Add(c,true);
                });
                Console.WriteLine("customer added");
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem adding customer > \n {e}");
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
                Console.WriteLine($"Find Customer went wrong > \n {e}");
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
                if (realm is null) throw new Exception("UpdateCustomer, >>>>>>>>>>>>>>> real is null");
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
        public async Task UpdateCustomerSwitch(User user, bool switchState)
        {
            try
            {
                var  otherRealm =  await GetOtherRealm(partition,user);
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

/*
        public async Task AddMovData(float x,float y,float z,User user)
        {
            try
            {
                var  otherRealm =  await GetOtherRealm(partition,user);
                if (otherRealm is null)throw new Exception("AddMovData >>> realm is null");

                otherRealm.Write(() =>
                {
                    var customer = otherRealm.Find<Customer>(user.Id);
                    if (customer is null) throw new Exception("AddMvData >>> Customer is null");
                    var currentDate = DateTimeOffset.Now;
                    var rewardCount = customer.Reward.Count(r => r.FinDate != null 
                                                                 && r.FinDate.Value.Month == currentDate.Month
                                                                 && r.FinDate.Value.Year == currentDate.Year);
                    
                    var currentReward = customer.Reward.First(r => r.FinDate == null);
                    if (currentReward != null && currentReward.MovData.Count <= StaticOpt.StepNeeded)
                    {
                        currentReward.MovData.Add(otherRealm.Add(new MovData() {AccData = new Acc()
                            {X = x, Y = y, Z = z}, Type = "step"}));
                    }
                    else if(rewardCount < 25)
                    {
                        
                        var cost = customer.Policy.Where(p => p.DelFlag == false)
                            .OrderByDescending(p => p.ExpiryDate).First().Price / 100;
                        
                        var reward = otherRealm.Add(new Reward()
                        {
                            Cost = cost
                        });
                        var movData = otherRealm.Add(new MovData() {AccData = new Acc()
                            {X = x, Y = y, Z = z}, Type = "step"});
                        reward.MovData.Add(movData);
                        
                    }
                    else
                    {
                        otherRealm.Add(new MovData() {AccData = new Acc()
                            {X = x, Y = y, Z = z}, Type = "step"});
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task<Dictionary<string,int>> CountDailyMovData(User user)
        {
            Dictionary<string, int> chartEntries = new Dictionary<string, int>();
            try
            {
                var hourDif = 24;
                var now = DateTime.Now;
                var prev = DateTime.Now.AddHours(-hourDif);
                
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("GetWeeksMovData >>>>>>> real null");
                realm.Write(() =>
                {
                   
                    for (int i = 0; i < 7; i++)
                    {
                        int count = 0;
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
*/
        public async Task<List<MovData>> GetAllMovData(string customerId,User user)
        {
            List<MovData> movData = null;
            try
            {
                await GetRealm(partition,user);
                if (realm is null)
                    throw new Exception(" GetAllMovData >>>>>>>>>>>>>>>>>>>>>>>>>; real is null");
                realm.Write(() =>
                {
                    movData = realm.All<MovData>().Where(data => data.Owner == customerId && data.DelFlag == false).ToList();
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return movData;
        }
        
//------------------------------------------------   reward methods ----------------------

        public async Task UpdateRewardsWithOverdraft(float price, User user, string customerId)
        {
            try
            {
                await GetRealm(partition,user);
                if (realm is null)
                    throw new Exception(" UpdateRewardsWithOverdraft >>>>>>>>>>>; real is null");
                realm.Write(() =>
                {
                   var customer = realm.Find<Customer>(customerId);
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
        
        public async Task UseRewards(User user, string customerId)
        {
            try
            {
                await GetRealm(partition,user);
                if (realm is null)
                    throw new Exception(" UseRewards >>>>>>>>>>>>>>>>>>>>>>>>>; real is null");
                realm.Write(() =>
                {
                  var customer= realm.Find<Customer>(customerId);
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
        public async Task<Reward> FindReward(User user)
        {
            Reward reward = null;
            try
            {
                await GetRealm(partition,user);
                if (realm is null)
                    throw new Exception(" FindReward >>>>>>>>>>>>>>>>>>>>>>>>>; real is null");
                realm.Write(()=>
                {
                    var customer = realm.Find<Customer>(user.Id);
                    reward = customer.Reward.FirstOrDefault(r => r.FinDate == null);
                    
                    //find reward count this month (25 = 25%)
                    var currentDate = DateTimeOffset.Now;
                    var rewardCount = customer.Reward.Count(r => r.FinDate != null
                                                                 && r.FinDate.Value.Month == currentDate.Month
                                                                 && r.FinDate.Value.Year == currentDate.Year);
                    
                    if (reward == null && rewardCount <25)
                    {
                        var cost = customer.Policy.Where(p => p.DelFlag == false)
                            .OrderByDescending(p => p.ExpiryDate).First().Price / 100;
                      reward = realm.Add(new Reward() {Cost = cost});
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
        public async Task<Tuple<bool, List<Reward>>> GetTotalRewards(User user,string id)
        {
            var rewards = new List<Reward>();
            var rewardsAndSwitch = new Tuple<bool, List<Reward>>(false,rewards);
            //float totalEarnings = 0;
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("getTotalRewards >>>>>>>>>>>> realm  is null");
                realm.Write(() =>
                {
                    var customer = realm.Find<Customer>(id);
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

        public async Task AddClaim( string hospitalCode,string patientNr,string type,User user,string customerId,string extraInfo)
        {
            try
            {
                await SubmitActivity(customerId, user, $"AddClaim");
                await GetRealm(partition,user);
                if (realm is null) throw new Exception(" AddClaim >>>>>>>>>>> realm null");
                realm.Write(() =>
                {
                    var customer = realm.Find<Customer>(customerId);

                    customer?.Claim.Add(realm.Add(new Claim()
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
        
        public async Task<Customer> ResolveClaim(string customerId,User user,string reason,bool action)
        {
            Customer customer = null;
            try
            {
                await SubmitActivity(customerId, user,"ResolveClaim");
                
                await GetRealm(partition,user);
                if (realm is null)
                    throw new Exception(" ResolveClaim >>>>>>>>>>>>>>>>>>; realm null");
                realm.Write(() =>
                {
                    customer = realm.Find<Customer>(customerId);
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
        public async Task<List<Claim>> GetClaims(User user,string customerId)
        {
            List<Claim> claims = new List<Claim>();
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception(" GetClaims >>>>>>>>> realm null");
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
        public async Task<List<Claim>> GetAllOpenClaims(User user)
        {
            var openClaims = new List<Claim>();
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("GetAllOpenClaims >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                realm.Write(() =>
                {
                    openClaims = realm.All<Claim>().Where(c => c.CloseDate == null && c.DelFlag == false).ToList();
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
/// 
/// </summary>
/// <param name="customerId">customer id </param>
/// <param name="user">Customer or client</param>
/// <returns>Tuple with current policy & can be updated or not.
/// if the policy can be updated. true/false=cant </returns>
/// <exception cref="Exception"></exception>
public async Task<Tuple<bool,Policy>> FindPolicy(string customerId,User user)
        {
            //Policy currentPolicy = new Policy();
           Tuple<bool, Policy> tuplePolicy = null;
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
public async Task<List<Policy>> GetPreviousPolicies(string customerId, User user)
        {
            List<Policy> previousPolicies = null;
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("GetPreviousPolicies >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                realm.Write(()=>
                {
                    previousPolicies = realm.Find<Customer>(customerId)
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

        public async Task<IEnumerable<Policy>> GetAllUpdatedPolicies(User user)
        {
            IEnumerable<Policy> policies = new List<Policy>();
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("GetAllUpdatedPolicies >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                realm.Write(() =>
                {
                    var now = DateTimeOffset.Now;
                    policies= realm.All<Policy>().Where(
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
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="user">Client only</param>
        /// <param name="allowUpdate"> true = allow update/false = dont allow update </param>
        /// <exception cref="Exception"></exception>
        public async Task<Customer> ResolvePolicyUpdate(string customerId, User user,bool allowUpdate)
        {
            Customer customer =null;
            try
            {
                await SubmitActivity(customerId, user, $"ResolvePolicyUpdate,Allow={allowUpdate}");
                
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("AllowPolicyUpdate >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                realm.Write(()=>
                {
                     customer = realm.Find<Customer>(customerId);
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
                
                
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("UpdatePolicy >>>>>>>>>>>>>>>>>>>>>>>>>> realm null");
                realm.Write(() =>
                {
                    realm.Find<Customer>(customerId)?.Policy?.Add(realm.Add(newPolicy));
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task UpdatePolicyDate(DateTimeOffset newDate, Policy currentPolicy, float totalCost, User user)
        {
            try
            {
                await GetRealm(partition,user);
                if (realm is null) throw new Exception("UpdatePolicyDate >>>>>>>>>>> realm was null");
                realm.Write(() =>
                {
                   var customer = realm.Find<Customer>(user.Id);
                    var newPolicy = realm.Add(new Policy()
                    {
                        Smoker = currentPolicy.Smoker,
                        Cover = currentPolicy.Cover,
                        ExpiryDate = newDate,
                        HospitalFee = currentPolicy.HospitalFee,
                        Hospitals = currentPolicy.Hospitals,
                        Plan = currentPolicy.Plan,
                        Owner = user.Id,
                        PayedPrice = currentPolicy.Price-totalCost,
                        Price = currentPolicy.Price,
                        UnderReview = false
                    });
                    customer.Policy.Add(newPolicy);
                    currentPolicy.DelFlag = true;
                    if (totalCost>0)
                    {
                        var rewards=customer.Reward
                            .Where(r => r.FinDate != null && r.DelFlag != false).ToList();
                        
                        foreach (var r in rewards) 
                            r.DelFlag = true;
                    }
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
                if (realm is null) throw new Exception("CreateClient >>>>>>>>>>> realm was null");
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
                if (realm is null)
                {
                    var config = new SyncConfiguration(p,user);
                    realm = await Realm.GetInstanceAsync(config);
                }
               
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
                Console.WriteLine($"GetRealm,realm error > \n {e.Message}");
                Console.WriteLine($"GetRealm, inner exception > {e.InnerException}");
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
                Console.WriteLine($"GetRealm,realm error > \n {e.Message}");
                Console.WriteLine($"GetRealm, inner exception > {e.InnerException}");
                return null;

            }
        }
        
        public void Dispose()
        {
            try
            {
                if (realm is null) return;
                //realm.SyncSession.Stop();
               
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
                    throw new Exception(" CleanDatabase >>>>>>>>>>>>>>>>>>; realm null");

                realm.Write(() =>
                {
                    realm.RemoveAll();
                });
                /*
                await GetRealm(user.Id, user);
                if (realm is null)
                    throw new Exception(" CleanClient >>>>>>>>>>>>>>>>>>; realm null");
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


        public async Task UpdatePolicyPrice(User user, string customerId, double price)
        {
            try
            {
                await GetRealm(partition, user);
                realm.Write(() =>
                {
                   var now= DateTimeOffset.Now;
                    var customer = realm.Find<Customer>(customerId);
                    if (customer is null) throw new Exception("Customer is null :::::::::::::::UpdatePolicyPrice");
                    
                        var policy = customer.Policy.FirstOrDefault(
                        p => p.Owner == customerId && p.ExpiryDate > now
                             && p.PayedPrice == 0 && p.DelFlag == false);
                    if (policy != null) policy.PayedPrice = (float?) price;
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}