using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Graphics;
using Android.Views;

namespace SeniorCapstoneProject
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, LaunchMode = LaunchMode.SingleTop, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Window?.SetStatusBarColor(Android.Graphics.Color.ParseColor("#FFFFFF"));

            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                var decorView = Window?.DecorView;
                if (decorView != null)
                {
                    decorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
                }
            }
        }
    }
}