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

            base.OnStartup(e);
        

        }
        protected override void OnExit(ExitEventArgs e)
        {
            base.OnExit(e);
            Config.SaveConfig();
        }
    }
}
