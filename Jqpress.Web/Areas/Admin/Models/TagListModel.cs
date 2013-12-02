using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Blog.Entity;

namespace Jqpress.Web.Areas.Admin.Models
{
    public class TagListModel
    {
        public TagListModel() 
        {
            TagList = new List<TagInfo>();
        }
        
        public List<TagInfo> TagList { get; set; }
    }
}