using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkWord
{
    /// <summary>
    /// 配置类型
    /// </summary>
    public class Config
    {
        /// <summary>
        /// 初始化
        /// </summary>
        static Config()
        {
            Common = new Common();//基本配置信息
            Common.WorkType = WorkType.Both;
            Common.EditFontStr = "Consolas";
            Common.EditFontIndex = 6;
            Common.LineBar = true;
            Common.ShowEndVl = false;
            Common.ShowTabs = false;
            Common.ShowSpace = false;
            Common.ShowHightLine = false;
            Common.BackGround = System.Windows.Media.Colors.Transparent;
            Common.StyleName = "DefaultStyle";
            Common.Signatrue = "";
            Common.Theme = ThemeStyle.Office2013;
            Common.FileList = new List<string>();

            Blogs = new BlogsInfo();//博客配置信息 
            CurrBlogsDocument = new BlogsDocumentInfo();//当前选择的文章的信息（主要用于草稿更新）


        }

        /// <summary>
        /// 当前选择的文章的信息（主要用于草稿更新）
        /// </summary>
        public static BlogsDocumentInfo CurrBlogsDocument { get; set; }
        /// <summary>
        /// 博客配置信息 
        /// </summary>
        public static BlogsInfo Blogs { get; set; }

        /// <summary>
        /// 配置信息
        /// </summary>
        public static Common Common { get; set; }

        /// <summary>
        /// 样式文件夹
        /// </summary>
        public static string StyleDir
        {
            get
            {
                return Environment.CurrentDirectory + "\\Source\\Style";
            }
        }
        /// <summary>
        /// 脚本文件夹
        /// </summary>
        public static string ScriptDir
        {
            get
            {
                return Environment.CurrentDirectory + "\\Source\\Script";
            }
        }

        /// <summary>
        /// 配置文件
        /// </summary>
        public static string ConfigFile
        {
            get
            {
                return Environment.CurrentDirectory + "\\Config\\MainSetting.xml";
            }
        }


        public static string MarkdownHelp
        {
            get
            {
                return Environment.CurrentDirectory + "\\Config\\markword.html";
            }
        }

        public static string MyDocumentFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\MarkWord\\";

        /// <summary>
        /// 博客配置文件，保存在一些特殊的文件夹下，防止因为直接复制软件而造成账号泄露
        /// </summary>
        public static string BlogsConfigFile
        {
            get
            {
                return MyDocumentFolder + "\\setting.bin";
            }
        }

        /// <summary>
        /// 样式字符字符串
        /// </summary>
        public static string Style { get; set; }

        public static Dictionary<string, string> StyleList { get; internal set; }

        /// <summary>
        /// 标题
        /// </summary>
        public static string Title
        {
            get
            {
                return "MarkWord";
            }
        }




        /// <summary>
        /// 读取配置信息
        /// </summary>
        public static void ReadConfig()
        {
            if (!System.IO.File.Exists(ConfigFile))
            {
                return;
            }
            Config.Common = Tools.ReadByXML<Common>(ConfigFile);


        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        public static void SaveConfig()
        {
            Tools.WriteByXML<Common>(Config.Common, ConfigFile);
        }


        /// <summary>
        /// 读取博客信息
        /// </summary>
        public static void ReadBlogsConfig()
        {
            //加载博客信息
            if (!System.IO.File.Exists(Config.BlogsConfigFile))
            {
                return;
            }
            try
            {
                Config.Blogs = Tools.ReadByBinary<BlogsInfo>(Config.BlogsConfigFile);
            }
            catch (Exception ex)
            { }
        }

        /// <summary>
        /// 保存博客信息
        /// </summary>
        public static void SaveBlogsConfig()
        {
            Tools.WriteByBinary<BlogsInfo>(Config.Blogs, Config.BlogsConfigFile);
        }


    }
}
