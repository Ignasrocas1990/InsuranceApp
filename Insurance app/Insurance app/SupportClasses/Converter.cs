using System;
using System.Globalization;
using Insurance_app.Pages;
using Insurance_app.ViewModels;

namespace Insurance_app.SupportClasses
{
    public static class Converter
    {
        public static Func<String,float>StringToFloat =  x => float.Parse(x, CultureInfo.InvariantCulture.NumberFormat);
        public static Func<long, DateTimeOffset> ToDTOffset = t => DateTimeOffset.FromUnixTimeMilliseconds(t);
        public static string nameSpace = nameof(Insurance_app) + "." + nameof(ViewModels) + ".";

        //public static string LogInViewModel = nameSpace+nameof(LogInViewModel);
        public static string HomeViewModel = nameSpace+nameof(HomeViewModel);
        //public static string QuoteViewModel = nameSpace+nameof(QuoteViewModel);
        //public static string RegistrationViewModel  = nameSpace+nameof(RegistrationViewModel);
        public static string CustomerViewModel  = nameSpace+nameof(CustomerViewModel);


        //public static string LogInViewModel = nameof(Insurance_app)+"."+nameof(ViewModels)+"."+nameof(LogInViewModel);
        public static float GetPrice(string price) =>(float) Math.Round(float.Parse(price) * 100f) / 100f;

    }
}