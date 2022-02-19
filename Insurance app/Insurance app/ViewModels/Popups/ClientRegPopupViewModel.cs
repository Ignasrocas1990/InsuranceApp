using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Pages.Popups;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    public class ClientRegPopupViewModel : ObservableObject
    {
        private ClientRegPopUp popUp;
        private string companyCode;
        private string email="";
        private string pass="";
        private string fname="";
        private string wait;
        private string lname="";
        public ICommand CancelCommand { get; }
        public ICommand RegisterCommand{ get; }
        

        public ClientRegPopupViewModel(ClientRegPopUp popUp,string companyCode)
        {
            this.popUp = popUp;
            this.companyCode = companyCode;
            RegisterCommand = new AsyncCommand(Register);
            CancelCommand = new Command(Dismiss);
        }

        private Task Register()
        {
            throw new NotImplementedException();
        }

        private Action Dismiss => () => popUp.Dismiss("");

        public string EmailDisplay
        {
            get => email;
            set => SetProperty(ref email, value);
        }

        public string PassDisplay
        {
            get => pass;
            set => SetProperty(ref pass, value);
        }

        public string FNameDisplay
        {
            get => fname;
            set => SetProperty(ref fname, value);
        }

        public string LNameDisplay
        {
            get => lname;
            set => SetProperty(ref lname, value);
        }

        public string CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
    }
}