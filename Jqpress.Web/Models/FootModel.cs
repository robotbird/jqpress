using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;


namespace Jqpress.Web.Models
{
    public class FootModel:BaseModel
    {

        public FootModel() 
        {
            RecentTags = new List<TagInfo>();
            GeneralLinks = new List<LinkInfo>();
        }
        /// <summary>
        /// 标签
        /// </summary>
        public List<TagInfo> RecentTags { get; set; }

        /// <summary>
        /// 普通链接
        /// </summary>
        public List<LinkInfo> GeneralLinks { get; set; }
    }
}