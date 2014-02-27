using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Blog.Domain
{
    /// <summary>
    /// 文件及文件夹信息
    /// </summary>
    public class FoldInfo
    {
        /// <summary>
        /// 文件夹路径
        /// </summary>
        public string FoldPath { get; set; }
        /// <summary>
        /// 文件夹名称
        /// </summary>
        public string FoldName { get; set; }
        /// <summary>
        /// 文件夹中包含的文件 
        /// </summary>
        public int FileSystemInfosLength { get; set; }

    }
}
