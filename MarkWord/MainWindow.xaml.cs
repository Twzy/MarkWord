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
using Fluent;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using MarkWord.ViewModels;
using ICSharpCode.AvalonEdit.Document;
using System.Text.RegularExpressions;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using System.Threading;
using System.Windows.Threading;

namespace MarkWord
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : RibbonWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            btnTheme2010.IsChecked = Config.Common.Theme == ThemeStyle.Office2010;
            btnTheme2013.IsChecked = Config.Common.Theme == ThemeStyle.Office2013;

            Common.labStatus = this.LabStatus;
            IniteWorkArea();
        }

        public double[] FontSizeList
        {
            get
            {
                return new double[] { 7, 8, 9, 10, 11, 12, 14, 16, 18, 20, 22, 24, 28, 32, 36, 48, 72 };
            }
        }
        /// <summary>
        /// 初始化工作区
        /// </summary>
        private void IniteWorkArea()
        {
            WebmarkHelp.Navigate(Config.MarkdownHelp);

            //编辑器
            markEdit.MarkDoc = this.markDoc;

            #region 加载编辑器配置

            #region 字体
            //初始化工具栏
            var viewFonts = new ViewModels.FontsViewModel();
            int viewDefaultFontIndex = 0;
            foreach (var i in viewFonts.FontsData)
            {

                comboBoxFontName.Items.Add(new TextBlock()
                {
                    Text = i,
                    FontFamily = new FontFamily(i)
                });
                if (i == Config.Common.EditFontStr)
                {
                    comboBoxFontName.SelectedIndex = viewDefaultFontIndex;
                }
                viewDefaultFontIndex++;
            }
            if (comboBoxFontSize.SelectedIndex < 0)
            {
                comboBoxFontSize.SelectedIndex = 0;
            }
            #endregion

            #region 字体数字
            var fontIndex = Config.Common.EditFontIndex;
            foreach (var i in FontSizeList)
            {
                comboBoxFontSize.Items.Add(new TextBlock()
                {
                    Text = i.ToString()
                });
            }
            comboBoxFontSize.SelectedIndex = fontIndex;
            #endregion


            buttonVEndLine.IsChecked = Config.Common.ShowEndVl;
            markEdit.textEditor.Options.ShowEndOfLine = Config.Common.ShowEndVl;

            buttonVTab.IsChecked = Config.Common.ShowTabs;
            markEdit.textEditor.Options.ShowTabs = Config.Common.ShowTabs;

            buttonVSpaces.IsChecked = Config.Common.ShowSpace;
            markEdit.textEditor.Options.ShowSpaces = Config.Common.ShowSpace;

            buttonNumberBar.IsChecked = Config.Common.LineBar;
            markEdit.textEditor.ShowLineNumbers = Config.Common.LineBar;

            buttonCurrLine.IsChecked = Config.Common.ShowHightLine;
            markEdit.textEditor.Options.HighlightCurrentLine = Config.Common.ShowHightLine;

            BtneditBackColor.Fill = new SolidColorBrush(Config.Common.BackGround);
            markEdit.textEditor.Background = new SolidColorBrush(Config.Common.BackGround);
            #endregion


            //初始化样式
            InisteStyle();

            //文件夹
            foreach (var i in Config.Common.FileList)
            {
                lstFileList.Items.Add(i);
            }
            //test
            // markEdit.LoadText(Properties.Resources.test);

            markDoc.LoadAllHTML(markEdit.textEditor.Text);

            //设置工作模式

            SetWorkArea();
        }

        /// <summary>
        /// 初始化样式
        /// </summary>
        private void InisteStyle()
        {
            BLL.FileManager.GetStyleList();//获取样式列表

            ImageSource iconBursh = new BitmapImage(new Uri("pack://application:,,,/Images/stylechange32.png", UriKind.RelativeOrAbsolute));

            //加载样式
            gropStyleList.Items.Clear();
            foreach (var i in Config.StyleList)
            {
                var toggleBtn = new ToggleButton()
                {
                    GroupName = "groupStyle",
                    Tag = i.Value,
                    SizeDefinition = new RibbonControlSizeDefinition("Large"),
                    Header = i.Key,
                    LargeIcon = iconBursh,
                };
                toggleBtn.Click += StyleChange_Click;
                gropStyleList.Items.Add(toggleBtn);
            }

            ChangeStyle(Config.Common.StyleName);
            //设置样式

        }


        private void ChangeStyle(string cssName)
        {
            foreach (ToggleButton s in gropStyleList.Items)
            {
                if ((s.Header as string) == cssName)
                {
                    s.IsChecked = true;
                    BLL.FileManager.LoadStyle(s.Header as string);
                }
            }
            markDoc.LoadAllHTML(markEdit.textEditor.Text);
        }



        private void SetWorkArea()
        {
            if (Config.Common.WorkType == WorkType.Edit)
            {

                this.grdView.MinWidth = 0;
                this.grdCode.MinWidth = 0;

                this.grdCode.Width = new GridLength(1, GridUnitType.Star);


                this.grdView.Width = new GridLength(0, GridUnitType.Star);

                grdSplitter.IsEnabled = false;

                this.btnEditSwitch.Content = "编辑";
                btnEdit.IsChecked = true;
            }
            else if (Config.Common.WorkType == WorkType.Display)
            {

                this.grdView.MinWidth = 0;
                this.grdCode.MinWidth = 0;

                this.grdCode.Width = new GridLength(0, GridUnitType.Star);
                this.grdView.Width = new GridLength(1, GridUnitType.Star);

                grdSplitter.IsEnabled = false;

                this.btnEditSwitch.Content = "查看";
                btnDisplay.IsChecked = true;

                markDoc.LoadAllHTML(this.markEdit.textEditor.Text);
            }
            else
            {
                this.grdView.MinWidth = 200;
                this.grdCode.MinWidth = 200;

                this.grdCode.Width = new GridLength(1, GridUnitType.Star);
                this.grdView.Width = new GridLength(1, GridUnitType.Star);

                grdSplitter.IsEnabled = true;

                this.btnEditSwitch.Content = "同步预览";
                btnScanling.IsChecked = true;
                markDoc.LoadAllHTML(this.markEdit.textEditor.Text);
            }
        }

        private void btnCssEdit_Click(object sender, RoutedEventArgs e)
        {
            WinStyleSelected ws = new WinStyleSelected();
            ws.Owner = this;
            ws.ShowDialog();
        }


        #region Ribbon方法

        #region 常规
        /// <summary>
        /// 打开文档
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOpenDoc_Click(object sender, RoutedEventArgs e)
        {
            this.OpenDoc();
        }
        private void btnSaveDoc_Click(object sender, RoutedEventArgs e)
        {
            this.SaveDoc();
        }
        private void btnSaveAsDoc_Click(object sender, RoutedEventArgs e)
        {
            this.SaveDocAsNew();
        }
        private void btnSelectAll_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.SelectAll();
        }

        private void btnPaste_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.Paste();
        }
        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.Copy();
        }

        private void btnCut_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.Cut();
        }
        #endregion

        #region 编辑器
        private void comboBoxFontName_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            markEdit.textEditor.FontFamily = new FontFamily(((TextBlock)comboBoxFontName.SelectedValue).Text);
            Config.Common.EditFontStr = ((TextBlock)comboBoxFontName.SelectedValue).Text;
        }


        private void comboBoxFontSize_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            markEdit.textEditor.FontSize = double.Parse(((TextBlock)comboBoxFontSize.SelectedValue).Text);
            Config.Common.EditFontIndex = comboBoxFontSize.SelectedIndex;
        }

        private void buttonGrowFont_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxFontSize.SelectedIndex < comboBoxFontSize.Items.Count - 1)
                comboBoxFontSize.SelectedIndex++;
        }

        private void buttonShrinkFont_Click(object sender, RoutedEventArgs e)
        {
            if (comboBoxFontSize.SelectedIndex > 0)
                comboBoxFontSize.SelectedIndex--;
        }
        private void colorGallery_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            if (colorGallery.SelectedColor != null)
            {
                markEdit.textEditor.Background = new SolidColorBrush(colorGallery.SelectedColor.Value);
                Config.Common.BackGround = colorGallery.SelectedColor.Value;
                BtneditBackColor.Fill = new SolidColorBrush(Config.Common.BackGround);
            }
        }
        private void buttonVEndLine_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.ShowEndVl = markEdit.textEditor.Options.ShowEndOfLine = buttonVEndLine.IsChecked.HasValue && buttonVEndLine.IsChecked.Value;

        }

        private void buttonVTab_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.ShowTabs = markEdit.textEditor.Options.ShowTabs = buttonVTab.IsChecked.HasValue && buttonVTab.IsChecked.Value;
        }

        private void buttonVSpaces_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.ShowSpace = markEdit.textEditor.Options.ShowSpaces = buttonVSpaces.IsChecked.HasValue && buttonVSpaces.IsChecked.Value;
        }

        private void buttonNumberBar_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.LineBar = markEdit.textEditor.ShowLineNumbers = buttonNumberBar.IsChecked.HasValue && buttonNumberBar.IsChecked.Value;
        }

        private void buttonCurrLine_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.ShowHightLine = markEdit.textEditor.Options.HighlightCurrentLine = buttonCurrLine.IsChecked.HasValue && buttonCurrLine.IsChecked.Value;
        }
        #endregion

        #region 插入Markdown

        private void btnTitle1_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("# ");
        }

        private void btnTitle2_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("## ");
        }

        private void btnTitle3_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("### ");
        }

        private void btnTitle4_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("#### ");
        }

        private void btnTitle5_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("##### ");
        }

        private void btnTitle6_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("###### ");
        }

        private void buttonMarkBlod_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("**");
        }

        private void buttonMarkItalic_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("*");
        }

        private void buttonMarkHref_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.SelectedText = "[Title](http://)";
        }

        private void buttonMarkImage_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.SelectedText = "![img](http://)";
        }

        private void btnImageFromFile_Click(object sender, RoutedEventArgs e)
        {
            var tmpResult = BLL.FileManager.GetImgFilePath();
            if (!string.IsNullOrEmpty(tmpResult))
            {
                markEdit.textEditor.SelectedText = string.Format("![img](file:///{0})", tmpResult.Replace('\\', '/'));
            }
        }

        private void buttonMarkTable_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.SelectedText = "| |\r\n| |";
        }
        private void btnMarkConvertToTable_Click(object sender, RoutedEventArgs e)
        {
            WinTableConvert tc = new WinTableConvert();
            tc.Owner = this;
            var tbResult = tc.ShowDialog();
            if (tbResult.HasValue && tbResult.Value)
            {
                markEdit.textEditor.SelectedText = tc.MarkTableStr;
            }

        }


        private void buttonMarkCode_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("` ");
        }
        private void buttonMarkCodeSinpper_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleSymmetricalMarkdownFormatting("``` \r\n");
        }
        private void buttonMarkUse_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleAsymmetricMarkdownFormatting("> ");
        }

        private void buttonMarkUL_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleAsymmetricMarkdownFormatting("+ ");
        }
        private void buttonMarkOL_Click(object sender, RoutedEventArgs e)
        {
            markEdit.ToggleAsymmetricMarkdownFormatting("1. ");
        }
        private void buttonMarkLine_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.SelectedText = "---";
        }

        private void buttonMarkTime_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.SelectedText = DateTime.Now.ToLocalTime().ToString();
        }

        private void btnMarkSignature_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.SelectedText = Config.Common.Signatrue;
        }
        private void btnEditMarkSignature_Click(object sender, RoutedEventArgs e)
        {
            var signatureForm = new WinSignature();
            signatureForm.Owner = this;
            signatureForm.ShowDialog();
        }


        #endregion

        #region 操作
        private void btnFind_Click(object sender, RoutedEventArgs e)
        {
            WinFindReplaceDialog.ShowDialog(markEdit.textEditor, 0, this);
        }

        private void btnReplace_Click(object sender, RoutedEventArgs e)
        {
            WinFindReplaceDialog.ShowDialog(markEdit.textEditor, 1, this);
        }

        private void btnUndo_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.Undo();
        }

        private void btnRedo_Click(object sender, RoutedEventArgs e)
        {
            markEdit.textEditor.Redo();

        }






        private void btnHead_Click(object sender, RoutedEventArgs e)
        {
            markEdit.scrViewer.ScrollToHome();
        }

        private void btnEnd_Click(object sender, RoutedEventArgs e)
        {
            markEdit.scrViewer.ScrollToEnd();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.WorkType = WorkType.Edit;
            SetWorkArea();
        }

        private void btnDisplay_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.WorkType = WorkType.Display;
            SetWorkArea();
        }

        private void btnScanling_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.WorkType = WorkType.Both;
            SetWorkArea();
        }

        #endregion

        #region 样式管理
        private void btnAddStyle_Click(object sender, RoutedEventArgs e)
        {
            WinStyleSelected ws = new WinStyleSelected();
            ws.Owner = this;
            ws.ShowDialog();

        }

        private void btnEditStyle_Click(object sender, RoutedEventArgs e)
        {

        }
        private void StyleChange_Click(object sender, RoutedEventArgs e)
        {
            ToggleButton togBtn = (ToggleButton)sender;
            ChangeStyle(togBtn.Header as string);
        }


        #endregion

        #region 导出
        private void btnOutputHtml_Click(object sender, RoutedEventArgs e)
        {
            BLL.FileManager.OutputHTLM(BLL.HtmlHelper.MarkToHTMlPage(markEdit.textEditor.Text));
        }

        private void btnOutputPdf_Click(object sender, RoutedEventArgs e)
        {

            BLL.FileManager.OutputPdf(BLL.HtmlHelper.MarkToHTMlPage(markEdit.textEditor.Text));
        }
        #endregion

        #region 打印
        private void btnPrintDoc_Click(object sender, RoutedEventArgs e)
        {
            markDoc.winWebDoc.ShowPrintDialog();
            //markEdit.Print();
        }

        private void btnPrintPrveDoc_Click(object sender, RoutedEventArgs e)
        {
            markDoc.winWebDoc.ShowPrintPreviewDialog();
        }
        #endregion

        #region 发布

        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPublish_Click(object sender, RoutedEventArgs e)
        {
            WinUploadProcess winProc = new WinUploadProcess();
            winProc.Owner = this;
            winProc.cbxPublish.IsChecked = false;
            Config.CurrBlogsDocument.MarkDownContent = markEdit.textEditor.Text;
            winProc.ShowDialog();
        }

        /// <summary>
        /// 发布为草稿
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPublishDraf_Click(object sender, RoutedEventArgs e)
        {
            WinUploadProcess winProc = new WinUploadProcess();
            winProc.Owner = this;
            winProc.cbxPublish.IsChecked = true;
            Config.CurrBlogsDocument.MarkDownContent = markEdit.textEditor.Text;
            winProc.ShowDialog();

        }



        private void btnBlogsUser_Click(object sender, RoutedEventArgs e)
        {
            var blogsLogin = new WinBlogsLogin();
            blogsLogin.Owner = this;
            blogsLogin.ShowDialog();
        }

        #endregion

        #region  视图
        private void btnFullSrceen_Click(object sender, RoutedEventArgs e)
        {
            ribbonToolbar.IsMinimized = !ribbonToolbar.IsMinimized;
        }

        private void btnTheme2013_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.Theme = ThemeStyle.Office2013;
            ChangeTheme();
        }

        private void btnTheme2010_Click(object sender, RoutedEventArgs e)
        {
            Config.Common.Theme = ThemeStyle.Office2010;
            ChangeTheme();
        }



        #endregion


        #region  BackGround
        private void btnOpNew_Click(object sender, RoutedEventArgs e)
        {
            NewDoc();
        }


        private void labBlogs_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(labBlogs.Text.ToString());
        }
        private void labBogsEmail_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start("mailto:" + labBogsEmail.Text.ToString());
        }
        private void labNetAnalyzer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Process.Start(labNetAnalyzer.Text.ToString());

        }

        private void btnPrintMarkdown_Click(object sender, RoutedEventArgs e)
        {
            this.markEdit.PrintMarkdown();
        }
        private void lstFileList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (lstFileList.SelectedItem == null)
            { return; }

            if (!System.IO.File.Exists(lstFileList.SelectedItem.ToString()))
            {
                Common.ShowMessage("文件不存在！");
                if (Config.Common.FileList.Contains(lstFileList.SelectedItem.ToString()))
                {
                    Config.Common.FileList.Remove(lstFileList.SelectedItem.ToString());
                }

                lstFileList.Items.Remove(lstFileList.SelectedItem);

            }
            else
            {
                OpenDoc(lstFileList.SelectedItem.ToString());
            }

            ribBackStage.IsOpen = false;
        }

        #endregion


        private void RibbonWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (BLL.FileManager.isChangeFlag)
            {
                var result = Common.ShowCorfmMessage("当前内容未保存，是否保存？");
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                else if (result == MessageBoxResult.OK)
                {
                    SaveDoc();
                }
                BLL.FileManager.isChangeFlag = false;
            }
        }

        #endregion

        #region 常规方法


        /// <summary>
        /// 新建文档
        /// </summary>
        public void NewDoc()
        {
            if (BLL.FileManager.isChangeFlag)
            {
                var result = Common.ShowCorfmMessage("当前内容未保存，是否保存？");
                if (result == MessageBoxResult.Cancel)
                    return;
                else if (result == MessageBoxResult.OK)
                {
                    SaveDoc();
                }
                BLL.FileManager.isChangeFlag = false;
            }

            markEdit.textEditor.Clear();
            markDoc.LoadAllHTML(markEdit.textEditor.Text);
            Config.CurrBlogsDocument = new BlogsDocumentInfo();
        }


        /// <summary>
        /// 打开文档
        /// </summary>
        public void OpenDoc()
        {

            if (BLL.FileManager.isChangeFlag)
            {
                var result = Common.ShowCorfmMessage("当前内容未保存，是否保存？");
                if (result == MessageBoxResult.Cancel)
                    return;
                else if (result == MessageBoxResult.OK)
                {
                    SaveDoc();
                }
                BLL.FileManager.isChangeFlag = false;
            }

            var p = BLL.FileManager.OpenMarkdownFile();
            if (p != null)
            {
                markEdit.LoadText(p);
            }
            ChangeTitle();
            markDoc.LoadAllHTML(markEdit.textEditor.Text);
        }

        public void OpenDoc(string filePath)
        {

            if (BLL.FileManager.isChangeFlag)
            {
                var result = Common.ShowCorfmMessage("当前内容未保存，是否保存？");
                if (result == MessageBoxResult.Cancel)
                    return;
                else if (result == MessageBoxResult.OK)
                {
                    SaveDoc();
                }
                BLL.FileManager.isChangeFlag = false;
            }


            var text = System.IO.File.ReadAllText(filePath);
            markEdit.LoadText(text);
            ChangeTitle();
            markDoc.LoadAllHTML(markEdit.textEditor.Text);
            Config.Common.FileList.Remove(filePath);
            Config.Common.FileList.Insert(0, filePath);
            BLL.FileManager.SavePath = filePath;
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        public void SaveDoc()
        {
            bool result = false;
            if (string.IsNullOrEmpty(BLL.FileManager.SavePath))
            {
                result = BLL.FileManager.SaveMarkdownFile(markEdit.textEditor.Text);
            }
            else
            {
                result = BLL.FileManager.SaveMarkdownFileWithoutDialg(markEdit.textEditor.Text);
            }

            if (result)
            {
                Common.ShowStatusMessage("文件保存成功");
            }
            ChangeTitle();
        }
        /// <summary>
        /// 另存为
        /// </summary>
        public void SaveDocAsNew()
        {
            var p = BLL.FileManager.SaveMarkdownFile(markEdit.textEditor.Text);
            if (p)
            {
                Common.ShowStatusMessage("文件保存成功");
            }
            ChangeTitle();
        }



        /// <summary>
        /// 标题更变
        /// </summary>
        private void ChangeTitle()
        {
            this.Title = string.Format("{0}  -  {1}", BLL.FileManager.SavePath, Config.Title);
        }


        #region 界面主题管理

        private void ChangeTheme()
        {
            this.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)(() =>
            {
                var owner = Window.GetWindow(this);

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

                if (owner is RibbonWindow)
                {
                    owner.Style = null;
                    owner.Style = owner.FindResource("RibbonWindowStyle") as Style;
                    owner.Style = null;
                    ++owner.Width;
                    --owner.Width;
                }

            }));
        }

        #endregion 



        #endregion

        #region 快捷键
        private void RibbonWindow_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            //control 组合键
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.N:
                        NewDoc();
                        break;
                    case Key.O:
                        OpenDoc();
                        break;
                    case Key.S:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        {
                            SaveDocAsNew();//另存为
                        }
                        else
                        {
                            SaveDoc();
                        }
                        break;
                    case Key.F:
                        btnFind_Click(null, null);
                        break;
                    case Key.Tab://视图切换
                        switch (Config.Common.WorkType)
                        {
                            case WorkType.Edit: Config.Common.WorkType = WorkType.Display; break;
                            case WorkType.Display: Config.Common.WorkType = WorkType.Both; break;
                            case WorkType.Both: Config.Common.WorkType = WorkType.Edit; break;
                        }
                        SetWorkArea();

                        break;

                    #region markdown快捷键
                    #region 标题
                    case Key.NumPad1:
                    case Key.D1:
                        markEdit.ToggleSymmetricalMarkdownFormatting("# ");
                        break;
                    case Key.NumPad2:
                    case Key.D2:
                        markEdit.ToggleSymmetricalMarkdownFormatting("## ");
                        break;
                    case Key.NumPad3:
                    case Key.D3:
                        markEdit.ToggleSymmetricalMarkdownFormatting("### ");
                        break;
                    case Key.NumPad4:
                    case Key.D4:
                        markEdit.ToggleSymmetricalMarkdownFormatting("#### ");
                        break;
                    case Key.NumPad5:
                    case Key.D5:
                        markEdit.ToggleSymmetricalMarkdownFormatting("##### ");
                        break;
                    case Key.NumPad6:
                    case Key.D6:
                        markEdit.ToggleSymmetricalMarkdownFormatting("###### ");
                        break;
                    #endregion
                    case Key.B:
                        markEdit.ToggleSymmetricalMarkdownFormatting("**");
                        break;
                    case Key.I:
                        markEdit.ToggleSymmetricalMarkdownFormatting("*");
                        break;
                    case Key.H:
                        markEdit.textEditor.SelectedText = "[Title](http://)";
                        break;
                    case Key.J://图片
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            btnImageFromFile_Click(null, null);
                        }
                        else
                        {
                            markEdit.textEditor.SelectedText = "![img](http://)";
                        }
                        break;
                    case Key.T://表格
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            btnMarkConvertToTable_Click(null, null);
                        }
                        else
                        {
                            markEdit.textEditor.SelectedText = "| |\r\n| |";
                        }
                        break;
                    case Key.K://代码
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            markEdit.ToggleSymmetricalMarkdownFormatting("``` \r\n");
                        }
                        else
                        {
                            markEdit.ToggleSymmetricalMarkdownFormatting("` ");
                        }
                        break;
                    case Key.Q:
                        markEdit.ToggleAsymmetricMarkdownFormatting("> ");
                        break;
                    case Key.L:
                        markEdit.textEditor.SelectedText = "---";
                        break;
                    case Key.U:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            markEdit.ToggleAsymmetricMarkdownFormatting("1. ");
                        }
                        else
                        {
                            markEdit.ToggleAsymmetricMarkdownFormatting("+ ");
                        }
                        break;
                    case Key.Y:
                        if ((e.KeyboardDevice.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
                        {
                            markEdit.textEditor.SelectedText = Config.Common.Signatrue;
                        }
                        else
                        {
                            markEdit.textEditor.SelectedText = DateTime.Now.ToLocalTime().ToString();
                        }
                        break;

                    #endregion

                    default:
                        break;
                }
            }
        }













        #endregion

        private void btnGetSource_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Twzy/MarkWord");
        }
    }
}
