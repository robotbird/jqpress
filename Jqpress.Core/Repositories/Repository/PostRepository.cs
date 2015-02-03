﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Framework.DbProvider;
using Jqpress.Framework.Configuration;
using Jqpress.Core.Domain;
using Jqpress.Core.Repositories.IRepository;

namespace Jqpress.Core.Repositories.Repository
{
    public partial class PostRepository:IPostRepository
    {
        
        
        /// <summary>
        /// 新增文章
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public virtual int Insert(PostInfo post) 
        {
            CheckPageName(post);
            string cmdText = string.Format(@"insert into [{0}posts]
                                (
                               [CategoryId],[TitlePic],[Title],[Summary],[PostContent],[PageName],[UserId],[CommentStatus],[CommentCount],[ViewCount],[Tag],[UrlFormat],[Template],[Recommend],[Status],[TopStatus],[HomeStatus],[PostTime],[UpdateTime]
                                )
                                values
                                (
                                @CategoryId,@TitlePic,@Title,@Summary,@PostContent,@PageName,@UserId,@CommentStatus,@CommentCount,@ViewCount,@Tag,@UrlFormat,@Template,@Recommend,@Status,@TopStatus,@HomeStatus,@PostTime,@UpdateTime
                                )", ConfigHelper.Tableprefix);

            using(var conn = new DapperHelper().OpenConnection())
            {
                conn.Execute(cmdText, new
                {
                    CategoryId = post.CategoryId,
                    TitlePic = post.TitlePic,
                    Title = post.Title,
                    Summary = post.Summary,
                    PostContent = post.PostContent,
                    PageName = post.PageName,
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
                    UpdateTime = post.UpdateTime.ToString()
                });
                return conn.Query<int>(string.Format("select top 1 [PostId] from [{0}Posts] order by [PostId] desc", ConfigHelper.Tableprefix),null).First();
            }
        }
        /// <summary>
        /// 更新文章
        /// </summary>
        /// <param name="post"></param>
        public virtual int Update(PostInfo post)
        {
            string cmdText = string.Format(@"update [{0}posts] set  
                                       [CategoryId]=@CategoryId,
                                       [TitlePic]=@TitlePic,
                                       [Title]=@Title,
                                       [Summary]=@Summary,
                                       [PostContent]=@PostContent,
                                       [PageName]=@PageName,
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


            using (var conn = new DapperHelper().OpenConnection())
            {
               return conn.Execute(cmdText, new
                {
                    CategoryId = post.CategoryId,
                    TitlePic = post.TitlePic,
                    Title = post.Title,
                    Summary = post.Summary,
                    PostContent = post.PostContent,
                    PageName = post.PageName,
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
        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="post"></param>
        public virtual int Delete(PostInfo post) 
        {
            PostInfo oldPost = GetById(post.PostId);
            if (oldPost == null) throw new Exception("文章不存在");

            string cmdText = string.Format("delete from [{0}posts] where [PostId] = @PostId", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
               return conn.Execute(cmdText, new {PostId = post.PostId });
            }
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual PostInfo GetById(object id) 
        {
            string cmdText = string.Format("select top 1 p.PostId,u.UserId,u.UserName,c.CategoryId,c.CateName from [{0}posts] p,[{0}Users] u,[{0}category] c where p.UserId=u.UserId and p.CategoryId=c.CategoryId", ConfigHelper.Tableprefix);
            using (var conn = new DapperHelper().OpenConnection())
            {
               // var list = conn.Query<PostInfo>(cmdText, new { PostId = (int)id });
               // return list.Any() ? list.ToList()[0] : null;
                var list = conn.Query<PostInfo, UserInfo, CategoryInfo, PostInfo>(cmdText, (post, user, cate) =>
                {
                    post.Author = user;
                    post.Category = cate;
                    return post;
                },  splitOn: "UserId,CategoryId");
                return list.Any() ? list.ToList()[0] : null;
            }



        }
        /// <summary>
        /// 获取所有文章
        /// </summary>
        public virtual IEnumerable<PostInfo> Table
        {
            get
            {
                string cmdText = string.Format("select * from [{0}posts] order by [postid] desc", ConfigHelper.Tableprefix);
                using (var conn = new DapperHelper().OpenConnection())
                {
                    var list = conn.Query<PostInfo>(cmdText, null);
                    return list;
                }
            }
        }

        /// <summary>
        /// 根据pagename获取实体
        /// </summary>
        /// <param name="pagename"></param>
        /// <returns></returns>
        public virtual PostInfo GetPostbyPageName(string pagename)
        {
            string cmdText = string.Format("select top 1 * from [{0}posts] where [pagename] = @_pagename", ConfigHelper.Tableprefix);

            using (var conn = new DapperHelper().OpenConnection())
            {
                var list = conn.Query<PostInfo>(cmdText, new { _pagename = pagename });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="categoryId"></param>
        /// <param name="tagId"></param>
        /// <param name="userId"></param>
        /// <param name="recommend"></param>
        /// <param name="status"></param>
        /// <param name="topstatus"></param>
        /// <param name="PostStatus"></param>
        /// <param name="begindate"></param>
        /// <param name="enddate"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public virtual List<PostInfo> GetPostList(int pageSize, int pageIndex, out int recordCount, string categoryId, int tagId, int userId, int recommend, int status, int topstatus, int PostStatus, string begindate, string enddate, string keyword)
        {
            string condition = " 1=1 ";

            if (categoryId != ""&&categoryId!="-1")
            {
                condition += " and categoryId in(" + categoryId+")";
            }
            if (tagId != -1)
            {
                condition += " and tag like '%{" + tagId + "}%'";
            }
            if (userId != -1)
            {
                condition += " and userid=" + userId;
            }
            if (recommend != -1)
            {
                condition += " and recommend=" + recommend;
            }
            if (status != -1)
            {
                condition += " and status=" + status;
            }

            if (topstatus != -1)
            {
                condition += " and topstatus=" + topstatus;
            }

            if (PostStatus != -1)
            {
                condition += " and PostStatus=" + PostStatus;
            }

            if (!string.IsNullOrEmpty(begindate))
            {
                condition += " and PostTime>=#" + begindate + "#";
            }
            if (!string.IsNullOrEmpty(enddate))
            {
                condition += " and PostTime<#" + enddate + "#";
            }

            if (!string.IsNullOrEmpty(keyword))
            {
                condition += string.Format(" and (summary like '%{0}%' or title like '%{0}%'  )", keyword);
            }

            using (var conn = new DapperHelper().OpenConnection())
            {
                string cmdTotalRecord = "select count(1) from [" + ConfigHelper.Tableprefix + "posts] where " + condition;
                recordCount = conn.Query<int>(cmdTotalRecord,null).First();
                string cmdText = new DapperHelper().GetPageSql("[" + ConfigHelper.Tableprefix + "Posts]", "[PostId]", "*", pageSize, pageIndex, 1, condition);

                var list = conn.Query<PostInfo>(cmdText, null);
                return list.ToList();
            }
        }
        /// <summary>
        /// 获取相关文章
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        public virtual List<PostInfo> GetPostListByRelated(int postId, int rowCount)
        {
            string tags;

            PostInfo post = GetById(postId);

            if (post != null && post.Tag.Length > 0)
            {
                tags = post.Tag;


                tags = tags.Replace("}", "},");
                string[] idList = tags.Split(',');

                string where = idList.Where(tagId => !string.IsNullOrEmpty(tagId)).Aggregate(" (", (current, tagId) => current + string.Format("  [tags] like '%{0}%' or ", tagId));
                where += " 1=2 ) and [status]=1 and [postid]<>" + postId;

                string cmdText = string.Format("select top {0} * from [{2}posts] where {1} order by [postid] desc", rowCount, where, ConfigHelper.Tableprefix);

                using (var conn = new DapperHelper().OpenConnection())
                {
                    var list = conn.Query<PostInfo>(cmdText, null);
                    return list.ToList();
                }
            }
            return new List<PostInfo>();
        }

        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <returns></returns>
        public virtual void CheckPageName(PostInfo post)
        {
            if (string.IsNullOrEmpty(post.PageName))
            {
                return;
            }
            while (true)
            {
                string cmdText = post.PostId == 0 ? string.Format("select count(1) from [{1}posts] where [pagename]='{0}'  ", post.PageName, ConfigHelper.Tableprefix) : string.Format("select count(1) from [{2}posts] where [pagename]='{0}'   and [postid]<>{1}", post.PageName, post.PostId, ConfigHelper.Tableprefix);

                using (var conn = new DapperHelper().OpenConnection())
                {
                    int r = conn.Query<int>(cmdText,null).First();

                    if (r == 0)
                    {
                        return;
                    }
                }
                post.PageName += "-2";
            }
        }
        /// <summary>
        /// 获取文章归档统计
        /// </summary>
        /// <returns></returns>
        public virtual List<ArchiveInfo> GetArchive()
        {
            string cmdText = string.Format("select format(PostTime, 'yyyymm') as [date] ,  count(*) as [count] from [{0}posts] where [status]=1 and [PostStatus]=0  group by  format(PostTime, 'yyyymm')  order by format(PostTime, 'yyyymm') desc", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
              return  conn.Query<ArchiveInfo>(cmdText,null).ToList();
            }
        }
        /// <summary>
        /// 更新访问量
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public virtual int UpdatePostViewCount(int postId, int addCount)
        {
            string cmdText = string.Format("update [{0}posts] set [viewcount] = [viewcount] + @addcount where [postid]=@postid", ConfigHelper.Tableprefix);
            using(var conn = new DapperHelper().OpenConnection())
            {
                return conn.Execute(cmdText, new { addcount =addCount,postid = postId });
            }
        }

    }
}
