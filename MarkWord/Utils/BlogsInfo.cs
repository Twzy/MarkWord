using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkWord
{
    /// <summary>
    /// 当前博客信息
    /// </summary>
    [Serializable]
    public class BlogsInfo
    {
        internal string BlogsID { get; set; }

        internal string BlogsMetaweblogUrl { get; set; }

        internal string BlogsUserId { get; set; }

        internal string BlogsPassword { get; set; }

        internal bool BlogsRecodPassword { get; set; }

        internal string BlogsUrl { get; set; }

        internal string BlogsName { get; set; }

        internal string BlogsModel { get; set; }
    }
}
