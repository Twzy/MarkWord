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
    public class AutoCompletion : ICompletionData
    {
        public AutoCompletion(string text, string value, string desc)
        {
            this.Text = text;
            this.Value = value;
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
        /// 显示的内容
        /// </summary>
        public string Text { get; private set; }

        public string Value { get; private set; }


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

            //处理鼠标位置
            // string pos = "{$pos}"; //
            var tagValue = ReplaceFlag(this.Value);//先处理其他内容
            //int offset = tagValue.IndexOf(pos); //+ completionSegment.Offset;
            //tagValue = tagValue.Replace(pos, "");

            textArea.Document.Remove(completionSegment.Offset - 1, 1);//移除输入的字符
            textArea.Document.Replace(completionSegment, tagValue);//插入信息
            //textArea.Document.Replace(offset, 1, "x",OffsetChangeMappingType.CharacterReplace);//插入信息

        }
        /// <summary>
        /// 处理代替指标
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string ReplaceFlag(string value)
        {
            //处理鼠标位置
            string pos = "{$pos}";
            value = value.Replace(pos, "");

            //替换为时间
            string time = "{$time}";
            value = value.Replace(time, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd HH:mm:ss"));

            //替换为时间
            string date = "{$date}";
            value = value.Replace(date, DateTime.Now.ToLocalTime().ToString("yyyy-MM-dd"));

            return value;

        }
    }
}
