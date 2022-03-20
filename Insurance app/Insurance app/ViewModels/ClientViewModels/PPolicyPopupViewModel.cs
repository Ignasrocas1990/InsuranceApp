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
using System.Windows.Input;
using Insurance_app.Models;
using Insurance_app.Pages.ClientPages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.ClientViewModels
{
    public class PPolicyPopupViewModel:ObservableObject
    {
        private readonly PreviousPolicyPopup popup;
        public ICommand CloseCommand { get; }
        public ObservableRangeCollection<Policy> PreviousPolicies { get; set; }

        public PPolicyPopupViewModel(PreviousPolicyPopup popup, IEnumerable<Policy> previousPolicies)
        {
            this.popup = popup;
            PreviousPolicies = new ObservableRangeCollection<Policy>(previousPolicies);
            CloseCommand = new Command(ClosePopUp);

        }

        private void ClosePopUp()
        {
            popup.Dismiss("");
        }
    }
}