using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Jqpress.Core.Domain;

namespace Jqpress.Web.Models
{
    public class PostModel:BaseModel
    {
        public PostModel()
        {
            Tags = new List<TagInfo>();
            Next = new PostInfo();
            Previous = new PostInfo();
            RelatedPosts = new List<PostInfo>();
        }

        /// <summary>
        /// 是否开启验证码
        /// </summary>
        public int EnableVerifyCode { get; set; }
        /// <summary>
        /// 文章
        /// </summary>
        public PostInfo Post { get; set; }
        /// <summary>
        /// 评论列表
        /// </summary>
        public List<CommentInfo> Comments { get; set; }
        /// <summary>
        /// 分页字符串
        /// </summary>
        public string Pager { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 对应标签
        /// </summary>
        public List<TagInfo> Tags { get; set; }


        /// <summary>
        /// 下一篇文章
        /// </summary>
        public PostInfo Next { get; set; }


        /// <summary>
        /// 上一篇文章
        /// </summary>
        public PostInfo Previous { get; set; }


        /// <summary>
        /// 相关文章
        /// </summary>
        public List<PostInfo> RelatedPosts { get; set; }


    }
}