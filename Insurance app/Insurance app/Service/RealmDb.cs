using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Realms;
using Realms.Sync;

namespace Insurance_app.Service
{
    public class RealmDb
    {
        private Realm _realm = null;
        public RealmDb() {}
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
            await GetRealm(user);
            try
            {
                if (_realm is null)
                {
                    Console.WriteLine("Couldn't get realm");
                    return;
                }
                _realm.Write(() =>
                {
                    _realm.Add(c);
                    
                });
                Console.WriteLine("customer added");
            }
            catch (Exception e)
            {
                Console.WriteLine($"problem adding customer : \n {e}");
            }
        }
        
        public async Task<Customer> FindCustomer(User user)
        {
            try
            {
                Customer c = null;
                await GetRealm(user);
                if (_realm != null)
                {
                    _realm.Write(() =>
                    {
                        c = _realm.All<Customer>().Where(u => u.Id == user.Id).FirstOrDefault();
                        //c = realm.Find<Customer>(user.Id);

                    });
                }
                return c;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Find Customer went wrong : \n {e}");
                return null;

            }
        }
        
// --------------------------- Mov Data  methods --------------------------------     
        public async Task AddMovData2(List<MovData> movList,User user)
        {
             await GetRealm(user);
            if (_realm is null)
            {
                Console.WriteLine("AddMovData2 realm is null");
                return;
            }
            try
            {
                _realm.Write(() =>
                {
                    var c = _realm.Find<Customer>(user.Id);
                    if (c != null)
                    {
                        var reward = c.Reward.Where(r => r.IsFinish = false).FirstOrDefault();
                        if (reward !=null)
                        {
                            foreach (var mov in movList)
                            {
                                reward.MovData.Add(mov);
                            }
                        }
                    }
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
//------------------------------------------------   reward methods ----------------------
        public async Task<Reward> AddNewReward(User user)
        {
            await GetRealm(user);
            if (_realm is null) return null;
            try
            {
                Reward reward = null;
                _realm.Write(()=>
                {
                    
                    var c = _realm.Find<Customer>(user.Id);
                    if (c!=null)
                    {
                        var r = _realm.Add(new Reward()
                        {
                            Partition = user.Id,
                            Cost = (c.Policy.Price/100)
                        });
                   
                        c.Reward.Add(r);
                        reward = _realm.Find<Reward>(r.Id);
                    }
                });
                return reward;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
// -------------------------------Get Instance ----------------------------
        private async Task GetRealm(User user)
        {
            try
            {
                _realm = null;

                var config = new SyncConfiguration(user.Id,user);
                    _realm = await Realm.GetInstanceAsync(config);
            }
            catch (Exception e)
            {
                Console.WriteLine($"realm instance error return null {e.Message}");
                Console.WriteLine($" inner exception : {e.InnerException}");
                _realm = null;
            }
        }
        public void StopSync()
        {
            if (_realm!=null)
                _realm.SyncSession.Stop();
            if (_realm != null) _realm.Dispose();
        }
    }
}