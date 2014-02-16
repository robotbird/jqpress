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

namespace Jqpress.Blog.Services
{
    /// <summary>
    /// 博客文章管理
    /// </summary>
    public class PostService
    {
        /// <summary>
        /// 列表
        /// </summary>
        private static List<PostInfo> _posts;
        /// <summary>
        /// 列表统计数量
        /// </summary>
        private static int _postcount;

        /// <summary>
        /// lock
        /// </summary>
        private static object lockHelper = new object();

        static PostService()
        {
            LoadPost();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void LoadPost()
        {
            if (_posts == null)
            {
                lock (lockHelper)
                {
                    if (_posts == null)
                    {
                        _posts = DatabaseProvider.Instance.GetPostList();
                    }
                }
            }
        }


        /// <summary>
        /// 添加文章
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public static int InsertPost(PostInfo post)
        {
            post.PostId = DatabaseProvider.Instance.InsertPost(post);

            _posts.Add(post);
            _posts.Sort();

            //统计
            StatisticsService.UpdateStatisticsPostCount(1);
            //用户
            UserService.UpdateUserPostCount(post.UserId, 1);
            //分类
            CategoryService.UpdateCategoryCount(post.CategoryId, 1);
            //标签
            TagService.UpdateTagUseCount(post.Tag, 1);

            //   RemovePostsCache();
            SendEmail(post);
            return post.PostId;
        }

        /// <summary>
        /// 修改文章
        /// </summary>
        /// <param name="_postinfo"></param>
        /// <returns></returns>
        public static int UpdatePost(PostInfo _postinfo)
        {
            //   PostInfo oldPost = GetPost(_postinfo.PostId);   //好像有问题,不能缓存

            PostInfo oldPost = GetPostByDatabase(_postinfo.PostId);

            int result = DatabaseProvider.Instance.UpdatePost(_postinfo);

            if (oldPost != null && oldPost.CategoryId != _postinfo.CategoryId)
            {
                //分类
                CategoryService.UpdateCategoryCount(oldPost.CategoryId, -1);
                CategoryService.UpdateCategoryCount(_postinfo.CategoryId, 1);
            }

            //     CacheHelper.Remove(CacheKey);

            //标签
            TagService.UpdateTagUseCount(oldPost.Tag, -1);
            TagService.UpdateTagUseCount(_postinfo.Tag, 1);

            //   RemovePostsCache();

            return result;
        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="postid"></param>
        /// <returns></returns>
        public static int DeletePost(int postid)
        {
            PostInfo oldPost = GetPost(postid);

            _posts.Remove(oldPost);

            int result = DatabaseProvider.Instance.DeletePost(postid);

            //统计
            StatisticsService.UpdateStatisticsPostCount(-1);
            //用户
            UserService.UpdateUserPostCount(oldPost.UserId, -1);
            //分类
            CategoryService.UpdateCategoryCount(oldPost.CategoryId, -1);
            //标签
            TagService.UpdateTagUseCount(oldPost.Tag, -1);

            //删除所有评论
            CommentService.DeleteCommentByPost(postid);

            //     RemovePostsCache();

            return result;
        }

        /// <summary>
        /// 根据Id获取文章
        /// </summary>
        /// <param name="postid"></param>
        /// <returns></returns>
        public static PostInfo GetPost(int postid)
        {
            //PostInfo p = DatabaseProvider.Instance.GetPost(postid);
            ////  BuildPost(p);
            //return p;
            PostInfo p = DatabaseProvider.Instance.GetPost(postid);
            return p;

            foreach (PostInfo post in _posts)
            {
                if (post.PostId == postid)
                {
                    return post;
                }
            }
            return null;
        }

        /// <summary>
        /// 从数据库获取文章
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public static PostInfo GetPostByDatabase(int postId)
        {
            return DatabaseProvider.Instance.GetPost(postId);
        }

        /// <summary>
        /// 根据别名获取文章
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        public static PostInfo GetPost(string slug)
        {
            foreach (PostInfo post in _posts)
            {
                if (!string.IsNullOrEmpty(slug) && post.Slug.ToLower() == slug.ToLower())
                {
                    return post;
                }
            }
            return null;
        }

        ///// <summary>
        ///// 获取文章列表
        ///// </summary>
        ///// <param name="pageSize"></param>
        ///// <param name="pageIndex"></param>
        ///// <param name="recordCount"></param>
        ///// <returns></returns>
        //public static List<PostInfo> GetPostList(int pageSize, int pageIndex, out int recordCount)
        //{
        //    return GetPostList(pageSize, pageIndex, out recordCount,-1, -1, -1,-1, -1, -1, -1, null, null, null);
        //}


        /// <summary>
        /// 获取全部文章,是缓存的
        /// </summary>
        /// <returns></returns>
        public static List<PostInfo> GetPostList()
        {
            return DatabaseProvider.Instance.GetPostList();
        }

        /// <summary>
        /// 获取文章数
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="tagId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int GetPostCount(int categoryId, int tagId, int userId)
        {
            int recordCount = 0;
            GetPostList(1, 1, out recordCount, categoryId, tagId, userId, -1, -1, -1, -1, string.Empty, string.Empty, string.Empty);

            return recordCount;
        }

        /// <summary>
        /// 获取文章数
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="tagId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="PostStatus"></param>
        /// <returns></returns>
        public static int GetPostCount(int categoryId, int tagId, int userId,int status,int PostStatus)
        {
            int recordCount = 0;
            GetPostList(1, 1, out recordCount, categoryId, tagId, userId, -1, status, -1, PostStatus,  string.Empty, string.Empty, string.Empty);

            return recordCount;
        }

        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="topstatus"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static List<PostInfo> GetPostList(int pageSize, int pageIndex, out int recordCount, int categoryId, int tagId, int userId, int recommend, int status, int topstatus, int PostStatus, string begindate, string enddate, string keyword)
        {
            List<PostInfo> list;
            try {
                if (pageIndex == 1 && tagId <= 0 && string.IsNullOrEmpty(begindate)&& string.IsNullOrEmpty(enddate) && string.IsNullOrEmpty(keyword))
                {
                    list = GetPostList(pageSize, categoryId, userId, recommend, status, topstatus, PostStatus);
                    recordCount = _postcount;
                }
                else
                {
                    list = DatabaseProvider.Instance.GetPostList(pageSize, pageIndex, out recordCount, categoryId, tagId, userId, recommend, status, topstatus, PostStatus, begindate, enddate, keyword);
                }

                return list;
            }catch(Exception e){
                throw e;
            }

        }


        /// <summary>
        /// 获取文章列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="categoryId"></param>
        /// <param name="userId"></param>
        /// <param name="status"></param>
        /// <param name="topstatus"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public static IPagedList<PostInfo> GetPostPageList(int pageSize, int pageIndex, out int recordCount, int categoryId, int tagId, int userId, int recommend, int status, int topstatus, int PostStatus, string begindate, string enddate, string keyword)
        {
            List<PostInfo> list;
            try
            {
                if (pageIndex == 1 && tagId <= 0 && string.IsNullOrEmpty(begindate) && string.IsNullOrEmpty(enddate) && string.IsNullOrEmpty(keyword))
                {
                    list = GetPostList(pageSize, categoryId, userId, recommend, status, topstatus, PostStatus);
                    recordCount = _postcount;
                }
                else
                {
                    list = DatabaseProvider.Instance.GetPostList(pageSize, pageIndex, out recordCount, categoryId, tagId, userId, recommend, status, topstatus, PostStatus, begindate, enddate, keyword);
                }

                return new PagedList<PostInfo>(list, pageIndex - 1, pageSize, recordCount);
            }
            catch (Exception e)
            {
                throw e;
            }

        }


        public static List<PostInfo> GetPostList(int rowCount, int categoryId, int userId, int recommend, int status, int topstatus, int PostStatus)
        {
            try{
                     List<PostInfo> list = GetPostList();
                    if (categoryId != -1)
                    {
                        list = list.FindAll(post => post.CategoryId == categoryId);
                    }

                    if (userId != -1)
                    {
                        list = list.FindAll(post => post.UserId == userId);
                    }
                    if (recommend != -1)
                    {
                        list = list.FindAll(post => post.Recommend == recommend);
                    }
                    if (status != -1)
                    {
                        list = list.FindAll(post => post.Status == status);
                    }
                    if (topstatus != -1)
                    {
                        list = list.FindAll(post => post.TopStatus == topstatus);
                    }
                    if (PostStatus != -1)
                    {
                        list = list.FindAll(post => post.PostStatus == PostStatus);
                    }
                    _postcount = list.Count;
                    if (rowCount > list.Count)
                    {
                        return list;
                    }
                    List<PostInfo> list2 = new List<PostInfo>();
                    for (int i = 0; i < rowCount; i++)
                    {
                        list2.Add(list[i]);
                    }
                    return list2;
            
            }catch(Exception e){
                throw e;
            
            }
       
        }


        /// <summary>
        /// 更新点击数
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public static int UpdatePostViewCount(int postId, int addCount)
        {
            //   CacheHelper.Remove(CacheKey);

            PostInfo post = GetPost(postId);

            if (post != null)
            {
                post.ViewCount += addCount;
            }
            return DatabaseProvider.Instance.UpdatePostViewCount(postId, addCount);
        }

        /// <summary>
        /// 更新评论数
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public static int UpdatePostCommentCount(int postId, int addCount)
        {
            PostInfo post = GetPost(postId);

            if (post != null)
            {
                post.CommentCount += addCount;

                return DatabaseProvider.Instance.UpdatePost(post);
            }
            return 0;

        }
        /// <summary>
        /// 保存远程图片
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static string SaveRemoteImage(string html)
        {
            string Reg = @"<img.*src=.?(http|https).+>";
            string currentHost = HttpContext.Current.Request.Url.Host;
            List<Uri> urlList = new List<Uri>();

            //获取图片URL地址
            foreach (Match m in Regex.Matches(html, Reg, RegexOptions.IgnoreCase | RegexOptions.Compiled))
            {
                Regex reg = new Regex(@"src=('|"")?(http|https).+?('|""|>| )+?", RegexOptions.IgnoreCase);
                string imgUrl = reg.Match(m.Value).Value.Replace("src=", "").Replace("'", "").Replace("\"", "").Replace(@">", "");
                Uri u = new Uri(imgUrl);
                if (u.Host != currentHost)
                {
                    urlList.Add(u);
                }
            }

            //去掉重复
            List<Uri> urlList2 = new List<Uri>();
            foreach (Uri u2 in urlList)
            {
                if (!urlList2.Contains(u2))
                {
                    urlList2.Add(u2);
                }
            }

            //保存
            WebClient wc = new WebClient();
            int i = 0;
            foreach (Uri u2 in urlList2)
            {
                i++;
                string extName = ".jpg";
                if (System.IO.Path.HasExtension(u2.AbsoluteUri))
                {
                    extName = System.IO.Path.GetExtension(u2.AbsoluteUri);
                    if (extName.IndexOf('?') >= 0)
                    {
                        extName = extName.Substring(0, extName.IndexOf('?'));
                    }
                }

                string path = ConfigHelper.SitePath + "upfiles/" + DateTime.Now.ToString("yyyyMM") + "/";


                if (!System.IO.Directory.Exists(HttpContext.Current.Server.MapPath(path)))
                {
                    System.IO.Directory.CreateDirectory(HttpContext.Current.Server.MapPath(path));
                }
                //  Response.Write(newDir);

                string newFileName = path + "auto_" + DateTime.Now.ToString("ddHHmmss") + i + extName;

                wc.DownloadFile(u2, HttpContext.Current.Server.MapPath(newFileName)); //非图片后缀要改成图片后缀
                //  Response.Write(u2.AbsoluteUri + "||<br>");

                //是否合法
                if (IsImage(HttpContext.Current.Server.MapPath(newFileName)))
                {
                    html = html.Replace(u2.AbsoluteUri, newFileName);
                }
                else
                {
                    System.IO.File.Delete(HttpContext.Current.Server.MapPath(newFileName));
                }
            }
            return html;
        }

        /// <summary>
        /// 检查是否为允许的图片格式
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsImage(string filePath)
        {
            bool ret = false;

            System.IO.FileStream fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            System.IO.BinaryReader r = new System.IO.BinaryReader(fs);
            string fileclass = "";
            byte buffer;
            try
            {
                buffer = r.ReadByte();
                fileclass = buffer.ToString();
                buffer = r.ReadByte();
                fileclass += buffer.ToString();
            }
            catch
            {
                return false;
            }
            r.Close();
            fs.Close();
            // String[] fileType = { "255216", "7173", "6677", "13780", "8297", "5549", "870", "87111", "8075" };
            string[] fileType = { "255216", "7173", "6677", "13780" };

            for (int i = 0; i < fileType.Length; i++)
            {
                if (fileclass == fileType[i])
                {
                    ret = true;
                    break;
                }
            }
            return ret;
        }

        /// <summary>
        /// 发邮件
        /// </summary>
        /// <param name="post"></param>
        public static void SendEmail(PostInfo post)
        {
            if (BlogConfig.GetSetting().SendMailAuthorByPost == 1)
            {
                List<UserInfo> list = UserService.GetUserList();
                List<string> emailList = new List<string>();

                foreach (UserInfo user in list)
                {
                    if (!Jqpress.Framework.Utils.Validate.IsEmail(user.Email))
                    {
                        continue;
                    }
                    //自己不用发
                    //if (CurrentUser.Email == user.Email)
                    //{
                    //    continue;
                    //}
                    ////不重复发送
                    //if (emailList.Contains(user.Email))
                    //{
                    //    continue;
                    //}
                    //emailList.Add(user.Email);

                    //string subject = string.Empty;
                    //string body = string.Empty;

                    //subject = string.Format("[新文章通知]{0}", post.Title);
                    //body += string.Format("{0}有新文章了:<br/>", BlogConfig.GetSetting().SiteName);
                    //body += "<hr/>";
                    //body += "<br />标题: " + post.Link;
                    //body += post.Detail;
                    //body += "<hr/>";
                    //body += "<br />作者: " + CurrentUser.Link;
                    //body += "<br />时间: " + post.PostTime;
                    //body += "<br />文章连接: " + post.Link;
                    //body += "<br />注:系统自动通知邮件,不要回复。";

                    // EmailHelper.SendAsync(user.Email, subject, body);
                }
            }
        }
    }
}
