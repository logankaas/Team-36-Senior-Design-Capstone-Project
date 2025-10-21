using Microsoft.Maui.Controls;
using System;

namespace SeniorCapstoneProject.Converters
{
    public class BubbleColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isUser = value is bool b && b;
            return isUser ? 1 : 0;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}