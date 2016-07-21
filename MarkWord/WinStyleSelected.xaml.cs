using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Fluent;
using ICSharpCode.AvalonEdit.Highlighting;
using System.IO;
namespace MarkWord
{
    /// <summary>
    /// WinStyleSelected.xaml 的交互逻辑
    /// </summary>
    public partial class WinStyleSelected
    {
        public WinStyleSelected()
        {
            InitializeComponent();

            cssEditer.textEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinition("CSS");
            cssEditer.border.Margin = new Thickness(0);
            cssEditer.textEditor.Margin = new Thickness(0);


        }


        public bool StyleAddModel = false;


        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (StyleAddModel)
            {
                btnNewCss_Click(null, null);
            }
            else
            {
                string[] files = System.IO.Directory.GetFiles(Config.StyleDir);
                foreach (var f in files)
                {
                    cbxStyleList.Items.Add(System.IO.Path.GetFileNameWithoutExtension(f));
                }
            }
        }


        public bool NewSave()
        {
            string fileName = System.IO.Path.Combine(Config.StyleDir, cbxStyleList + ".css");
            if (File.Exists(fileName))
            {
                Common.ShowMessage("改样式名称已经存在，请重新命名。");
                return false;
            }
            try
            {
                File.WriteAllText(fileName, cssEditer.textEditor.Text, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Common.ShowMessage("保存样式异常：" + ex.Message);
                return false;
            }
        }

        public bool EditSave()
        {
            string fileName = System.IO.Path.Combine(Config.StyleDir, cbxStyleList.Text + ".css");
            try
            {
                File.WriteAllText(fileName, cssEditer.textEditor.Text, Encoding.UTF8);
                return true;
            }
            catch (Exception ex)
            {
                Common.ShowMessage("保存样式异常：" + ex.Message);
                return false;
            }
        }

        private void btnNewCss_Click(object sender, RoutedEventArgs e)
        {
            cbxStyleList.IsEditable = true;
            cbxStyleList.Text = "newStyle_" + DateTime.Now.ToLocalTime().ToString("yyyyMMddHHmmss");
            cssEditer.textEditor.Text = @"/*
开始编辑，" + DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss") + @"
*/";

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (StyleAddModel)
            {
                NewSave();
            }
            else
            {
                EditSave();
            }
        }

        private void cbxStyleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string fileName = System.IO.Path.Combine(Config.StyleDir, cbxStyleList.Text + ".css");
            if (!File.Exists(fileName))
            {
                cssEditer.textEditor.Text = @"/*
无可用数据
*/";
                return;
            }
            cssEditer.textEditor.Text = File.ReadAllText(fileName, Encoding.UTF8);
        }


    }
}
