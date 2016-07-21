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
namespace MarkWord
{
    /// <summary>
    /// TableConvert.xaml 的交互逻辑
    /// </summary>
    public partial class WinTableConvert
    {
        /// <summary>
        /// 文本框
        /// </summary>
        System.Windows.Forms.RichTextBox winTextBox = new System.Windows.Forms.RichTextBox();
        public WinTableConvert()
        {
            InitializeComponent();

            winTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            winTextBox.AcceptsTab = true;
         
            winTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.winHost.Child = winTextBox;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            ConverToStr();
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        #region 业务
        /// <summary>
        /// 转为表格字符串
        /// </summary>
        public string MarkTableStr { get; set; }

        private void ConverToStr()
        {
            char splitChar = '\t';
            switch (cbxSpan.SelectedIndex)
            {
                case 0:
                    splitChar = '\t';
                    break;
                case 1:
                    splitChar = ',';
                    break;
                case 2:
                    splitChar = ' ';
                    break;
            }

            StringBuilder sbr = new StringBuilder();
            string[] Lines = winTextBox.Text.Replace("\r", "").Split('\n');
            foreach (var s in Lines)
            {
                string[] cells = s.Split(splitChar);
                sbr.Append("|");
                foreach (var c in cells)
                {
                    sbr.Append(c + "|");
                }
                sbr.Append("\r\n");
            }

            this.MarkTableStr = sbr.ToString();
        }

        #endregion

    }
}
