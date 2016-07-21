using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarkWord
{
    /// <summary>
    /// 工作类型
    /// </summary>
    public enum WorkType : byte
    {
        /// <summary>
        /// 编辑模式
        /// </summary>
        Edit = 0,
        /// <summary>
        /// 浏览模式
        /// </summary>
        Display = 1,

        /// <summary>
        /// 同步预览模式
        /// </summary>
        Both = 2


    }
}
