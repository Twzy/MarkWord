using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// WinUploadProcess.xaml 的交互逻辑
    /// </summary>
    public partial class WinUploadProcess
    {
        public WinUploadProcess()
        {
            InitializeComponent();
            BLL.BlogsHelper.publishHander = UpdateProcess;//注册方法
        }

        bool Publish { get; set; }
        bool OpenWhenPublish { get; set; }
        bool publishAsNew { get; set; }
        string pwd = "";

        Thread tdPublish;

        public void Start()
        {
            tdPublish = new Thread(() =>
             {
                 string postId = BLL.BlogsHelper.UploadBlogs(Config.Blogs.BlogsMetaweblogUrl,
                                                Config.Blogs.BlogsID,
                                                Tools.Decrypt(Config.Blogs.BlogsUserId),
                                                pwd,
                                                Config.Blogs.BlogsModel,
                                               publishAsNew ? "" : Config.CurrBlogsDocument.PostId,
                                                Config.CurrBlogsDocument.Title,
                                                Config.CurrBlogsDocument.MarkDownContent,
                                                Publish);
                 Config.CurrBlogsDocument.PostId = postId;
                 if (this.OpenWhenPublish)
                 {
                     System.Diagnostics.Process.Start(Config.Blogs.BlogsUrl);
                 }

                 pane_input.Dispatcher.Invoke(new DisplayHandler(() =>
                 {
                     btnOK.IsEnabled = true;
                     btnCancel.IsEnabled = true;
                     pane_input.IsEnabled = true;
                 }));

             });
            tdPublish.IsBackground = true;
            tdPublish.Start();
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(txtTitle.Text))
            {
                Common.ShowMessage("请输入标题");
                return;
            }

            Config.CurrBlogsDocument.Title = txtTitle.Text;
            this.Publish = !(cbxPublish.IsChecked.HasValue && cbxPublish.IsChecked.Value);
            this.OpenWhenPublish = cbxOpen.IsChecked.HasValue && cbxOpen.IsChecked.Value;
            this.publishAsNew = cbxPubAsNew.IsChecked.HasValue && cbxPubAsNew.IsChecked.Value;

            this.pwd = Config.Blogs.BlogsRecodPassword ? Tools.Decrypt(Config.Blogs.BlogsPassword) : "";
            if (string.IsNullOrEmpty(Config.Blogs.BlogsMetaweblogUrl) ||
                string.IsNullOrEmpty(Config.Blogs.BlogsUserId) ||
                string.IsNullOrEmpty(this.pwd))
            {
                Common.ShowMessage("需要配置用户信息！");
                WinBlogsLogin tmepDia = new WinBlogsLogin();
                tmepDia.Owner = this;
                var result = tmepDia.ShowDialog();
                if (!result.HasValue || !result.Value)
                    return;//取消
                this.pwd = tmepDia.txtPwd.Password;
            }

            btnOK.IsEnabled = false;
            btnCancel.IsEnabled = false;
            pane_input.IsEnabled = false;


            Start();

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


        private void UpdateProcess(int maxCount, int Count, string msg)
        {
            prgBar.Dispatcher.Invoke(new DisplayHandler(() =>
            {
                prgBar.Maximum = maxCount;
                prgBar.Value = Count;
                lasStatus.Content = msg;
            }));
        }

        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.txtTitle.Text = Config.CurrBlogsDocument.Title;
            if (string.IsNullOrEmpty(Config.CurrBlogsDocument.PostId))
            {
                cbxPubAsNew.IsChecked = true;
                cbxPubAsNew.IsEnabled = false;
            }
            else
            {
                cbxPubAsNew.IsEnabled = true;
                cbxPubAsNew.IsChecked = false;
                labPostId.Content = Config.CurrBlogsDocument.PostId;
            }


        }

        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (tdPublish != null && tdPublish.IsAlive)
            {
                Common.ShowMessage("正在发布数据，请稍后……");
                e.Cancel = true;
            }
        }


    }
}
