using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MarkWord
{
    /// <summary>
    /// 公共配置类型
    /// </summary>
    [Serializable]
    public class Common
    {
        /// <summary>
        /// 工作类型
        /// </summary>
        public WorkType WorkType { get; set; }
        /// <summary>
        /// 字体字符串
        /// </summary>
        public string EditFontStr { get; set; }
        /// <summary>
        /// 字体索引
        /// </summary>
        public int EditFontIndex { get; set; }

        /// <summary>
        /// 是否显示行号
        /// </summary>
        public bool LineBar { get; set; }

        public bool ShowEndVl { get; set; }

        public bool ShowTabs { get; set; }

        public bool ShowSpace { get; set; }

        public bool ShowHightLine { get; set; }

        public Color BackGround { get; set; }

        public string StyleName { get; set; }
        /// <summary>
        /// 签名
        /// </summary>
        public string Signatrue { get; set; }

        public ThemeStyle Theme { get; set; }

        public List<string> FileList { get; set; }

        public static void ShowMessage(string msg)
        {
            MessageBox.Show(msg);
        }

        public static MessageBoxResult ShowCorfmMessage(string msg)
        {
            return MessageBox.Show(msg, "提示", MessageBoxButton.YesNoCancel);
        }
        public static TextBlock labStatus { get; set; }

        public static void ShowStatusMessage(string msg)
        {
            labStatus.Text = msg;
        }
    }
}
