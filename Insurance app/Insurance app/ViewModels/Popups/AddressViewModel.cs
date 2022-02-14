using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Pages.Popups;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    public class AddressViewModel : ObservableObject
    {
        private AddressPopup popup;
        private int houseN;
        private string street;
        private string city; 
        private string county;
        private string country; 
        private string postCode;
        private string errors="";

        public ICommand CancelCommand { get; }
        public ICommand SaveCommand { get; }

        public AddressViewModel(AddressPopup popup, Address address)
        {
            this.popup = popup;
            Init(address);
            SaveCommand = new AsyncCommand(Save);
            CancelCommand = new Command(Close);
        }

        private void Init(Address address)
        {
            if (address.HouseN != null) HouseNDisplay =  (int) address.HouseN;
            StreetDisplay = address.Street;
            CityDisplay = address.City;
            CountyDisplay = address.County;
            CountryDisplay = address.Country;
            PostCodeDisplay = address.PostCode;
        }

        private async Task Save()
        {
            
            if (postCode.Length < 4 || country.Length < 3 || street.Length < 5 || 0 > houseN )
            {
                errors += "Inputs are invalid length \n";
            }
            if (StaticOpt.HasNumbers(city) || StaticOpt.HasNumbers(country) || StaticOpt.HasNumbers(county))
            {
                errors = "City,Country,County cant have numbers";
            }

            if (errors.Length > 2)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Error", errors, "close");
                errors = "";
                return;

            }

            popup.Dismiss(new Address()
            {
                HouseN = houseN,
                Street = street,
                City = city,
                County = county,
                Country = country,
                PostCode = postCode
            });
            
        }
        private void Close()
        {
            popup.Dismiss(null);
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