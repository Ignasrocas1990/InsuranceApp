using System.Windows.Input;
using Insurance_app.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class AddressViewModel : ObservableObject
    {
        private AddressPopup popup;
        private string city =""; 
        private string country=""; 
        private string county=""; 
        private int houseN=0;
        private string postCode="";
        private string street="";

        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }

        public AddressViewModel(AddressPopup popup )
        {
            this.popup = popup;
            SaveCommand = new Command(save);
            CancelCommand = new Command(cancel);
        }

        private void save()
        {
           //check inputs and combine into dictionary
        }

        private void cancel()
        {
            popup.Dismiss("");
        }

        public string CityDisplay
        {
            get => city;
            set => SetProperty(ref city, value);
        }

        public string CountryDisplay
        {
            get => country;
            set => SetProperty(ref country, value);
        }

        public string CountyDisplay
        {
            get => county;
            set => SetProperty(ref county, value);
        }

        public int HouseNDisplay
        {
            get => houseN;
            set => SetProperty(ref houseN, value);
        }

        public string PostCodeDisplay
        {
            get => postCode;
            set => SetProperty(ref postCode, value);
        }

        public string StreetDisplay
        {
            get => street;
            set => SetProperty(ref street, value);
        }
    }
}