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
    public class QuoteViewModel : ObservableObject
    {
        public ICommand GetQuotCommand { get; }
        public ICommand ResetPasswordCommand { get; }
        private int responseCounter = 0;
        private HttpService api;
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
        private string policyId = "";
        
        public ICommand InfoCommand { get; }
        public IList<string> HospitalList { get; }
        //age
        public IList<string> CoverList { get; }
        public IList<int> HospitalFeeList { get; }
        public IList<string> PlanList { get; }
        private UserManager userManager;
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
           api = new HttpService();
           InfoCommand = new AsyncCommand<string>(StaticOpt.InfoPopup);
           ResetPasswordCommand = new AsyncCommand(ResetPassword);
           this.policyId = policyId;
           userManager = new UserManager();
       }
        public async Task SetUp()
        {
            IsExpiredCustomer = false;
            if (policyId.Equals("")) return;
            SetUpWaitDisplay = true;
            try
            {
                //ObjectId.Parse(policyId);
               var customer = await userManager.GetCustomer(App.RealmApp.CurrentUser, App.RealmApp.CurrentUser.Id);
               if (customer.Dob != null) SelectedDate = customer.Dob.Value.UtcDateTime;
               email = customer.Email;
               name = customer.Name;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            IsExpiredCustomer = true;
            SetUpWaitDisplay = false;
        }
        
        private async Task GetQuote()
       {
           if (!App.NetConnection())
           {
               await Application.Current.MainPage.DisplayAlert("error",StaticOpt.NetworkConMsg, "close");
               return;
           }
           if (elegalChars != "")
           {
               await Application.Current.MainPage.DisplayAlert("Error",elegalChars , "close");
               return;
           }
            
           var age = DateTime.Now.Year - selectedDate.Year;
          
           string price;
           
           try
           {
               CircularWaitDisplay=true;
                timer.Start();
                price =  await api.SendQuoteRequest(hospitals, age, cover, hospitalExcess, plan, smoker);
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
                await Application.Current.MainPage.DisplayAlert("Error", "Something went wrong, try again in a min", "close");
               return;
           }
           CircularWaitDisplay=false;
           responseCounter = 0;
            bool action = await Application.Current.MainPage.DisplayAlert("Price",$"Price for the quote is : {price}",  "Accept","Deny");
           if (action && policyId=="")
           {
               await TransferToRegistration(age,price);
           }else if (action)
           {
               //TODO   update policy etc... and navigate back to log in screen
           }
       }

        private async Task TransferToRegistration(int age,string price)
        {
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
                await Application.Current.MainPage.Navigation.PushAsync(new RegistrationPage(tempQuote,price));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        private async Task ResetPassword()
        {
            CircularWaitDisplay = true;
            try
            {
                if (!App.NetConnection())
                {
                    await Application.Current.MainPage.DisplayAlert("Notice", StaticOpt.NetworkConMsg, "close");
                    throw new Exception();
                }
                var tempPass = StaticOpt.TempPassGenerator();// TODO NEED TO UPDATE API FIRST 
                await App.RealmApp.EmailPasswordAuth.CallResetPasswordFunctionAsync(email,tempPass);
                api.ResetPasswordEmail(email,name, DateTime.Now, tempPass);
                await Application.Current.MainPage.DisplayAlert(
                    "Notice", "The temporary password has been send to account email.", "close");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            CircularWaitDisplay = false;
        }

        private async void CheckResponseTime(object o, ElapsedEventArgs e)
       {
           responseCounter += 1;
           if (responseCounter != StaticOpt.MaxResponseTime) return;
           tooLate = true;
           CircularWaitDisplay=false;
           responseCounter = 0;
           await Application.Current.MainPage.DisplayAlert("Error",StaticOpt.NetworkConMsg, "close");
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

       
    }
}