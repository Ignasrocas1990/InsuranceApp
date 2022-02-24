using System;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;


namespace Insurance_app.ViewModels.ClientViewModels
{
    public class CustomersViewModel : ObservableObject
    {
        public ObservableRangeCollection<Customer> Customers { get; set; }
        private readonly UserManager userManager;
        public AsyncCommand<string> StepViewCommand { get; }
        public AsyncCommand<string> CustomerDetailsCommand { get; }
        public AsyncCommand<string> CustomerClaimsCommand { get; }
        
        public CustomersViewModel()
        {
            userManager = new UserManager();
            Customers = new ObservableRangeCollection<Customer>();
            StepViewCommand = new AsyncCommand<string>(ViewSteps);
            CustomerDetailsCommand = new AsyncCommand<string>(ManageCustomer);
            CustomerClaimsCommand = new AsyncCommand<string>(ManageCustomerClaim);
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
        private async Task ManageCustomerClaim(string customerId)
        {
            var Id = customerId;
            if (customerId == "")
                return;
            var route = $"//{nameof(ClaimPage)}?Id={Id}";
            Console.WriteLine(Id);
            await Shell.Current.GoToAsync(route);
        }
        
        private async Task ManageCustomer(string customerId)
        {
            if (customerId == "")
                return;
            var route = $"//{nameof(ProfilePage)}?customerId={customerId}";
            Console.WriteLine(customerId);
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