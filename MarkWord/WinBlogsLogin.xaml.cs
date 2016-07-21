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
using BlogsAPI;
namespace MarkWord
{
    /// <summary>
    /// BlogsLogin.xaml 的交互逻辑
    /// </summary>
    public partial class WinBlogsLogin
    {
        public WinBlogsLogin()
        {
            InitializeComponent();
        }

        MetaTools metaTools = new MetaTools();
     
        private void RibbonWindow_Loaded(object sender, RoutedEventArgs e)
        {
            txtAPIUrl.Text = Config.Blogs.BlogsMetaweblogUrl;
            txtUser.Text = Tools.Decrypt(Config.Blogs.BlogsUserId);
            txtModel.Text = Config.Blogs.BlogsModel;

            cbxRecodePwd.IsChecked = Config.Blogs.BlogsRecodPassword;
            if (cbxRecodePwd.IsChecked.HasValue && cbxRecodePwd.IsChecked.Value)
            {
                txtPwd.Password = Tools.Decrypt(Config.Blogs.BlogsPassword);
            }

            labBlogTitle.Content = Config.Blogs.BlogsName;
            labBlogUrl.Content = Config.Blogs.BlogsUrl;
        }


        private bool Login()
        {
            if (string.IsNullOrEmpty(txtAPIUrl.Text) || string.IsNullOrEmpty(txtUser.Text) || string.IsNullOrEmpty(txtPwd.Password))
            {
                Common.ShowMessage("请正确输入配置信息");
                return false;
            }

            metaTools.Url = txtAPIUrl.Text;
           
            Config.Blogs.BlogsMetaweblogUrl = txtAPIUrl.Text;
            Config.Blogs.BlogsUserId = Tools.Encrypt(txtUser.Text);
            Config.Blogs.BlogsRecodPassword = cbxRecodePwd.IsChecked.HasValue && cbxRecodePwd.IsChecked.Value;
            Config.Blogs.BlogsModel = txtModel.Text;

            if (cbxRecodePwd.IsChecked.HasValue && cbxRecodePwd.IsChecked.Value)
            {
                Config.Blogs.BlogsPassword = Tools.Encrypt(txtPwd.Password);
            }
            else
            {
                Config.Blogs.BlogsPassword = "";
            }

            try
            {

                var result = metaTools.getUsersBlogs(string.Empty, txtUser.Text, txtPwd.Password);
                if (result.Length > 0)
                {
                    Config.Blogs.BlogsID = result[0].blogid;
                    labBlogTitle.Content = Config.Blogs.BlogsName = result[0].blogName;
                    labBlogUrl.Content = Config.Blogs.BlogsUrl = result[0].url;

                    Config.SaveBlogsConfig();

                    return true;
                }
                else
                {
                    Config.Blogs.BlogsID = string.Empty;
                    labBlogTitle.Content = Config.Blogs.BlogsName = string.Empty;
                    labBlogUrl.Content = Config.Blogs.BlogsUrl = string.Empty;

                    Common.ShowMessage("校验失败，请检查登陆信息！");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Common.ShowMessage(ex.Message);
                return false;
            }
        }



        /// <summary>
        /// 验证
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            Login();
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (Login())
            {
                Common.ShowMessage("账户设置成功");
                this.DialogResult = true;
            }
        }
        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }


    }
}
