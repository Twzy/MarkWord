using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Search;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// MarkEditer.xaml 的交互逻辑
    /// </summary>
    public partial class MarkEditer : UserControl
    {
        public MarkEditer()
        {
            InitializeComponent();


            textEditor.TextArea.TextEntering += TextArea_TextEntering;
            textEditor.TextArea.TextEntered+= TextArea_TextEntered;
            loadAutoLst();
        }

        public WebDoc MarkDoc { get; set; }


        public void RefashBody()
        {
            if (MarkDoc == null)
                return;
            MarkDoc.LoadBody(this.textEditor.Text);
        }

        #region 自动提示

        List<AutoItem> itemList_all = new List<AutoItem>();
        List<AutoItem> itemList_title = new List<AutoItem>();
        List<AutoItem> itemList_Custom = new List<AutoItem>();

        public void loadAutoLst()
        {
            itemList_all.AddRange(new AutoItem[]
            {
                new AutoItem{Name="# 标题1",  Value = "# ", Descript = "标题1"},
                new AutoItem{Name="## 标题2",  Value = "## ", Descript = "标题2"},
                new AutoItem{Name="### 标题3",  Value = "### ", Descript = "标题3"},
                new AutoItem{Name="#### 标题4",  Value = "#### ", Descript = "标题4"},
                new AutoItem{Name="##### 标题5",  Value = "##### ", Descript = "标题5"},
                new AutoItem{Name="###### 标题6",  Value = "###### ", Descript = "标题6"},
                new AutoItem{Name="**粗体** ",  Value = "**{$pos}**", Descript = "粗体"},
                new AutoItem{Name="*斜体*",  Value = "*{$pos}*", Descript = "斜体"},
                new AutoItem{Name="[Title](http://{$pos}) 超链接",  Value = "[Title](http://)", Descript = "超链接"},
                new AutoItem{Name="![img](http://{$pos}) 图片",  Value = "![img](http://)", Descript = "图片"},
                new AutoItem{Name="|表格|",  Value = "|{$pos}| |\r\n| | |", Descript = "表格"},
                new AutoItem{Name="`` 行内代码",  Value = "`{$pos}`", Descript = "行内代码"},
                new AutoItem{Name="``` ``` 代码块",  Value = "```\r\n{$pos}\r\n``` ", Descript = "代码块"},
                new AutoItem{Name="> 引用",  Value = "> ", Descript = "引用"},
                new AutoItem{Name="[toc] 目录",  Value = "[toc]", Descript = "目录"},
                new AutoItem{Name="[toc -1] 目录 跳过1个",  Value = "[toc -1]", Descript = "目录 跳过前面1个标题"},
                new AutoItem{Name="[toc -2] 目录 跳过2个",  Value = "[toc -2]", Descript = "目录 跳过前面2个标题"},
                new AutoItem{Name="--- 横线",  Value = "---", Descript = "横线"},
                new AutoItem{Name = "@time", Value="{$time}",Descript = "时间"},
                new AutoItem{Name = "@date", Value="{$date}",Descript = "时间"},
                });
        }



        CompletionWindow completionWindow;
        private void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
        {
            if (e.Text.Length > 0 && completionWindow != null && !char.IsLetterOrDigit(e.Text[0]))
            {
                completionWindow.CompletionList.RequestInsertion(e);
            }
        }

        public void AutoComplete(List<AutoItem> autoList)
        {
            completionWindow = new CompletionWindow(textEditor.TextArea);

            IList<ICompletionData> data = completionWindow.CompletionList.CompletionData;
            foreach (var f in autoList)
            {
                data.Add(new AutoCompletion(f.Name, f.Value, f.Descript));
            }


            completionWindow.Show();
            completionWindow.Closed += delegate
            {
                completionWindow = null;
            };
        }

        private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
        {
            if (e.Text == "#" ||
                e.Text == "*" ||
                e.Text == "[" ||
                e.Text == "!" ||
                e.Text == "`" ||
                e.Text == "-"||
                e.Text == "@")
            {
                var tmpList = new List<AutoItem>();
                foreach (var i in itemList_all)
                {
                    if (i.Name.StartsWith(e.Text))
                    {
                        tmpList.Add(i);
                    }
                }

                AutoComplete(tmpList);
            }
            //else if (e.Text == "@")
            //{
            //    AutoComplete(itemList_Custom);
            //}
        }

        //执行全部提示
        private void TxtCode_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyboardDevice.Modifiers & ModifierKeys.Control) == ModifierKeys.Control &&
                (e.KeyboardDevice.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift &&
                e.Key == Key.Space)
            {
                AutoComplete(itemList_all);
            }
        }
        #endregion





        #region Mark标记
        /// <summary>
        /// 两边追加标志
        /// </summary>
        /// <param name="syntax"></param>
        public void ToggleSymmetricalMarkdownFormatting(string syntax)
        {
            int selectionLength = this.textEditor.SelectionLength;
            int selectionStart = this.textEditor.SelectionStart;
            if (selectionLength == 0 && selectionStart + syntax.Length <= this.textEditor.Text.Length)
            {
                string text = this.textEditor.Document.GetText(selectionStart, syntax.Length);
                if (text == syntax)
                {
                    this.textEditor.SelectionStart += syntax.Length;
                    return;
                }
            }
            char[] array = syntax.ToCharArray();
            Array.Reverse(array);
            string text2 = new string(array);
            int num = this.textEditor.SelectionLength;
            int num2 = this.textEditor.SelectionStart;
            if (num2 >= syntax.Length)
            {
                num2 -= syntax.Length;
                num += syntax.Length;
            }
            DocumentLine lineByOffset = this.textEditor.Document.GetLineByOffset(this.textEditor.CaretOffset);
            if (num2 + num + syntax.Length <= lineByOffset.EndOffset)
            {
                num += syntax.Length;
            }
            string text3 = "";
            if (num > 0)
            {
                text3 = this.textEditor.Document.GetText(num2, num);
            }
            Match match = Regex.Match(text3, string.Concat(new string[]
          {
             "^",
             Regex.Escape(syntax),
             "(.*)",
             Regex.Escape(text2),
             "$"
          }), RegexOptions.Singleline);
            bool success = match.Success;
            if (success)
            {
                text3 = match.Groups[1].Value;
                this.textEditor.SelectionStart = num2;
                this.textEditor.SelectionLength = num;
                this.textEditor.SelectedText = text3;
                return;
            }
            text3 = syntax + this.textEditor.SelectedText + text2;
            this.textEditor.SelectedText = text3;
            this.textEditor.SelectionLength -= syntax.Length * 2;
            this.textEditor.SelectionStart += syntax.Length;
        }

        /// <summary>
        /// 左边添加前缀
        /// </summary>
        /// <param name="markdownSyntaxToApply"></param>
        public void ToggleAsymmetricMarkdownFormatting(string markdownSyntaxToApply)
        {
            bool flag = this.textEditor.SelectedText == this.textEditor.Document.GetText(this.CurrentLine);
            bool flag2 = this.textEditor.SelectedText.Contains(Environment.NewLine);
            bool flag3 = this.textEditor.CaretOffset == this.CurrentLine.Offset;
            if ((!flag3 || !flag) && !flag2)
            {
                this.textEditor.SelectedText = Environment.NewLine + this.textEditor.SelectedText;
                this.textEditor.SelectionLength -= Environment.NewLine.Length;
                this.textEditor.SelectionStart += Environment.NewLine.Length;
            }
            if (this.textEditor.SelectionLength > 0)
            {
                string selectedText = this.textEditor.SelectedText;
                string selectedText2;
                if (selectedText.Contains(markdownSyntaxToApply))
                {
                    selectedText2 = selectedText.Replace(markdownSyntaxToApply, "");
                }
                else
                {
                    selectedText2 = markdownSyntaxToApply + selectedText.Replace(Environment.NewLine, Environment.NewLine + markdownSyntaxToApply);
                }
                this.textEditor.SelectedText = selectedText2;
                return;
            }
            DocumentLine lineByOffset = this.textEditor.Document.GetLineByOffset(this.textEditor.CaretOffset);
            string text = string.Empty;
            if (!(lineByOffset.Length == 0))
            {
                text = Environment.NewLine;
            }
            this.textEditor.SelectedText = text + markdownSyntaxToApply;
            this.textEditor.SelectionLength = 0;
            this.textEditor.SelectionStart += markdownSyntaxToApply.Length + text.Length;
        }
        public DocumentLine CurrentLine
        {
            get
            {
                return this.textEditor.Document.GetLineByOffset(this.textEditor.CaretOffset);
            }
        }





        #endregion

        #region 事件

        bool isLoadFlag = false;
        public void LoadText(string markStr)
        {
            isLoadFlag = true;
            this.textEditor.Text = markStr;
            isLoadFlag = false;
        }

        /// <summary>
        /// 文本更变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            if (!isLoadFlag)
            {
                if (this.textEditor.Text != "" && scrViewer != null)
                    if (scrViewer.ScrollableHeight == scrViewer.VerticalOffset)
                        scrViewer.ScrollToBottom();

                BLL.FileManager.isChangeFlag = true;
            }
            //加载文档
            if (MarkDoc == null)
                return;
            if (Config.Common.WorkType == WorkType.Both)
            {
                MarkDoc.LoadBody(this.textEditor.Text);
            }
        }

        #endregion

        #region 打印
        public void PrintMarkdown()
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() != true)
            {
                return;
            }
            FlowDocument flowDocument = this.CreateFlowDocument(this.textEditor);
            flowDocument.PageHeight = printDialog.PrintableAreaHeight;
            flowDocument.PageWidth = printDialog.PrintableAreaWidth;
            flowDocument.PagePadding = new Thickness(72.0);
            flowDocument.ColumnGap = 0.0;
            flowDocument.ColumnWidth = flowDocument.PageWidth - flowDocument.ColumnGap - flowDocument.PagePadding.Left - flowDocument.PagePadding.Right;
            IDocumentPaginatorSource documentPaginatorSource = flowDocument;
            try
            {
                printDialog.PrintDocument(documentPaginatorSource.DocumentPaginator, "MarkWord");
            }
            catch (System.Exception exception)
            {
                Common.ShowMessage(exception.Message);
            }
        }
        private FlowDocument CreateFlowDocument(TextEditor editor)
        {
            IHighlighter highlighter = editor.TextArea.GetService(typeof(IHighlighter)) as IHighlighter;
            return new FlowDocument(this.ConvertTextDocumentToBlock(editor.Document, highlighter))
            {
                FontFamily = editor.FontFamily,
                FontSize = editor.FontSize
            };
        }

        private Block ConvertTextDocumentToBlock(TextDocument document, IHighlighter highlighter)
        {
            if (document == null)
            {
                throw new System.ArgumentNullException("document");
            }
            Paragraph paragraph = new Paragraph();
            foreach (DocumentLine current in document.Lines)
            {
                int lineNumber = current.LineNumber;
                HighlightedInlineBuilder highlightedInlineBuilder = new HighlightedInlineBuilder(document.GetText(current));
                if (highlighter != null)
                {
                    HighlightedLine highlightedLine = highlighter.HighlightLine(lineNumber);
                    int offset = current.Offset;
                    foreach (HighlightedSection current2 in highlightedLine.Sections)
                    {
                        highlightedInlineBuilder.SetHighlighting(current2.Offset - offset, current2.Length, current2.Color);
                    }
                }
                paragraph.Inlines.AddRange(highlightedInlineBuilder.CreateRuns());
                paragraph.Inlines.Add(new LineBreak());
            }
            return paragraph;
        }
        #endregion

        #region 脚本
        public double ScrollViewerPositionPercentage
        {
            get
            {
                double num = this.scrViewer.ExtentHeight - this.scrViewer.ViewportHeight;
                double result;
                if (num != 0.0)
                {
                    result = this.scrViewer.VerticalOffset / num;
                }
                else
                {
                    result = 0.0;
                }
                return result;
            }
        }



        private void scrViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            if (MarkDoc == null) return;
            if (Config.Common.WorkType == WorkType.Both)
            {
                MarkDoc.ScrollAuto(this.ScrollViewerPositionPercentage);
            }
        }
        #endregion

        #region 查找



        #endregion

    }
}
