﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Configuration;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Utils;

namespace Jqpress.Blog.Entity
{
    /// <summary>
    /// 分类实体
    /// RSSURL
    /// </summary>
    public class CategoryInfo : IComparable<CategoryInfo>
    {
        public int CompareTo(CategoryInfo other)
        {
            if (this.SortNum != other.SortNum)
            {
                return this.SortNum.CompareTo(other.SortNum);
            }
            return this.CategoryId.CompareTo(other.CategoryId);
        }
 
        #region 非字段



        /// <summary>
        /// 地址
        /// </summary>
        public string Url
        {
            //set { _url = value; }
            //get { return _url; }
            get
            {
                string url = string.Format("{0}category/{1}{2}", ConfigHelper.SiteUrl, this.CategoryId, BlogConfig.GetSetting().RewriteExtension);
                if (!string.IsNullOrEmpty(this.Slug))
                {
                    url = string.Format("{0}category/{1}{2}", ConfigHelper.SiteUrl, Jqpress.Framework.Web.HttpHelper.UrlEncode(this.Slug), BlogConfig.GetSetting().RewriteExtension);
                }
                return url;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        public string Link
        {
            get
            {
                return string.Format("<a href=\"{0}\" title=\"分类:{1}\">{2}</a>", this.Url, this.CateName, this.CateName);
            }
        }


        /// <summary>
        /// 订阅URL
        /// </summary>
        public string FeedUrl
        {
            get
            {
                return string.Format("{0}feed/post/categoryid/{1}{2}", ConfigHelper.SiteUrl, this.CategoryId, BlogConfig.GetSetting().RewriteExtension);
            }
        }
        /// <summary>
        /// 订阅连接
        /// </summary>
        public string FeedLink
        {
            get
            {
                return string.Format("<a href=\"{0}\" title=\"订阅:{1}\">订阅</a>", this.FeedUrl, this.CateName);
            }
        }
        /// <summary>
        /// 当前分类的深度
        /// </summary>
        public int Depth { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Path { get; set; }
        /// <summary>
        /// 树结构
        /// </summary>
        public string TreeChar { get; set; }
        #endregion


        /// <summary>
        /// ID
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// 父类id
        /// </summary>
        public int ParentId { get; set; }
      

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CateName { get; set; }
        /// <summary>
        /// 别名 一般字母类型，简短url
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int SortNum { get; set; }

        /// <summary>
        /// 文章数量
        /// </summary>
        public int PostCount { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
