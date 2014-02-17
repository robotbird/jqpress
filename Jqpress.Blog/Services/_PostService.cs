using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Web;
using System.Text.RegularExpressions;
using Jqpress.Framework.Mvc;
using Jqpress.Framework.Configuration;
using Jqpress.Blog.Data;
using Jqpress.Blog.Entity;
using Jqpress.Blog.Configuration;
using Jqpress.Blog.Repository;

namespace Jqpress.Blog.Services
{
    /// <summary>
    /// 博客文章管理
    /// </summary>
    public class _PostService
    {
        #region 私有变量
        private IPostRepository _postRepository;
        #endregion

        #region 构造函数
        /// <summary>
        /// 构造器方法
        /// </summary>
        public _PostService()
            : this(new PostRepository())
        {
        }
        /// <summary>
        /// 构造器方法
        /// </summary>
        /// <param name="postRepository"></param>
        public _PostService(IPostRepository postRepository)
        {
            this._postRepository = postRepository;
        }
        #endregion

        #region 方法
        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="post"></param>
        public void Update(PostInfo post)
        {
            _postRepository.Update(post);
        }
        #endregion
    }
}
