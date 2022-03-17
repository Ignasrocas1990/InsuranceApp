using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Insurance_app.Models;
using Insurance_app.Pages;
using Insurance_app.Pages.Popups;
using Realms.Sync;
using SkiaSharp;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using c = SkiaSharp.SKColors;

namespace Insurance_app.SupportClasses
{
    public static class StaticOpt
    {
        //public const string Url = "http://ec2-34-251-148-246.eu-west-1.compute.amazonaws.com/predict";
        
        public const string PredictUrl = "https://testRESTapi.pythonanywhere.com/predict";
        public const string EmailUrl = "https://testRESTapi.pythonanywhere.com/notifyCustomer";
        public const string PassResetEmailUrl = "https://testRESTapi.pythonanywhere.com/resetPass";
        public const string ClaimEmailUrl = "https://testRESTapi.pythonanywhere.com/ClaimNotifyCustomer";
        public const string CompanyCodeUrl = "https://testRESTapi.pythonanywhere.com/CompanyCode";
        
        public static string MyRealmAppId = "application-1-luybv";
        public static readonly double StepNeeded = 10000;
        public static readonly int MovUpdateArraySize = 5;
        public static readonly int MaxResponseTime = 120;
        public static readonly int MaxNameLen = 20;
        public static readonly int blockTime = 180;

        public static Func<string, bool> HasNumbers => s => s.Any(char.IsDigit);

        public static readonly SKColor White = c.WhiteSmoke;
        
        public static readonly SKColor[] ChartColors=
        {
            c.Blue,c.LightBlue,c.Red,c.Aqua,c.Black,c.LightGreen
            ,c.MediumVioletRed,c.Yellow,c.Orange,c.Green,c.Firebrick
        };
        public enum CoverEnum {Low,Medium,High}
        public enum PlanEnum {Low,Medium,High}

        public static IList<String> HospitalsEnum()
        {
            return new List<String>() {"Public Hospitals", "Most Hospitals", "All Hospitals"};
        }
        public static IList<int> ExcessFee()
        {
            return new List<int>() {300, 150, 0};
        }
        public static string InfoTest(string type)
        {
            switch (type)
            {
                case "Hospital":
                    return "Private & Public Hospitals"+
                           "~Most Hospitals"+
                           "~Public Hospitals"+
                           "~Covered by\npublic hospitals\nand hospitals such as\nSt Patrick's University Hospital\nBon Secours Hospital Glasnevin\nand other..." +
                           "~Covered by\npublic hospitals\nand selected private\nhospitals such as\nHermitage Medical Clinic\nSt Vincents Private Hospital\nand more..." +
                           "~Covered by\nlocal hospitals\nsuch as St James Hospital\nCappagh National Orthopaedic Hospital\n and other...";
                case "Cover":
                    return "Low cover~Medium cover~High cover"+
                           "~Covers minimum expenses\ntowards the doctor visits."+
                           "~Covers up to 70%\nof the doctor visits not\nincluding special cases."+
                           "~Full covers most of\nthe doctor visits";
                case "Fee":
                    return "150-300€~51-150€~€0-50€" +
                           "~You will pay between\n€150 - €300\nper normal hospital admission." +
                           "~You will pay between\n€51 - €175\nper normal hospital admission." +
                           "~You will pay between\n€0 - €50\nper normal hospital admission.";
                case "Plan":
                    return "Low Plan~Medium Plan~High Plan" +
                           "~Includes\nConsultants\nScans\nTherapies" +
                           "~Includes\n(Low Plan)+\nHealth coach\nNutritionist\nOptical\nPhysiotherapy" +
                           "~Includes\n(Medium Plan)+\nDental\nPersonal GP Online Care";
                default:
                    return "";
            }
        }

        public static string TempPassGenerator()
        {
            string sourceSL = "qwertyuiopasdfghjklzxcvbnm";
            string sourceCL = "MNBVCXZASDFGHJKLPOIUYTREWQ";
            string sourceN = "1234567890";
            string sourceS = "=-/?#][|!£$%^&*()_+";
            Random r = new Random();
            string pass = "";
            for (int i = 0; i < 9; i++)
            {
                if (i<3)
                {
                    pass += sourceCL[r.Next(0, sourceCL.Length-1)];
                }else if (i < 6)
                {
                    pass += sourceSL[r.Next(0, sourceSL.Length-1)];
                }else if (i < 8)
                {
                    pass += sourceN[r.Next(0, sourceN.Length-1)];
                }
                else
                {
                    pass += sourceS[r.Next(0, sourceS.Length-1)];
                }
                
            }
            
            return pass;
        }
        
        public static async Task InfoPopup(string type)
        {
            await Application.Current.MainPage.Navigation.ShowPopupAsync(new InfoPopup(type));
        }
        public static async Task Logout()
        {
            try
            {
                await App.RealmApp.RemoveUserAsync(App.RealmApp.CurrentUser);
                Application.Current.MainPage = new NavigationPage(new LogInPage());
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}