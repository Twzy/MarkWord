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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarkWord
{
    /// <summary>
    /// WebDoc.xaml 的交互逻辑
    /// </summary>
    public partial class WebDoc : UserControl
    {


        public WebDoc()
        {
            InitializeComponent();

            IniteWebBrowser();
        }
        //winForm webBrowser
        public System.Windows.Forms.WebBrowser winWebDoc;
        //Awesomium.Windows.Forms.WebControl winWebDoc;
        /// <summary>
        /// 初始化
        /// </summary>
        private void IniteWebBrowser()
        {
            winWebDoc = new System.Windows.Forms.WebBrowser();
            winWebDoc.ScriptErrorsSuppressed = true;
            winWebDoc.AllowNavigation = false;
            winWebDoc.AllowWebBrowserDrop = false;
            winWebDoc.WebBrowserShortcutsEnabled = false;
            winWebDoc.IsWebBrowserContextMenuEnabled = false;
            winWebDoc.NewWindow += WinWebDoc_NewWindow;
            winWebDoc.Dock = System.Windows.Forms.DockStyle.Fill;
            docViewer.Child = winWebDoc;


            //winWebDoc = new Awesomium.Windows.Forms.WebControl();
            //winWebDoc.Dock = System.Windows.Forms.DockStyle.Fill;
            //winWebDoc.Size = new System.Drawing.Size(10, 10);
            //docViewer.Child = winWebDoc;

        }

        private void WinWebDoc_NewWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
        }



        /// <summary>
        /// 加载样式
        /// </summary>
        /// <param name="styleStr"></param>
        /// <param name="bodyStr"></param>
        public void LoadAllHTML(string MarkValue)
        {
            if (winWebDoc.Document == null)
            {
                winWebDoc.DocumentText = "";
            }
            else
            {
                winWebDoc.Document.OpenNew(true);
            }
            winWebDoc.Document.Write(BLL.HtmlHelper.MarkToHTMlPage(MarkValue));
            // winWebDoc.LoadHTML(BLL.HtmlHelper.MarkToHTMlPage(MarkValue));
        }





        /// <summary>
        /// 添加web数据
        /// </summary>
        /// <param name="value"></param>
        public void LoadBody(string MarkValue)
        {

            if (winWebDoc.Document == null)
                return;
            winWebDoc.Document.InvokeScript("updatePageContent", new object[] { CommonMark.CommonMarkConverter.Convert(MarkValue) });
        }


        public void ScrollAuto(double value)
        {
            if (winWebDoc.Document == null)
                return;
            winWebDoc.Document.InvokeScript("scrollToPageContent", new object[] { value.ToString(System.Globalization.CultureInfo.InvariantCulture) });
            //if (!winWebDoc.IsDocumentReady)
            //    return;
            //string javascript = "(function(){ alert(document.body.scrollHeight - document.body.clientHeight); })(); "; //string.Format("(function(val){{ scrollToPageContent(val);}})({0});", value.ToString(System.Globalization.CultureInfo.InvariantCulture));
            //winWebDoc.ExecuteJavascript(javascript);
        }

    }
}
