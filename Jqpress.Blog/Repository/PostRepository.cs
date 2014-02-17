using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Entity;
using Jqpress.Framework.DbProvider;
using Jqpress.Framework.Configuration;


namespace Jqpress.Blog.Repository
{
    public partial class PostRepository:IPostRepository
    {
        DapperHelper dapper = new DapperHelper();
        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="post"></param>
        public virtual void Update(PostInfo post)
        {
            string cmdText = string.Format(@"update [{0}posts] set  
                                       [CategoryId]=@CategoryId,
                                       [Title]=@Title,
                                       [Summary]=@Summary,
                                       [PostContent]=@PostContent,
                                       [Slug]=@Slug,
                                       [UserId]=@UserId,
                                       [CommentStatus]=@CommentStatus,
                                       [CommentCount]=@CommentCount,
                                       [ViewCount]=@ViewCount,
                                       [Tag]=@Tag,
                                       [UrlFormat]=@UrlFormat,
                                       [Template]=@Template,
                                       [Recommend]=@Recommend,
                                       [Status]=@Status,
                                       [TopStatus]=@TopStatus,
                                       [HomeStatus]=@HomeStatus,
                                       [PostTime]=@PostTime,
                                       [UpdateTime]=@UpdateTime
                                   where [PostId]=@PostId", ConfigHelper.Tableprefix);


            using (var conn = dapper.OpenConnection())
            {
                conn.Execute(cmdText, new
                {
                    CategoryId = post.CategoryId,
                    Title = post.Title,
                    Summary = post.Summary,
                    PostContent = post.PostContent,
                    Slug = post.Slug,
                    UserId = post.UserId,
                    CommentStatus = post.CommentStatus,
                    CommentCount = post.CommentCount,
                    ViewCount = post.ViewCount,
                    Tag = post.Tag,
                    UrlFormat = post.UrlFormat,
                    Template = post.Template,
                    Recommend = post.Recommend,
                    Status = post.Status,
                    TopStatus = post.TopStatus,
                    HomeStatus = post.HomeStatus,
                    PostTime = post.PostTime.ToString(),
                    UpdateTime = post.UpdateTime.ToString(),
                    PostId = post.PostId
                });
            }
        }

        public virtual void Insert(PostInfo post) 
        {
        
        }

        public virtual void Delete(PostInfo post) 
        {
        }

        public virtual PostInfo GetById(object id) 
        {
            return null;
        }

        public virtual IQueryable<PostInfo> Table
        {
            get { return null; }
        }
    }
}
