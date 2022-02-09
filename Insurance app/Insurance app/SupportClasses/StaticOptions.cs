using System;
using System.Collections.Generic;
using System.Linq;
using Insurance_app.Models;
using SkiaSharp;

using c = SkiaSharp.SKColors;

namespace Insurance_app.SupportClasses
{
    public static class StaticOptions
    {
        public static string MyRealmAppId = "application-0-bvutx";
        public static readonly double StepNeeded = 10000;
        public static readonly int MovUpdateArraySize = 5;
        public static readonly int MaxResponseTime = 30;
        public static readonly string AgeLimitErrorMessage = "The age limit is between 18 and 65";
        public static readonly string ConnectionErrorMessage = "Network connectivity not available";
        public static readonly int MaxNameLen = 20;
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
        public static string IsValid(string fName,string lName,string phoneNr,string email)
        {
            var errors = "";
            if (fName.Length < 2 || fName.Length > 20 || StaticOptions.HasNumbers(fName))
            {
                errors += " First name is invalid \n";
            }
            if (lName.Length < 4 || lName.Length > 20 || StaticOptions.HasNumbers(lName))
            {
                errors += " Last name is invalid \n";
            }
            if (phoneNr.Length < 9 || !StaticOptions.HasNumbers(phoneNr))
            {
                errors += " Phone nr is invalid \n";
            }
            
            
            if (email.Length < 15 || !email.Contains("@") || !email.Contains("."))
            {
                errors += " Email is invalid \n";
            }
            return errors;
        }

        public static string isPasswordValid(string password)
        {
            string errors="";
            if (password.Length < 7)
            {
                errors += " Password is invalid \n";
            }

            return errors;
        }
        

    }
}