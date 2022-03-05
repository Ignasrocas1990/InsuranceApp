using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class AccountSettingsViewModel:ObservableObject
    {
        public ICommand UpdateOptionsCommand { get; }
        public ICommand ChangePassCommand { get; }
        public ICommand CancelAccountCommand { get; }

        private UserManager userManager;
        private string customerId="";
        private string email;
        private bool originalRewardSwitch;
        private bool originalDirrectDebid;

        


        public AccountSettingsViewModel()
        {
            ChangePassCommand = new AsyncCommand(ChangePassword);
            UpdateOptionsCommand = new AsyncCommand(UpdateOptions);
            CancelAccountCommand = new AsyncCommand(CancelAccount);
            userManager = new UserManager();
        }
        public async Task SetUp()
        {
            if (customerId.Equals(""))
            {
                customerId = App.RealmApp.CurrentUser.Id;
            }
            try
            {
              var customer = await userManager.GetCustomer(App.RealmApp.CurrentUser, customerId);
              if (customer != null)
              {
                  originalRewardSwitch = customer.DirectDebitSwitch;
                  UseRewardsDisplay = originalRewardSwitch;
              
                  originalDirrectDebid = customer.AutoRewardUse;
                  DirectDebitDisplay = originalRewardSwitch;
                  email = customer.Email;
              }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetUpWaitDisplay = false;
        }

        private Task UpdateOptions()
        {
            CircularWaitDisplay = true;
            try
            {
                if (originalDirrectDebid == DirectDebitDisplay && originalRewardSwitch == UseRewardsDisplay)
                {
                    CircularWaitDisplay = false;
                    Shell.Current.DisplayAlert("Notice", StaticOpt.SameDetailsMsg, "close");
                }
                else
                { 
                    userManager.UpdateAccountSettings(App.RealmApp.CurrentUser,customerId,DirectDebitDisplay,UseRewardsDisplay);
                    CircularWaitDisplay = false;
                    Shell.Current.DisplayAlert("Notice", StaticOpt.SuccessUpdateMsg, "close");
                }
               
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
            return Task.CompletedTask;
        }
        private async Task CancelAccount()
        {
            CircularWaitDisplay = true;
            try
            {
               var result = await Shell.Current.DisplayAlert("Notice",
                   "Are you sure you want to cancel account?\n" +
                   "Any un-used rewards will be lost!", "Yes","No");
               if (!result) return;
               userManager.UpdateAccountSettings(App.RealmApp.CurrentUser,customerId,false,false);
               await Shell.Current.DisplayAlert("Notice",
                   "Account has been canceled.\n" +
                   "You can still use app till end of the month.\n" +
                   "If you change your mind, just come back here and setup \"Direct-debit\" ", "close");
               CircularWaitDisplay = false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            } 
            CircularWaitDisplay = false;
        }

        private async Task ChangePassword()
        {
            try
            {
                CircularWaitDisplay = true;
                await App.RealmApp.EmailPasswordAuth
                    .CallResetPasswordFunctionAsync(email,password);
                CircularWaitDisplay = false;
                await Shell.Current.DisplayAlert("Message", StaticOpt.SuccessUpdateMsg, "close");

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
        }
        private string password;
        public string PassDisplay
        {
            get => password;
            set => SetProperty(ref password, value);
        }
        private string password2;
        public string PassDisplay2
        {
            get => password2;
            set => SetProperty(ref password2, value);
        }

        bool direct;
        public bool DirectDebitDisplay
        {
            get => direct;
            set => SetProperty(ref direct, value);
        }

        private bool useRewards;
        public bool UseRewardsDisplay
        {
            get => useRewards;
            set => SetProperty(ref useRewards, value);
        }

        private bool wait;

        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        private bool setUpWait;

        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }
    }
}