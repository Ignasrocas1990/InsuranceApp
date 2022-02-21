using System;
using System.Collections;
using System.Collections.Generic;
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
        private readonly AddressPopup popup;
        private int houseN;
        private string street;
        private string city; 
        private string county;
        private string country; 
        private string postCode;

        public ICommand CancelCommand { get; }
        public AddressViewModel(AddressPopup popup, Address address)
        {
            this.popup = popup;
            Init(address);
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

        public void Save()
        {
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