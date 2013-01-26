﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jqpress.Blog.Entity
{
    /// <summary>
    /// 主题实体
    /// </summary>
    public class ThemeInfo
    {
        /// <summary>
        /// 主题名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 对应程序版本
        /// </summary>
        public string Version { get; set; }

        /// <summary> 
        /// 作者
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// 作者Email
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 作者网站
        /// </summary>
        public string SiteUrl { get; set; }

        /// <summary>
        /// 发布日期
        /// </summary>
        public string PubDate { get; set; }
    }
}
