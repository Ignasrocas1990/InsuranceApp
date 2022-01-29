using System;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using UserManager = Insurance_app.Logic.UserManager;

namespace Insurance_app.ViewModels
{
    public class MovViewModel : ObservableObject
    {
        private UserManager userManager;
        
        public MovViewModel()
        {
            userManager = new UserManager();
        }
        private ObservableRangeCollection<MovData> _MovData;
        private Reward reward;
        private Customer customer;
        private int count=0;

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
                var reward = customer.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                if (reward is null)
                {
                    //reward= await rewardManager.CreateReward(customer);
                    customer.CreateReward();
                    reward = customer.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                }
                _MovData = new ObservableRangeCollection<MovData>(reward.MovData.ToList());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        public void AddMov(float x, float y,float z, int type, DateTimeOffset time)
        {
            count++;
            //here we need to count (if mov data is <=10000)
            

            if (_MovData.Count >= StaticOptions.StepNeeded)
            {
                customer.CreateReward();
            }
            _MovData.Add(new MovData()
            {
                AccData = new Acc()
                {
                    X = x,
                    Y = y,
                    Z = z
                },
                DateTimeStamp = time,
                Type = "step"
            });
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