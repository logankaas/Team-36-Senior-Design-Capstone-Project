using Microsoft.Maui.Controls;
using System;

namespace SeniorCapstoneProject.Converters
{
    public class BubbleMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isUser = value is bool b && b;
            return isUser ? new Thickness(80, 8, 16, 8) : new Thickness(16, 8, 80, 8);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}