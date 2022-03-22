/*
    Copyright 2020,Ignas Rocas

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

using System;
using System.Linq;
using System.Text;
using Insurance_app.Models;
using Insurance_app.ViewModels.Popups;
using Xamarin.CommunityToolkit.UI.Views;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Insurance_app.Pages.Popups
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddressPopup : Popup<Address>
    {
        public AddressPopup(Address address)
        {
            InitializeComponent();
            BindingContext = new AddressViewModel(this,address);
        }
        /// <summary>
        /// Address validation
        /// </summary>
        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                var vm = (AddressViewModel)BindingContext;
                
                if ( HouseNrValidator.IsValid && StreetValidator.IsValid && CityValidator.IsValid 
                     && CountryValidator.IsValid && CountyValidator.IsValid &&  PostValidator.IsValid)
                {
                    vm.Save();
                }
                else
                {
                    var errBuilder = new StringBuilder();

                    if (HouseNrValidator.IsNotValid)
                    {
                        if (HouseNrValidator.Errors != null)
                            foreach (var err in HouseNrValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }

                    if (StreetValidator.IsNotValid)
                    {
                        if (StreetValidator.Errors != null)
                            foreach (var err in StreetValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }

                    if (CountyValidator.IsNotValid)
                    {
                        if (CountyValidator.Errors != null)
                            foreach (var err in CountyValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                    if (CityValidator.IsNotValid)
                    {
                        if (CityValidator.Errors != null)
                            foreach (var err in CityValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                    if (CountryValidator.IsNotValid)
                    {
                        if (CountryValidator.Errors != null)
                            foreach (var err in CountryValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                    if (PostValidator.IsNotValid)
                    {
                        if (PostValidator.Errors != null)
                            foreach (var err in PostValidator.Errors.OfType<string>())
                            {
                                errBuilder.AppendLine(err);
                            }
                    }
                   
                    await Application.Current.MainPage.DisplayAlert("Error", errBuilder.ToString(), "close");
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
            
        }
    }
}