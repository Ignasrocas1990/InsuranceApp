using System;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Logic;
using Insurance_app.Models;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Insurance_app.ViewModels.ClientViewModels
{
    public class ClientOClaimsViewModel:ObservableObject,IDisposable
    {
        public ClaimManager ClaimManager;
        public ObservableRangeCollection<Claim> claims { get; set; }
        public ClientOClaimsViewModel()
        {
            ClaimManager = new ClaimManager();
            claims = new ObservableRangeCollection<Claim>();
        }
        public async Task Setup()
        {
            try
            {
                SetUpWaitDisplay = true;
                claims.AddRange(await ClaimManager.GetAllOpenClaims(App.RealmApp.CurrentUser));
                if (claims.Count>0)
                {
                    ListVisibleDisplay = true;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetUpWaitDisplay = false;
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
            claims = new ObservableRangeCollection<Claim>();
        }
    }
}