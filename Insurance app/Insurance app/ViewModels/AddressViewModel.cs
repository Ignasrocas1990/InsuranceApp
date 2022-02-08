using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    public class AddressViewModel : ObservableObject
    {
        private AddressPopup popup;
        private int houseN=0;
        private string street="";
        private string city =""; 
        private string county="";
        private string country=""; 
        private string postCode="";
        private string errors = "";

        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }

        public AddressViewModel(AddressPopup popup )
        {
            this.popup = popup;
            SaveCommand = new AsyncCommand(Save);
            CancelCommand = new Command(Cancel);
        }

        private async Task Save()
        {
            
            if (postCode.Length < 4 || country.Length < 3 || street.Length < 5 || 0 > houseN )
            {
                errors += "Inputs are invalid length \n";
            }
            if (StaticOptions.HasNumbers(city) || StaticOptions.HasNumbers(country) || StaticOptions.HasNumbers(county))
            {
                errors = "City,Country,County cant have numbers";
            }

            if (errors != "")
            {
                await Shell.Current.CurrentPage.DisplayAlert("Error", errors, "close");
                errors = "";
                return;

            }
            popup.Dismiss($"{houseN}~{street}~{city}~{county}~{country}~{postCode}");
        }
        private void Cancel()
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