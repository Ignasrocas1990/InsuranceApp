using System;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Pages;
using Java.Util;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class CustomerViewModel : ObservableObject
    {
        public CustomerViewModel()
        {
        }
        private Observable _Customer = new Observable();
        private ObservableRangeCollection<MovData> _MovData = new ObservableRangeCollection<MovData>();

        public async Task Setup()
        {
            try
            {
                var c = await App.RealmDb.FindCustomer(App.RealmApp.CurrentUser.Id);
                if (c is null)
                {
                    await Shell.Current.GoToAsync($"//{nameof(LogInPage)}");
                    return;
                }
                var reward = c.Reward.Where(r => r.FinDate == null).FirstOrDefault();
                if (reward != null)
                {
                    var mData = reward.MovData.ToList();
                    if (mData != null)
                    {
                        _MovData =  new ObservableRangeCollection<MovData>(mData);
                        return;
                    }
                }
                else
                {
                    var rw = await App.RealmDb.AddNewReward(c);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            
            
        }
        
        
        
        
        
        
        //property methods
        public ObservableRangeCollection<MovData> MovDataDisplay
        {
            get => _MovData;
            set => SetProperty(ref _MovData, value);
        }
        public Observable CustomerDisplay
        {
            get => _Customer;
            set => SetProperty(ref _Customer,value);
        }
        
    }
}