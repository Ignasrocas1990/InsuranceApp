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

using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels.ClientViewModels
{
    /// <summary>
    /// It used to set & view ListView data(customer open claims) of
    /// ClientOpenClaims page UI in real time via BindingContext.
    /// </summary>
    public class ClientOClaimsViewModel:ObservableObject,IDisposable
    {
        private readonly ClaimManager claimManager;
        public ObservableRangeCollection<Claim> Claims { get; set; }
        public ICommand ClaimSelectedCommand { get; }

        public ClientOClaimsViewModel()
        {
            claimManager = new ClaimManager();
            Claims = new ObservableRangeCollection<Claim>();
            ClaimSelectedCommand = new AsyncCommand<Claim>(SelectedClaim);
        }
        /// <summary>
        /// Loads in data using manager classes via database
        /// And set it to Bindable properties(UI)
        /// </summary>
        public async Task Setup()
        {
            try
            {
                SetUpWaitDisplay = true;
                Claims.Clear();
                Claims.AddRange(await claimManager.GetAllOpenClaims(App.RealmApp.CurrentUser));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            ListVisibleDisplay = Claims.Count>0;
            PolicyInVisibleDisplay = !ListVisibleDisplay;
            SetUpWaitDisplay = false;
        }
        /// <summary>
        /// Navigates to Claims page after client clicks on list view claim
        /// with parameter
        /// </summary>
        /// <param name="claim">Selected Claim Instance</param>
        private async Task SelectedClaim(Claim claim)
        {
            if (claim is null) return;
            Console.WriteLine(claim.Owner);
            var route = $"//{nameof(ClaimPage)}?CustomerId={claim.Owner}";
            await Shell.Current.GoToAsync(route);
        }
        //------------------------   Bindable properties ----------------------
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

        private bool policyVisible;
        public bool PolicyInVisibleDisplay
        {
            get => policyVisible;
            set => SetProperty(ref policyVisible, value);
        }

        public void Dispose()
        {
            claimManager.Dispose();
        }
    }
}