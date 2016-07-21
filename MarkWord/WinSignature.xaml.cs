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
    /// WinSignature.xaml 的交互逻辑
    /// </summary>
    public partial class WinSignature 
    {
        public WinSignature()
        {
            InitializeComponent();

            this.txtSignature.Text = Config.Common.Signatrue;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.Signatrue = txtSignature.Text;
            this.DialogResult = true;
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
