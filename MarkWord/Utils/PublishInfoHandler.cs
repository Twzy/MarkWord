using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkWord
{
    /// <summary>
    /// 用于展示发布进度
    /// </summary>
    /// <param name="maxcount"></param>
    /// <param name="value"></param>
    /// <param name="msg"></param>
    public delegate void PublishInfoHandler(int maxcount, int value, string msg);
}
