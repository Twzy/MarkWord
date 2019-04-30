using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Media.Imaging;

namespace MarkWord
{
    public class MsCompletion : ICompletionData
    {
        public MsCompletion(string text, string desc)
        {
            this.Text = text;
            this.Description = desc;
        }

        public System.Windows.Media.ImageSource Image
        {
            get
            {
                return new BitmapImage(new Uri("pack://application:,,,/MarkWord;Component/Images/T_SelectParent_Sm_N.png", UriKind.Absolute));
            }
        }

        /// <summary>
        /// 函数名称
        /// </summary>
        public string Text { get; private set; }


        // Use this property if you want to show a fancy UIElement in the drop down list.
        public object Content
        {
            get { return this.Text; }
        }

        public object Description
        {
            get; private set;
        }

        public double Priority { get { return 0; } }

        public void Complete(TextArea textArea, ISegment completionSegment, EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }
    }
}
