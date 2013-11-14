﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Web;
using Jqpress.Blog.Entity;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class PostListModel:BaseModel
    {
        public PostListModel()
        {
            PostList = new List<PostInfo>();
            PageList = new PostPageList();
        }
        /// <summary>
        /// post list
        /// </summary>
        public List<PostInfo> PostList { get;set;}

        public PostPageList PageList { get; set; }

        /// <summary>
        /// 文章列表信息(作者,分类等)
        /// </summary>
        public string PostMessage { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }
    }
}