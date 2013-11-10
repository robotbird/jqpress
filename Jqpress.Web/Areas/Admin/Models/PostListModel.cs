using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using System.Web;
using Jqpress.Blog.Entity;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class PostListModel
    {
        /// <summary>
        /// post list
        /// </summary>
        public List<PostInfo> PostList { get;set;}
    }
}