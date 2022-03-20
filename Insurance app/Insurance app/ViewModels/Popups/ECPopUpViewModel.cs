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

using System.Collections.Generic;
using Insurance_app.Models;
using Insurance_app.Pages.Popups;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Insurance_app.ViewModels.Popups
{
    public class EcPopUpViewModel : ObservableObject
    {
        public ObservableRangeCollection<Claim> Claims { get; set; }
        private readonly ExistingClaimsPopup popup;

        public EcPopUpViewModel(ExistingClaimsPopup popup, List<Claim> existingClaims)
        {
            this.popup = popup;
            Claims = new ObservableRangeCollection<Claim>(existingClaims);
        }
    }
}