using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Insurance_app.Communications;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.SupportClasses;
using Newtonsoft.Json;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using System.Timers;
using Android.Provider;
using Insurance_app.Logic;
using Insurance_app.Pages.Popups;
using Insurance_app.Service;
using MongoDB.Bson;
using Xamarin.CommunityToolkit.Extensions;


namespace Insurance_app.ViewModels
{
    public class QuoteViewModel : ObservableObject,IDisposable
    {
        public ICommand GetQuotCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        private int responseCounter = 0;
        private bool wait;
        private bool tooLate;
        private int hospitals;
        private int cover;
        private int hospitalExcess;
        private int plan;
        private int smoker=0;
        private bool isSmokerChecker=false;
        private readonly Timer timer;
        private string elegalChars = "";
        private readonly string policyId = "";
        private string customerId = "";
        
        public ICommand InfoCommand { get; }
        public IList<string> HospitalList { get; }
        //age
        public IList<string> CoverList { get; }
        public IList<int> HospitalFeeList { get; }
        public IList<string> PlanList { get; }
        private readonly UserManager userManager;
        private readonly PolicyManager policyManager;
        private string email;
        private string name;

        public QuoteViewModel(string policyId)
       {
           HospitalList = StaticOpt.HospitalsEnum();
           CoverList = Enum.GetNames(typeof(StaticOpt.CoverEnum)).ToList();
           HospitalFeeList = StaticOpt.ExcessFee();
           PlanList = Enum.GetNames(typeof(StaticOpt.PlanEnum)).ToList();
           
           timer = new Timer(1000);
           timer.Elapsed += CheckResponseTime;
           GetQuotCommand = new AsyncCommand(GetQuote);
           InfoCommand = new AsyncCommand<string>(StaticOpt.InfoPopup);
           ResetPasswordCommand = new AsyncCommand(ResetPassword);
           this.policyId = policyId;
           userManager = new UserManager();
           policyManager = new PolicyManager();

       }

        public async Task SetUp()
        {
            IsExpiredCustomer = false;
            if (policyId.Equals("")) return;
            SetUpWaitDisplay = true;
            try
            {
                //ObjectId.Parse(policyId);
                customerId = App.RealmApp.CurrentUser.Id;
               var customer = await userManager.GetCustomer(App.RealmApp.CurrentUser, customerId);
               if (customer.Dob != null) SelectedDate = customer.Dob.Value.UtcDateTime;
               email = customer.Email;
               name = customer.Name;
               IsExpiredCustomer = true;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            SetUpWaitDisplay = false;
        }
        
        private async Task GetQuote()
       {
           if (!App.NetConnection())
           {
              await Msg.AlertError(Msg.NetworkConMsg);
               return;
           }
           if (elegalChars != "")
           {
              await Msg.AlertError(elegalChars);
               return;
           }
            
           var age = DateTime.Now.Year - selectedDate.Year;
          
           string price;
           
           try
           {
               CircularWaitDisplay=true;
                timer.Start();
                price =  await HttpService.SendQuoteRequest(hospitals, age, cover, hospitalExcess, plan, smoker);
                timer.Stop();
                if (tooLate)
                {
                    tooLate = false;
                    return;
                }
           }
           catch
           {
               timer.Stop();
               responseCounter = 0;
              await Msg.AlertError(Msg.ApiSendErrorMsg);
               CircularWaitDisplay = false;
               return;
           }
           CircularWaitDisplay = false;
           responseCounter = 0;
           bool action =
               await Application.Current.MainPage.DisplayAlert(Msg.Notice,
                   $"Price for the quote is : {price}", "Accept", "Deny");
           if (action && policyId == "")
           {
               await TransferToRegistration(age, price);
           }
           else if (action)
           {
               await CreatePolicyAndPay(price);
           }
           
       }

        private async Task CreatePolicyAndPay(string price)
        {
            try
            {
                var expiryDate = DateTimeOffset.Now.AddMonths(1);
                var priceFloat = Converter.GetPrice(price);

                var policy = policyManager.RegisterPolicy(priceFloat,0.0f, CoverList[cover],
                    hospitalExcess, HospitalList[hospitals], PlanList[plan],
                    smoker, false,expiryDate,customerId);
                CircularWaitDisplay = true;
                await policyManager.AddPolicy(customerId, App.RealmApp.CurrentUser, policy);
                await Application.Current.MainPage.Navigation.PushModalAsync(new PaymentPage(null));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            CircularWaitDisplay = false;
        }

        private async Task TransferToRegistration(int age,string price)
        {
            CircularWaitDisplay=true;
            try
            {   
                var tempQuote = new Dictionary<string, string>
                {
                    {"Hospitals",HospitalList[hospitals]},
                    {"Age",$"{age}"},
                    {"Cover",CoverList[cover]},
                    {"Hospital_Excess",$"{hospitalExcess}"},
                    {"Plan",PlanList[plan]},
                    {"Smoker",$"{smoker}"},
                    {selectedDate.ToString("d"), "-1"}
                };
                await Application.Current.MainPage.Navigation.PushModalAsync(new RegistrationPage(tempQuote,price));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            CircularWaitDisplay=false;
        }
        private async Task ResetPassword()
        {
            if (!App.NetConnection())
            {
               await Msg.Alert(Msg.NetworkConMsg);
                return;
            }
            CircularWaitDisplay = true;
            await userManager.ResetPassword(email, name);
            CircularWaitDisplay = false;
           await Msg.Alert(Msg.ResetPassMsg);
        }

        private async void CheckResponseTime(object o, ElapsedEventArgs e)
       {
           responseCounter += 1;
           if (responseCounter != StaticOpt.MaxResponseTime) return;
           tooLate = true;
           CircularWaitDisplay=false;
           responseCounter = 0;
          await Msg.AlertError(Msg.NetworkConMsg);
       }

//-----------------------------data binding methods ------------------------------------------------
       public bool CircularWaitDisplay
       {
           get => wait;
           set => SetProperty(ref wait,value);
       }
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
        public int SelectedHospitalExcess
        {
            get => hospitalExcess;
            set => SetProperty(ref hospitalExcess, value);
        }
        public int SelectedPlan
        {
            get => plan;
            set => SetProperty(ref plan, value);
        }

        public DateTime MinDate { get; } = DateTime.Now.AddYears(-65);
        public DateTime MaxDate { get; } = DateTime.Now.AddYears(-18);
        
        private DateTime selectedDate = DateTime.Now.AddYears(-18);
        

        public DateTime SelectedDate
        {
            get => selectedDate;
            set => SetProperty(ref selectedDate, value);
        }
        
        public bool IsSmoker
        {
            get => isSmokerChecker;
            set => SetProperty(ref isSmokerChecker, UpdateSmokerValue(value));
        }
        private bool UpdateSmokerValue(bool value)
        {
            smoker = value ? 1 : 0;
            return value;
        }
        
        private bool enabled=true;
        public bool IsEnabled
        {
            get => enabled;
            set => SetProperty(ref enabled, value);
        }

        private bool expiredCustomer=false;
        public bool IsExpiredCustomer
        {
            get => expiredCustomer;
            set => SetProperty(ref expiredCustomer, value);
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
            userManager?.Dispose();
        }
    }
}