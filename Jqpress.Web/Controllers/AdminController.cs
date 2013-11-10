using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;

using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Schema;
using System.IO;
using Jqpress.Blog.Common;
using Jqpress.Blog.Entity;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Configuration;
using Jqpress.Blog.Services;
using Jqpress.Blog.Entity.Enum;
using Jqpress.Blog.Configuration;

using Jqpress.Web.Models.Admin;

namespace Jqpress.Web.Controllers
{
    public partial class AdminController : Controller
    {
        #region 首页
        private int UpfileCount = 0;

        /// <summary>
        /// 控制台首页
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new IndexModel();

            model.PostCount = StatisticsService.GetStatistics().PostCount;
            model.CommentCount = StatisticsService.GetStatistics().CommentCount;
            model.TagCount = StatisticsService.GetStatistics().TagCount;
            model.VisitCount = StatisticsService.GetStatistics().VisitCount;
            model.Commentlist = CommentService.GetCommentList(15, 1, -1, -1, -1, (int)ApprovedStatus.Wait, -1, string.Empty);
            model.DbPath = ConfigHelper.SitePath + ConfigHelper.DbConnection;

            System.IO.FileInfo file = new System.IO.FileInfo(Server.MapPath(ConfigHelper.SitePath + ConfigHelper.DbConnection));
            model.DbSize = GetFileSize(file.Length);

            model.UpfilePath = ConfigHelper.SitePath + "upfiles";

            GetDirectorySize(Server.MapPath(model.UpfilePath));

            model.UpfileSize = GetFileSize(dirSize);

            GetDirectoryCount(Server.MapPath(model.UpfilePath));
            model.UpfileCount = UpfileCount;

            return View(model);
        }

        /// <summary>
        /// 获取rss结果
        /// </summary>
        /// <returns></returns>
        public ActionResult GetRss()
        {
            string tmpl = string.Empty;
            List<FeedInfo> listrss = new List<FeedInfo>();
            try
            {
                string rssurl = "http://www.jqpress.com/feed/post.aspx";
                XmlTextReader reader = new XmlTextReader(rssurl);
                DataSet ds = new DataSet();
                ds.ReadXml(reader);
                if (ds != null && ds.Tables.Count > 0)
                {
                    DataTable dt = ds.Tables[2];

                    foreach (DataRow dr in dt.Rows)
                    {
                        FeedInfo post = new FeedInfo();
                        post.title = dr["title"].ToString();
                        post.description = dr["description"].ToString();
                        post.link = dr["link"].ToString();
                        post.pubDate = dr["pubDate"].ToString();
                        listrss.Add(post);
                    }
                }

            }
            catch (Exception e) { }

            for (int i = 0; i < (listrss.Count > 4 ? 4 : listrss.Count); i++)
            {
                tmpl += "<a title=" + listrss[i].description + " href=" + listrss[i].link + " class=rsswidget>" + listrss[i].title + "</a>";
                tmpl += "<span class=rss-date>" + Jqpress.Framework.Utils.DateTimeHelper.DateToChineseString(Convert.ToDateTime(listrss[i].pubDate)) + "</span>";
                tmpl += "<div class=rssSummary>" + Jqpress.Framework.Utils.StringHelper.CutString(listrss[i].description, 0, 80) + " […]</div>";
            }
            return Content(tmpl);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="size">byte</param>
        /// <returns></returns>
        protected string GetFileSize(long size)
        {
            string FileSize = string.Empty;
            if (size > (1024 * 1024 * 1024))
                FileSize = ((double)size / (1024 * 1024 * 1024)).ToString(".##") + " GB";
            else if (size > (1024 * 1024))
                FileSize = ((double)size / (1024 * 1024)).ToString(".##") + " MB";
            else if (size > 1024)
                FileSize = ((double)size / 1024).ToString(".##") + " KB";
            else if (size == 0)
                FileSize = "0 Byte";
            else
                FileSize = ((double)size / 1).ToString(".##") + " Byte";

            return FileSize;
        }

        /// <summary>
        /// 文件夹大小
        /// </summary>
        public long dirSize = 0;

        /// <summary>
        /// 递归文件夹大小
        /// </summary>
        /// <param name="dirp"></param>
        /// <returns></returns>
        private long GetDirectorySize(string dirp)
        {
            DirectoryInfo mydir = new DirectoryInfo(dirp);
            foreach (FileSystemInfo fsi in mydir.GetFileSystemInfos())
            {
                if (fsi is FileInfo)
                {
                    FileInfo fi = (FileInfo)fsi;
                    dirSize += fi.Length;
                }
                else
                {
                    DirectoryInfo di = (DirectoryInfo)fsi;
                    string new_dir = di.FullName;
                    GetDirectorySize(new_dir);
                }
            }
            return dirSize;
        }

        /// <summary>
        /// 递归文件数量
        /// </summary>
        /// <param name="dirp"></param>
        /// <returns></returns>
        private int GetDirectoryCount(string dirp)
        {
            DirectoryInfo mydir = new DirectoryInfo(dirp);
            foreach (FileSystemInfo fsi in mydir.GetFileSystemInfos())
            {
                if (fsi is FileInfo)
                {
                    //   FileInfo fi = (FileInfo)fsi;
                    UpfileCount += 1;
                }
                else
                {
                    DirectoryInfo di = (DirectoryInfo)fsi;
                    string new_dir = di.FullName;
                    GetDirectoryCount(new_dir);
                }
            }
            return UpfileCount;
        }
        #endregion

        #region 登录
        /// <summary>
        /// 登录
        /// </summary>
        /// <returns></returns>
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginOn()
        {
            if (VerifyLogin())
            {
                return RedirectToAction("index", "admin");
            }
            return View();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        public bool VerifyLogin()
        {
            UserInfo user = null;
            string userName = PressRequest.GetFormString("username");
            string password = Jqpress.Framework.Utils.EncryptHelper.MD5(PressRequest.GetFormString("password"));
            int expires = PressRequest.GetFormString("rememberme") == "forever" ? 43200 : 0;

            user = UserService.GetUser(userName, password);

            if (user != null)
            {
                if (user.Status == 0)
                {
                    ModelState.AddModelError("", "此用户已停用");
                }
                UserService.WriteUserCookie(user.UserId, user.UserName, user.Password, expires);
                return true;
            }
            else
            {
                ModelState.AddModelError("", "用户名或密码错误!");
            }
            return false;
        }
        #endregion

        #region 文章
        public ActionResult PostList() 
        {
            var model = new PostListModel();

            string keyword = StringHelper.SqlEncode(PressRequest.GetQueryString("keyword"));
            int categoryId = PressRequest.GetQueryInt("categoryid", -1);
            int userId = PressRequest.GetQueryInt("userid", -1);
            int recommend = PressRequest.GetQueryInt("recommend", -1);
            int hide = PressRequest.GetQueryInt("hide", -1);

            int pageindex = 0;
            const int pagesize = 20;
            pageindex = PressRequest.GetInt("page", 1);
            ViewBag.Page = pageindex;
            int count = 0;

            // txtKeyword.Text = keyword; 暂时注释
           // ddlCategory.SelectedValue = categoryId.ToString();
            //ddlAuthor.SelectedValue = userId.ToString();
            //  chkRecommend.Checked = recommend == 1 ? true : false; 暂时注释
            //chkHideStatus.Checked = hide == 1 ? true : false; 暂时注释

            int totalRecord = 0;

           // List<PostInfo> list = PostService.GetPostList(Pager1.PageSize, Pager1.PageIndex, out totalRecord, categoryId, -1, userId, recommend, -1, -1, hide, string.Empty, string.Empty, keyword);

            return View(model);
        }
        #endregion

        #region 用户
        #endregion

        #region 链接
        #endregion

        #region 设置
        #endregion

        #region 工具
        #endregion
    }
}
