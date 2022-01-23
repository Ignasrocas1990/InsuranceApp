using System;
using System.Globalization;
using Insurance_app.ViewModels;
using Xamarin.Forms;

namespace Insurance_app
{
    public class ValueProgressBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //if 30 sec is your maximum time
            // return (double)value/30;

            //if 60 sec if your maximum time
            return (double) value / HomePageViewModel.StepNeeded;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}