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
            return (double) value / HomeViewModel.StepNeeded;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}