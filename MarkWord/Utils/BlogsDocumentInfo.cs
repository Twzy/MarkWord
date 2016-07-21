using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkWord
{
    /// <summary>
    /// 当前操作的标题信息
    /// </summary>
    [Serializable]
    public class BlogsDocumentInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public string Title { get; set; }

        public string MarkDownContent { get; set; }
        //主要用于更新
        public string PostId { get; set; }
    }
}
