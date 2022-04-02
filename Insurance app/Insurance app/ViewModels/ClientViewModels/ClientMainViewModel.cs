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
    /// It used to set & view ListView data(Customers) of
    /// ClientMainPage UI in real time via BindingContext.
    /// </summary>
    public class ClientMainViewModel : ObservableObject,IDisposable
    {
        public ObservableRangeCollection<Customer> Customers { get; set; }
        private readonly UserManager userManager;
        public ICommand StepViewCommand { get; }
        public ICommand CustomerDetailsCommand { get; }
        public ICommand CustomerClaimsCommand { get; }
        public ICommand PolicyCommand { get; }

        public ClientMainViewModel()
        {
            userManager = new UserManager();
            Customers = new ObservableRangeCollection<Customer>();
            StepViewCommand = new AsyncCommand<string>(ViewSteps);
            CustomerDetailsCommand = new AsyncCommand<string>(ManageDetails);
            CustomerClaimsCommand = new AsyncCommand<string>(ManageClaim);
            PolicyCommand = new AsyncCommand<string>(ManagePolicy);
        }
        
        /// <summary>
        /// Gets and sets ListView source to customer list.
        /// </summary>
        public async Task Setup()
        {
            try
            {
                SetUpWaitDisplay = true;
                Customers.Clear();
                Customers.AddRange(await userManager.GetAllCustomer(App.RealmApp.CurrentUser));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetUpWaitDisplay = false;
        }
        /// <summary>
        /// Navigates user to Profile Page with parameter
        /// </summary>
        /// <param name="customerId">Selected customers parameter string</param>
        private async Task ManageDetails(string customerId)
        {
            SetUpWaitDisplay = true;
            if (!customerId.Equals(""))
            {
                var route = $"//{nameof(ProfilePage)}?CustomerId={customerId}";
                await Shell.Current.GoToAsync(route);
            }
        }
        /// <summary>
        /// Navigates user to Claim Page with parameter
        /// </summary>
        /// <param name="customerId">Selected customers parameter string</param>
        private async Task ManageClaim(string customerId)
        {
            SetUpWaitDisplay = true;
            if (!customerId.Equals(""))
            {
                var route = $"//{nameof(ClaimPage)}?CustomerId={customerId}";
                await Shell.Current.GoToAsync(route);
            }
               
        }
        /// <summary>
        /// Navigates user to Policy Page with parameter
        /// </summary>
        /// <param name="customerId">Selected customers parameter string</param>
        private async Task ManagePolicy(string customerId)
        {
            SetUpWaitDisplay = true;
            if (customerId == "")
                return;
            var route = $"//{nameof(PolicyPage)}?CustomerId={customerId}";
            await Shell.Current.GoToAsync(route);
        }
        /// <summary>
        /// Navigates user to Report Page with parameter
        /// </summary>
        /// <param name="customerId">Selected customers parameter string</param>
        private async Task ViewSteps(string customerId)
        {
            SetUpWaitDisplay = true;
            if (customerId == "")
                return;
            var route = $"//{nameof(Report)}?CustomerId={customerId}";
            await Shell.Current.GoToAsync(route);
        }
        // -------------- Bindable properties -----------------------
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
        
        public void Dispose()
        {
            Customers.Clear();
            userManager.Dispose();
        }
    }
}