using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Models;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;


namespace Insurance_app.ViewModels.ClientViewModels
{
    public class CustomersViewModel : ObservableObject
    {
        public ObservableRangeCollection<Customer> Customers { get; set; }
        private readonly UserManager userManager;
        public AsyncCommand<Customer> StepViewCommand { get; }
        
        public CustomersViewModel()
        {
            userManager = new UserManager();
            Customers = new ObservableRangeCollection<Customer>();
            StepViewCommand = new AsyncCommand<Customer>(ViewSteps);

        }

        private async Task ViewSteps(Customer customer)
        {
            if (customer == null)
                return;
            await Shell.Current.DisplayAlert("Notice", customer.Id, "close");
        }

        public async Task Setup()
        {
            SetUpWaitDisplay = true;
            var list = await userManager.GetAllCustomer(App.RealmApp.CurrentUser);
            Customers.AddRange(list);
            SetUpWaitDisplay = false;
        }


        private bool wait;

        public bool SetUpWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        
    }
}