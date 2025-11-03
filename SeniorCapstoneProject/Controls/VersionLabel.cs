using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;

namespace SeniorCapstoneProject.Controls
{
    public class VersionLabel : Label
    {
        public VersionLabel()
        {
            Text = $"Version {VersionTracking.CurrentVersion}";
            FontSize = 12;
            HorizontalOptions = LayoutOptions.Center;
            VerticalOptions = LayoutOptions.End;
            TextColor = Colors.Gray;
        }
    }
}