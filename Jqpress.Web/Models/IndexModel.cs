using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Blog.Entity;

namespace Jqpress.Web.Models
{
    public class IndexModel:BaseModel
    {
        public List<PostInfo> PostList { get; set; }

        public PostPageList PageList { get; set; }

        public IndexModel() 
        {
            PageList = new PostPageList();
            PostList = new List<PostInfo>();
        }
    }
}