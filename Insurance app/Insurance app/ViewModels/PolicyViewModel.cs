﻿/*   Copyright 2020,Ignas Rocas

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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Insurance_app.Logic;
using Insurance_app.Models;
using Insurance_app.Pages.ClientPages;
using Insurance_app.Service;
using Insurance_app.SupportClasses;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Insurance_app.ViewModels
{
    [QueryProperty(nameof(CustomerId), "CustomerId")]
    public class PolicyViewModel : ObservableObject,IDisposable
    {
        private const string EmailSubject = "Policy update";
        private const string EmailPolicyMsg = "Policy change request has been";
        private bool wait;
        private int hospitals;
        private int cover;
        private int fee;
        private int plan;
        private bool isSmoker;
        private float price;
        private bool canBeUpdated;
        private DateTimeOffset date;
        private DateTimeOffset? dob;
        private readonly Timer timer;
        private int rCount = 0;
        private bool tooLate;
        public ICommand UpdatePolicy { get; }
        public ICommand InfoCommand { get; }
        public ICommand ViewPrevPoliciesCommand { get; }
        public ICommand ResolveUpdateCommand { get; }

        public IList<string> HospitalList { get; }
        public IList<string> CoverList { get; }
        public IList<int> HospitalFeeList { get; }
        public IList<string> PlanList { get; } 
        private readonly PolicyManager policyManager;
        private readonly UserManager userManager;
        private string customerId = "";
        
        

        public PolicyViewModel()
        {
            UpdatePolicy = new AsyncCommand(Update);
            InfoCommand = new AsyncCommand<string>(StaticOpt.InfoPopup);
            ViewPrevPoliciesCommand = new AsyncCommand(ViewPrevPolicies);
            ResolveUpdateCommand = new AsyncCommand(ResolveUpdate);
            policyManager = new PolicyManager();
            userManager = new UserManager();
            timer = new Timer(1000);
            timer.Elapsed += CheckResponseTime;
            CoverList = Enum.GetNames(typeof(StaticOpt.CoverEnum)).ToList();
            HospitalFeeList = StaticOpt.ExcessFee();
            HospitalList = StaticOpt.HospitalsEnum();
            PlanList = Enum.GetNames(typeof(StaticOpt.PlanEnum)).ToList();
        }
        
        public async Task Setup()
        {
            PrevPoliciesIsVisible = false;
            var tempUpdate = false;
            try
            {
                SetUpWaitDisplay = true;
                UnderReviewDisplay = false;
                InfoIsVisible = false;

                if (customerId == "")
                {
                    customerId = App.RealmApp.CurrentUser.Id;
                }
                else if(customerId != App.RealmApp.CurrentUser.Id)
                {
                    await GetPreviousPolicies();
                }
                var policy = await FindPolicy();
                tempUpdate= SelectPreviousPolicy(policy);
            }
            catch (Exception e)
            {
                Console.WriteLine($"policy setup problem: \n {e}");
            }
            SetUpWaitDisplay = false;
            
            UnderReviewDisplay = tempUpdate;
            InfoIsVisible = !tempUpdate;
            PrevPoliciesIsVisible = policyManager.PreviousPolicies.Count>0 ;
            if (customerId != App.RealmApp.CurrentUser.Id && UnderReviewDisplay)
            {
                ClientActionNeeded = true;
            }
        }

        private bool SelectPreviousPolicy(Policy policy)
        {

            try
            {
                if (policy.Price != null) price = (float) policy.Price;
                PriceDisplay = (Math.Round(price * 100f) / 100f).ToString(CultureInfo.InvariantCulture);
                if (policy.Hospitals != null) SelectedHospital = HospitalList.IndexOf(policy.Hospitals);
                if (policy.Cover != null) SelectedCover =  CoverList.IndexOf(policy.Cover);
                if (policy.HospitalFee != null) SelectedItemHospitalFee = (int) policy.HospitalFee;
                if (policy.Plan != null) SelectedPlan = PlanList.IndexOf(policy.Plan) ;
                IsSmokerDisplay = Convert.ToBoolean(policy.Smoker);
                if (policy.ExpiryDate != null)
                {
                    ExpiryDateDisplay = policy.ExpiryDate.Value.Date.ToString("d");
                    date = (DateTimeOffset) policy.ExpiryDate;
                }
                return policy.UnderReview != null && (bool) policy.UnderReview;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return true;
        }

        private async Task Update()
        {
            if (!canBeUpdated)
            {
                await Msg.AlertError("Policy can only be updated every 3 months");
                return;
            }
            try
            {
                CircularWaitDisplay = true;
                if (dob is null)
                    await GetCurrentCustomer();

                var age = DateTime.Now.Year - dob.Value.Year;
                
                timer.Start();
                var newPrice = await HttpService.SendQuoteRequest(hospitals, age, cover, fee, plan, smoker);
                CircularWaitDisplay = false;
                timer.Stop();
                if (tooLate)
                {
                    tooLate = false;
                    return;
                }

                bool action = await Application.Current.MainPage.DisplayAlert(Msg.Notice,
                    $"Price for the quote is : {newPrice}", "Accept", "Deny");
                if (!action) return;

                var answer = await Shell.Current.DisplayAlert(
                    Msg.Notice, "You about to request to update the policy", "save", "cancel");
                if (!answer) return;
                UnderReviewDisplay = true;
                InfoIsVisible = !UnderReviewDisplay;
                CircularWaitDisplay = true;

                await SavePolicy(newPrice);
                PriceDisplay = $"{newPrice}";
                await Msg.Alert("Update requested successfully");
            }
            catch (Exception e)
            {
                Console.WriteLine($" Update policy error : {e}");
            }
            timer.Stop();
        }

        private async Task SavePolicy(string newPrice)
        {
            try
            {
                var newPolicy = policyManager.CreatePolicy(StaticOpt.StringToFloat(newPrice), price,
                     CoverList[cover], fee, HospitalList[hospitals],PlanList[plan], smoker,
                    true, date, DateTimeOffset.Now, customerId);
                await policyManager.AddPolicy(customerId,App.RealmApp.CurrentUser, newPolicy);
            }
            catch (Exception e)
            {
                await Msg.AlertError("Update requested failure\nPlease try again later");
                Console.WriteLine(e);
            }

            CircularWaitDisplay = false;
        }

        private async Task<Policy> FindPolicy()
        {
            Policy policy = null;
            try
            {
                 (canBeUpdated, policy) = await policyManager.FindPolicy(customerId,App.RealmApp.CurrentUser);
                 policyManager.RemoveIfContains(policy);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                canBeUpdated = false;
            }
            return policy ?? new Policy();
        }
        private async Task ViewPrevPolicies()
        {
            try
            {
                await Application.Current.MainPage.Navigation
                    .ShowPopupAsync(new PreviousPolicyPopup(policyManager.PreviousPolicies));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async Task ResolveUpdate()
        {
            try
            {
                if (!App.NetConnection())
                {
                    await Msg.Alert(Msg.NetworkConMsg);
                    return;
                }
                var answer = await Shell.Current.DisplayAlert(Msg.Notice, 
                    "Allow the Policy update ?", "Allow", "Deny");

                var answerString  = answer ? "Allow" : "Deny";

                var result = await Shell.Current.DisplayAlert(Msg.Notice, 
                    $"Are you sure you want to {answerString} the update?", "Yes", "No");
                if (!result) return;
                
                    CircularWaitDisplay = true;
                    var customer = await policyManager.AllowUpdate(customerId,App.RealmApp.CurrentUser,answer);
                    if (customer !=null)
                    {
                        HttpService.CustomerNotifyEmail(customer.Email, customer.Name, DateTime.Now, $"{answerString}'ed");
                        await Shell.Current.DisplayAlert(Msg.Notice, Msg.EmailSent, "close");
                    }
                    
                    ClientActionNeeded = false;
                    CircularWaitDisplay = false;
                    await Setup();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task GetPreviousPolicies() => 
            await policyManager.GetPreviousPolicies(customerId,App.RealmApp.CurrentUser);
        private async Task GetCurrentCustomer() => 
            dob = await userManager.GetCustomersDob(customerId,App.RealmApp.CurrentUser);

        private async void CheckResponseTime(object o, ElapsedEventArgs e)
        {
            rCount += 1;
            if (rCount != StaticOpt.MaxResponseTime) return;
            tooLate = true;
            CircularWaitDisplay = false;
            timer.Stop();
            rCount = 0;
            await Shell.Current.DisplayAlert(Msg.Error, "Something went wrong, try again in a min", "close");
        }

//-----------------------------data binding methods ------------------------------------------------


        public int SelectedHospital
        {
            get => hospitals;
            set => SetProperty(ref hospitals, value);
        }

        public int SelectedCover
        {
            get => cover;
            set => SetProperty(ref cover, value);
        }

        public int SelectedItemHospitalFee
        {
            get => fee;
            set => SetProperty(ref fee, value);
        }

        public int SelectedPlan
        {
            get => plan;
            set => SetProperty(ref plan, value);
        }

        public bool IsSmokerDisplay
        {
            get => isSmoker;
            set => SetProperty(ref isSmoker, (UpdateSmokerValue(value)));
        }

        private int smoker;

        private bool UpdateSmokerValue(bool value)
        {
            smoker = value ? 1 : 0;
            return value;
        }

        private bool updating;

        public bool UnderReviewDisplay
        {
            get => updating;
            set => SetProperty(ref updating, value);
        }

        private string expiryDate;

        public string ExpiryDateDisplay
        {
            get => expiryDate;
            set => SetProperty(ref expiryDate, value);
        }

        private string priceString;


        public string PriceDisplay
        {
            get => priceString;
            set => SetProperty(ref priceString, value);
        }
        public string CustomerId
        {
            get => customerId;
            set =>  customerId = Uri.UnescapeDataString(value ?? string.Empty);

        }
        private bool infoIsVisible;
        public bool InfoIsVisible
        {
            get => infoIsVisible;
            set => SetProperty(ref infoIsVisible, value);
        }

        private bool isClient;
        public bool ClientActionNeeded
        {
            get => isClient;
            set => SetProperty(ref isClient, value);
        }

        private bool prevPolicies = false;
        public bool PrevPoliciesIsVisible
        {
            get => prevPolicies;
            set => SetProperty(ref prevPolicies, value);
        }
        public bool CircularWaitDisplay
        {
            get => wait;
            set => SetProperty(ref wait, value);
        }
        private bool setUpWait;

        public bool SetUpWaitDisplay
        {
            get => setUpWait;
            set => SetProperty(ref setUpWait, value);
        }

        public void Dispose()
        {
            timer?.Dispose();
            policyManager?.Dispose();
            userManager?.Dispose();
        }
    }
}