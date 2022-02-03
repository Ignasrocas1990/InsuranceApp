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
        
       
        //private ObservableRangeCollection<MovData> _MovData;
        private Reward reward;
        private Customer customer;
        private List<MovData> newMovDataList;
        private MovManager movManager;
        public MovViewModel()
        {
            //userManager = new UserManager(App.RealmApp.CurrentUser);
            movManager = new MovManager();
        }
        public async Task Setup()
        {
            try
            {
                //customer = await userManager.GetCustomer();
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
                //_MovData = new ObservableRangeCollection<MovData>(reward?.MovData.ToList() ?? throw new InvalidOperationException());
                newMovDataList = new List<MovData>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }



        //property methods


    }
}