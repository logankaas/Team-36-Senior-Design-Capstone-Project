using Microsoft.Maui.Controls;
using System;

namespace SeniorCapstoneProject.Converters
{
    public class BubbleColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool isUser = value is bool b && b;
            return isUser ? Color.FromArgb("#4F8CFF") : Color.FromArgb("#F2F4F8");
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}