using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using UserManager = Insurance_app.Logic.UserManager;

namespace Insurance_app.ViewModels
{
    public class MovViewModel : ObservableObject
    {
        private UserManager userManager;
        
       
        private ObservableRangeCollection<MovData> _MovData;
        private Reward reward;
        private Customer customer;
        private List<MovData> newMovDataList;
        private MovManager movManager;
        public MovViewModel()
        {
            userManager = new UserManager();
            movManager = new MovManager();
        }
        public async Task Setup()
        {
            try
            {
                customer = await userManager.GetCustomer();
                if (customer is null)
                {
                    await Shell.Current.GoToAsync($"//{nameof(LogInPage)}",false);
                    return;
                }

                reward = customer.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                if (reward is null)
                {
                    //reward= await rewardManager.CreateReward(customer);
                    await customer.CreateReward();
                    reward = customer.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                }
                _MovData = new ObservableRangeCollection<MovData>(reward?.MovData.ToList() ?? throw new InvalidOperationException());
                newMovDataList = new List<MovData>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public async Task AddMov(float x, float y,float z, int type, DateTimeOffset time)
        {
            //here we need to count (if mov data is <=10000)
            try
            {
                if (_MovData.Count >= StaticOptions.StepNeeded)
                {
                    await customer.CreateReward();
                }
                var currMovData = new MovData()
                {
                    AccData = new Acc()
                    {
                        X = x,
                        Y = y,
                        Z = z
                    },
                    DateTimeStamp = time,
                    Type = "step",
                    Partition = customer.Id
                };
                newMovDataList.Add(currMovData);
                _MovData.Add(currMovData);

                if (newMovDataList.Count > 4)
                {
                    //might need to initialize new MovData observable if still gives out-------------------------
                    //or re-create new Realm
                    var result  = await movManager.AddMovData(newMovDataList);
                 
                    newMovDataList.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"failed to save mov data {e}");
            }
            
            
            //else we create new reward
            
        }


        //property methods
        public ObservableRangeCollection<MovData> MovDataDisplay
        {
            get => _MovData;
            set => SetProperty(ref _MovData, value);
        }


    }
}