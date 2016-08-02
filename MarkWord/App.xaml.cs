using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
namespace MarkWord
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            SplashScreen s = new SplashScreen("Images/loadImg.png");
            s.Show(true, true);

            BLL.FileManager.CheckMyDocFolder();//检查文件夹
            Config.ReadConfig();
            Config.ReadBlogsConfig();


            Application.Current.Resources.BeginInit();
            switch (Config.Common.Theme)
            {
                case ThemeStyle.Office2010:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Generic.xaml") });
                    Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                    break;
                case ThemeStyle.Office2013:
                    Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Fluent;component/Themes/Office2013/Generic.xaml") });
                    Application.Current.Resources.MergedDictionaries.RemoveAt(0);
                    break;
            }
            Application.Current.Resources.EndInit();


            base.OnStartup(e);
        

        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Config.SaveConfig();
        }
    }
}
