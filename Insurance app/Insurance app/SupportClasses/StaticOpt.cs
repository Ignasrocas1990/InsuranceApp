using System;
using System.Collections.Generic;
using System.Linq;
using Insurance_app.Models;
using SkiaSharp;

using c = SkiaSharp.SKColors;

namespace Insurance_app.SupportClasses
{
    public static class StaticOpt
    {
        public static string MyRealmAppId = "application-0-bvutx";
        public static readonly double StepNeeded = 10000;
        public static readonly int MovUpdateArraySize = 5;
        public static readonly int MaxResponseTime = 60;
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
            if (fName.Length < 2 || fName.Length > 20 || HasNumbers(fName))
            {
                errors += " First name is invalid \n";
            }
            if (lName.Length < 4 || lName.Length > 20 || HasNumbers(lName))
            {
                errors += " Last name is invalid \n";
            }
            if (phoneNr.Length < 9 || !HasNumbers(phoneNr))
            {
                errors += " Phone nr is invalid \n";
            }
            
            
            if (email.Length < 15 || !email.Contains("@") || !email.Contains("."))
            {
                errors += " Email is invalid \n";
            }
            return errors;
        }

        public static string IsPasswordValid(string password)
        {
            string errors="";
            if (password.Length < 7)
            {
                errors += " Password is invalid \n";
            }

            return errors;
        }
        public static string IsClaimInfoValid(string hospitalPostcode, string patientNr)
        {
            string errors = "";
            if (hospitalPostcode.Length < 2 || hospitalPostcode.Length > 200)
            {
                errors += "Hospital Code is invalid length\n";
            }

            if (patientNr.Length < 2 || patientNr.Length > 200)
            {
                errors += "Patient Nr is invalid length";
            }
            return errors;
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


   
    }
}