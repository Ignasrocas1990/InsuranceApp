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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Insurance_app.Pages;
using Insurance_app.Pages.Popups;
using SkiaSharp;
using Xamarin.CommunityToolkit.Extensions;
using Xamarin.Forms;
using c = SkiaSharp.SKColors;

namespace Insurance_app.SupportClasses
{
    /// <summary>
    /// Class used to store static option in regards to
    /// the application (colours, API urls, Temporary password generator etc...)
    /// </summary>
    public static class StaticOpt
    {
        private const string AwsDns = "http://ec2-54-216-17-229.eu-west-1.compute.amazonaws.com";
        public const string PredictUrl = $"{AwsDns}/predict";
        public const string PassResetEmailUrl = $"{AwsDns}/resetPass";
        public const string EmailUrl = $"{AwsDns}/notifyCustomer";
        public const string ClaimEmailUrl = $"{AwsDns}/ClaimNotifyCustomer";
        public const string CompanyCodeUrl = $"{AwsDns}/CompanyCode";
        public const string EmailConfirm = $"{AwsDns}/confirmationEmail";

        public static readonly string MyRealmAppId = "application-1-luybv";
        public const double StepNeeded = 10000;
        public static readonly double PercentPerStep = Math.Round(1 / (StepNeeded / 100),2);
        public const int MaxResponseTime = 120;
        
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
        
        /// <summary>
        /// Return's information in regards to Policy
        /// </summary>
        /// <param name="type">Type of information</param>
        /// <returns>Description string</returns>
        public static string InfoTest(string type)
        {
            return type switch
            {
                "Hospital" => "Private & Public Hospitals" + "~Most Hospitals" + "~Public Hospitals" +
                              "~Covered by\npublic hospitals\nand hospitals such as\nSt Patrick's University Hospital\nBon Secours Hospital Glasnevin\nand other..." +
                              "~Covered by\npublic hospitals\nand selected private\nhospitals such as\nHermitage Medical Clinic\nSt Vincents Private Hospital\nand more..." +
                              "~Covered by\nlocal hospitals\nsuch as St James Hospital\nCappagh National Orthopaedic Hospital\n and other...",
                "Cover" => "Low cover~Medium cover~High cover" +
                           "~Covers minimum expenses\ntowards the doctor visits." +
                           "~Covers up to 70%\nof the doctor visits not\nincluding special cases." +
                           "~Full covers most of\nthe doctor visits",
                "Fee" => "150-300€~51-150€~€0-50€" +
                         "~You will pay between\n€150 - €300\nper normal hospital admission." +
                         "~You will pay between\n€51 - €175\nper normal hospital admission." +
                         "~You will pay between\n€0 - €50\nper normal hospital admission.",
                "Plan" => "Low Plan~Medium Plan~High Plan" + "~Includes\nConsultants\nScans\nTherapies" +
                          "~Includes\n(Low Plan)+\nHealth coach\nNutritionist\nOptical\nPhysiotherapy" +
                          "~Includes\n(Medium Plan)+\nDental\nPersonal GP Online Care",
                _ => ""
            };
        }

        /// <summary>
        /// Creates a temporary password/email confirm code
        /// </summary>
        /// <param name="length">length of password int</param>
        /// <param name="forPass">is is for password or a email confirmation bool</param>
        /// <returns></returns>
        public static string TempPassGenerator(int length,bool forPass)
        {
            const string sourceSl = "qwertyuiopasdfghjklzxcvbnm";
            const string sourceCl = "MNBVCXZASDFGHJKLPOIUYTREWQ";
            const string sourceN = "1234567890";
            var sourceS = "=-/?#][|!£$%^&*()_+";
            if (!forPass)
            {
                sourceS = "qwertyuiopasdfghjklzxcvbnm";
            }
            var r = new Random();
            var pass = "";
            for (var i = 0; i <= length; i++)
            {
                
                if (i<2)
                {
                    pass += sourceSl[r.Next(0, sourceSl.Length-1)];
                }else if (i < 4)
                {
                    pass += sourceCl[r.Next(0, sourceCl.Length-1)];
                }else if (i < 6)
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
        
        public static readonly Func<string,float>StringToFloat =  x => float.Parse(x, CultureInfo.InvariantCulture.NumberFormat);
        public static readonly Func<float?, double> FloatToDouble = x => Math.Round(Convert.ToDouble(x), 2);
        
        /// <summary>
        /// Converts and rounds the price 2 dec places
        /// </summary>
        /// <param name="price">full price string</param>
        /// <returns>price rounded float</returns>
        public static float GetPrice(string price) =>(float) Math.Round(float.Parse(price) * 100f) / 100f;
        
        /// <summary>
        /// Displays informational pop up
        /// (Using Xamarin community tool kit)
        /// </summary>
        /// <param name="type">Type of informational pop up string</param>
        public static async Task InfoPopup(string type)
        {
            await Application.Current.MainPage.Navigation.ShowPopupAsync(new InfoPopup(type));
        }
        
        /// <summary>
        /// logout Realm authorized user
        /// and navigates current view to log in page
        /// </summary>
        public static async Task Logout()
        {
            try
            {
                if (App.RealmApp.CurrentUser is null) return;
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