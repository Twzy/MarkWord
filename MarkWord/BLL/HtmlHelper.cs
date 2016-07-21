using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkWord.BLL
{
    public class HtmlHelper
    {
        public static string MarkToHTMlPage(string MarkValue)
        {
            #region 数据初始化
            StringBuilder sbr = new StringBuilder();
            sbr.AppendLine("<!--墨云软件--> <!doctype html><html><head><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /><title></title>");
            sbr.AppendLine("<style type='text/css'>");
            #region 样式
            sbr.AppendFormat(@"

{0}

", string.IsNullOrEmpty(Config.Style) ? Properties.Resources.DefaultStyle : Config.Style);
            #endregion 
            sbr.AppendLine("</style>");
            sbr.AppendLine("<script type='text/javascript'>");
            sbr.AppendLine("function updatePageContent(msg){ document.body.innerHTML= msg;} ");
            sbr.AppendLine("function scrollToPageContent(value){  window.scrollTo(0, value * (document.body.scrollHeight - document.body.clientHeight)); } ");
            sbr.AppendLine("</script>");
            sbr.AppendFormat(@"</head><body>
{0}
 </body></html>", CommonMark.CommonMarkConverter.Convert(MarkValue));
            #endregion

            return sbr.ToString();

        }

    }
}
