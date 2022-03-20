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
    public class OpenPolicyRViewModel:ObservableObject
    {
        public ObservableRangeCollection<Policy> Policies { get; set; }
        public ICommand PolicySelectedCommand { get; }
        private readonly PolicyManager policyManager;
        
        
        public OpenPolicyRViewModel()
        {
            Policies = new ObservableRangeCollection<Policy>();
            PolicySelectedCommand = new AsyncCommand<Policy>(SelectedPolicy);
            policyManager = new PolicyManager();
        }
        public async Task Setup()
        {
            try
            {
                SetUpWaitDisplay = true;
                Policies.ReplaceRange(await policyManager.GetAllUpdatedPolicies(App.RealmApp.CurrentUser));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            ListVisibleDisplay = Policies.Count>0;
            SetUpWaitDisplay = false;
        }
        private async Task SelectedPolicy(Policy policy)
        {
            if (policy is null) return;
            Console.WriteLine(policy.Owner);
            var route = $"//{nameof(PolicyPage)}?CustomerId={policy.Owner}";
            await Shell.Current.GoToAsync(route);
        }
        
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
        public void Dispose()
        {
            policyManager.Dispose();
        }
    }
}