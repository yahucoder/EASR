using System;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("UbuntuRegular.ttf", Alias = "Ubuntu")]
[assembly: ExportFont("RobotoCondensed-Bold.ttf", Alias ="RobotoBold")]
[assembly: ExportFont("RobotoCondensed-BoldItalic.ttf", Alias = "RobotoBT")]
[assembly: ExportFont("RobotoCondensed-Italic.ttf", Alias = "RobotoItalic")]
[assembly: ExportFont("RobotoCondensed-Light.ttf", Alias = "RobotoLight")]
[assembly: ExportFont("RobotoCondensed-LightItalic.ttf", Alias = "RobotoLightItalic")]
[assembly: ExportFont("RobotoCondensed-Regular.ttf", Alias = "Roboto")]

namespace Easr
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());


        //MainPage = new MainPage();
    }

        protected override void OnStart()
        {
            
        }

        protected override void OnSleep()
        {
            
        }

        protected override void OnResume()
        {
            
        }

       
    }
}
