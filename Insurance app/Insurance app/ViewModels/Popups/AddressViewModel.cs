/*   Copyright 2020,Ignas Rocas

    Licensed under the Apache License, Version 2.0 (the "License");
    you may not use this file except in compliance with the License.
    You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

    Unless required by applicable law or agreed to in writing, software
    distributed under the License is distributed on an "AS IS" BASIS,
    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
    See the License for the specific language governing permissions and
    limitations under the License.
    
              Name : Ignas Rocas
    Student Number : C00135830
           Purpose : 4th year project
 */

using System.Windows.Input;
using Insurance_app.Models;
using Insurance_app.Pages.Popups;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.Popups
{
    /// <summary>
    /// Class used to store and manipulate AddressPopup Page UI inputs
    /// in real time via BindingContext and its properties
    /// </summary>
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
            Setup(address);
            CancelCommand = new Command(Close);
        }
        /// <summary>
        /// Sets received data to the UI bindable properties
        /// </summary>
        /// <param name="address">Address instance</param>
        private void Setup(Address address)
        {
            if (address.HouseN != null) HouseNDisplay =  (int) address.HouseN;
            StreetDisplay = address.Street;
            CityDisplay = address.City;
            CountyDisplay = address.County;
            CountryDisplay = address.Country;
            PostCodeDisplay = address.PostCode;
            
        }
        /// <summary>
        /// Creates new Address instance using
        /// user inputted data and returns it to main page
        /// </summary>
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
//---------------------------- Bindable properties below -------------------------------
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