using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;


namespace Insurance_app.ViewModels.ClientViewModels
{
    public class ClientMainViewModel : ObservableObject
    {
        public ObservableRangeCollection<Customer> Customers { get; set; }
        private readonly UserManager userManager;
        public ICommand StepViewCommand { get; }
        public ICommand CustomerDetailsCommand { get; }
        public ICommand CustomerClaimsCommand { get; }
        
        public ICommand PolicyCommand { get; }
        
        public ClientMainViewModel()
        {
            userManager = new UserManager();
            Customers = new ObservableRangeCollection<Customer>();
            StepViewCommand = new AsyncCommand<string>(ViewSteps);
            CustomerDetailsCommand = new AsyncCommand<string>(ManageDetails);
            CustomerClaimsCommand = new AsyncCommand<string>(ManageClaim);
            PolicyCommand = new AsyncCommand<string>(ManagePolicy);
        }
        
        private bool First = true;
        public async Task Setup()
        {
            try
            {
                if (!First) return;
                SetUpWaitDisplay = true;
                var list = await userManager.GetAllCustomer(App.RealmApp.CurrentUser);
                Customers.AddRange(list);
                SetUpWaitDisplay = false;
                First = false;
            }
            catch (System.Exception e)
            {

                System.Console.WriteLine(e);
            }
            
        }
        private async Task ManageDetails(string customerId)
        {
            if (!customerId.Equals(""))
            {
                var route = $"//{nameof(ProfilePage)}?CustomerId={customerId}";
                await Shell.Current.GoToAsync(route);
            }
        }
        private async Task ManageClaim(string customerId)
        {
            if (!customerId.Equals(""))
            {
                var route = $"//{nameof(ClaimPage)}?CustomerId={customerId}";
                await Shell.Current.GoToAsync(route);
            }
               
        }
        private async Task ManagePolicy(string customerId)
        {
            if (customerId == "")
                return;
            var route = $"//{nameof(PolicyPage)}?CustomerId={customerId}";
            await Shell.Current.GoToAsync(route);
        }
        
        private async Task ViewSteps(string customerId)
        {
            if (customerId == "")
                return;
            await Shell.Current.DisplayAlert("Notice", customerId, "close");
        }
        
        private bool wait;
        public bool SetUpWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        
    }
}