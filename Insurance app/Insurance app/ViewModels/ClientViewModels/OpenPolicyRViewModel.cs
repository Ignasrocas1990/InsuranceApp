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
    public class OpenPolicyRViewModel:ObservableObject
    {
        public ObservableRangeCollection<Policy> Policies { get; set; }
        public ICommand PolicySelectedCommand { get; }
        public PolicyManager PolicyManager;
        
        
        public OpenPolicyRViewModel()
        {
            Policies = new ObservableRangeCollection<Policy>();
            PolicySelectedCommand = new AsyncCommand<Policy>(SelectedPolicy);
            PolicyManager = new PolicyManager();
        }
        public async Task Setup()
        {
            try
            {
                SetUpWaitDisplay = true;
                Policies.ReplaceRange(await PolicyManager.GetAllUpdatedPolicies(App.RealmApp.CurrentUser));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            ListVisibleDisplay = Policies.Count>0;
            SetUpWaitDisplay = false;
        }
        private async Task SelectedPolicy(Policy policy)
        {
            if (policy is null) return;
            Console.WriteLine(policy.Owner);
            var route = $"//{nameof(PolicyPage)}?CustomerId={policy.Owner}";
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

        private bool listVisible;
        public bool ListVisibleDisplay
        {
            get => listVisible;
            set => SetProperty(ref listVisible, value);
        }
        public void Dispose()
        {
            Policies.Clear();
        }
    }
}