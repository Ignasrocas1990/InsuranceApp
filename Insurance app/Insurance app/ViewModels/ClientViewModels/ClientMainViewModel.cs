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
    public class ClientMainViewModel : ObservableObject,IDisposable
    {
        public ObservableRangeCollection<Customer> Customers { get; set; }
        public readonly UserManager UserManager;
        public ICommand StepViewCommand { get; }
        public ICommand CustomerDetailsCommand { get; }
        public ICommand CustomerClaimsCommand { get; }
        
        public ICommand PolicyCommand { get; }
        
        public ClientMainViewModel()
        {
            UserManager = new UserManager();
            Customers = new ObservableRangeCollection<Customer>();
            StepViewCommand = new AsyncCommand<string>(ViewSteps);
            CustomerDetailsCommand = new AsyncCommand<string>(ManageDetails);
            CustomerClaimsCommand = new AsyncCommand<string>(ManageClaim);
            PolicyCommand = new AsyncCommand<string>(ManagePolicy);
        }
        
        public async Task Setup()
        {
            try
            {
                SetUpWaitDisplay = true;
                Customers.ReplaceRange(await UserManager.GetAllCustomer(App.RealmApp.CurrentUser));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetUpWaitDisplay = false;
        }
        private async Task ManageDetails(string customerId)
        {
            SetUpWaitDisplay = true;
            if (!customerId.Equals(""))
            {
                var route = $"//{nameof(ProfilePage)}?CustomerId={customerId}";
                await Shell.Current.GoToAsync(route);
            }
        }
        private async Task ManageClaim(string customerId)
        {
            SetUpWaitDisplay = true;
            if (!customerId.Equals(""))
            {
                var route = $"//{nameof(ClaimPage)}?CustomerId={customerId}";
                await Shell.Current.GoToAsync(route);
            }
               
        }
        private async Task ManagePolicy(string customerId)
        {
            SetUpWaitDisplay = true;
            if (customerId == "")
                return;
            var route = $"//{nameof(PolicyPage)}?CustomerId={customerId}";
            await Shell.Current.GoToAsync(route);
        }
        
        private async Task ViewSteps(string customerId)
        {
            SetUpWaitDisplay = true;
            if (customerId == "")
                return;
            var route = $"//{nameof(Report)}?CustomerId={customerId}";
            await Shell.Current.GoToAsync(route);
        }
        
        private bool wait;
        public bool SetUpWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }

        private bool w;
        public bool CircularWaitDisplay
        {
            get => w;
            set => SetProperty(ref w, value);
        }

        public void Dispose()
        {
            Customers.Clear();
        }
    }
}