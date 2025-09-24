using Microsoft.Maui.Storage;

namespace SeniorCapstoneProject
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window(new NavigationPage(new MainPage()));
        }
    }
}

//namespace SeniorCapstoneProject
//{
//    public partial class App : Application
//    {
//        public App()
//        {
//            InitializeComponent();
//        }

//        protected override Window CreateWindow(IActivationState activationState)
//        {
//            return new Window(new NavigationPage(new MainPage()));
//        }
//    }
//}