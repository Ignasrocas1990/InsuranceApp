using System;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;
using UserManager = Insurance_app.Logic.UserManager;

namespace Insurance_app.ViewModels
{
    public class CustomerViewModel : ObservableObject
    {
        private UserManager userManager;
        private RewardManager rewardManager;
        public CustomerViewModel()
        {
            rewardManager = new RewardManager();
            userManager = new UserManager();
        }
        private ObservableRangeCollection<MovData> _MovData;
        public async Task Setup()
        {
            try
            {
                var customer = await userManager.GetCustomer();
                if (customer is null)
                {
                    await Shell.Current.GoToAsync($"//{nameof(LogInPage)}",false);
                    return;
                }
                var reward = customer.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                if (reward is null)
                {
                    reward= await rewardManager.AddReward(customer);
                }
                _MovData =  new ObservableRangeCollection<MovData>(reward.MovData.ToList());

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        //property methods
        public ObservableRangeCollection<MovData> MovDataDisplay
        {
            get => _MovData;
            set => SetProperty(ref _MovData, value);
        }
    }
}