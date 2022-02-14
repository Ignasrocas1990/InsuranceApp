using System;
using System.Globalization;
using Xamarin.Forms;

namespace Insurance_app.SupportClasses
{
    public class ValueProgressBarConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                return (double) value / StaticOpt.StepNeeded;

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}