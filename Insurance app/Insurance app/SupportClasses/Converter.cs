using System;
using System.Globalization;
using Insurance_app.Pages;
using Insurance_app.ViewModels;
using Insurance_app.ViewModels.Popups;

namespace Insurance_app.SupportClasses
{
    public static class Converter
    {
        public static Func<String,float>StringToFloat =  x => float.Parse(x, CultureInfo.InvariantCulture.NumberFormat);
        public static Func<String, double> StringToDouble = x => Math.Round(Convert.ToDouble(x),2);
        public static Func<float, double> FloatToDouble = x => Math.Round(Convert.ToDouble(x), 2);

        private static string nameSpace = nameof(Insurance_app) + "." + nameof(ViewModels) + ".";


        public static string HomeViewModel = nameSpace+nameof(HomeViewModel);
        public static string ClaimViewModel = nameSpace+nameof(ClaimViewModel);
        public static string ProfileViewModel = nameSpace+nameof(ProfileViewModel);
        public static string ReportViewModel = nameSpace+nameof(ReportViewModel);

        //public static string LogInViewModel = nameof(Insurance_app)+"."+nameof(ViewModels)+"."+nameof(LogInViewModel);
        public static float GetPrice(string price) =>(float) Math.Round(float.Parse(price) * 100f) / 100f;

    }
}