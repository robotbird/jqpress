using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Themes;
using Jqpress.Framework.Web;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Configuration;
using Jqpress.Core.Services;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Configuration;
using Jqpress.Web.Models;

namespace Jqpress.Web.Controllers
{
    public class HomeController : Controller
    {
        #region private items
        private PostService _postService = new PostService();
        private CategoryService _categoryService = new CategoryService();
        private LinkService _linkService = new LinkService();
        private TagService _tagService = new TagService();
        private ThemeService _themeService = new ThemeService();
        private UserService _userService = new UserService();
        #endregion;

        public HomeController() 
        {
 
            var theme = PressRequest.GetQueryString("theme");
            _themeService.InitTheme(theme);
        }
        public ActionResult Index(string pagename)
        {
            var config = SiteConfig.GetSetting();
            if (!config.IsInstalled)
            {
               return RedirectToAction("Index", "Install");
            }

            var model = new IndexModel();
            if (string.IsNullOrEmpty(pagename))
            {
                model.MetaTitle = config.SiteName;
                model.MetaKeywords = config.MetaKeywords;
                model.MetaDescription = config.MetaDescription;
            }
            

            const int pageSize = 10;
            int count = 0;
            int pageIndex = PressRequest.GetInt("page", 1);
            var cateid = PressRequest.GetQueryString("cateid");//todo:获取分类的名称或者编号
            int tagid = PressRequest.GetQueryInt("tagid", -1);
            //if (cateid > 0)
            //    pageIndex = pageIndex + 1;
            var postlist = _postService.GetPostPageList(pageSize, pageIndex, out count, cateid, tagid, -1, -1, -1, -1, -1, "", "", "");
            model.PageList.LoadPagedList(postlist);
            model.PostList = (List<PostInfo>)postlist;
            return View(model);
            
        }

        public ActionResult Post(int id) 
        {
            var model = new PostModel();
            PostInfo post = null;

            string name = PressRequest.GetQueryString("name");

            if (!Validate.IsInt(name))
            {
                post = _postService.GetPostByPageName(StringHelper.SqlEncode(name));
            }
            else
            {
                post = _postService.GetPost(id);
            }

            if (post == null)
            {
                return View("404",model);
            }

            string cookie = "isviewpost" + post.PostId;
            int isview = TypeConverter.StrToInt(PressRequest.GetCookie(cookie), 0);
            //未访问或按刷新统计
            if (isview == 0 || SiteConfig.GetSetting().SiteTotalType == 1)
            {
                _postService.UpdatePostViewCount(post.PostId, 1);
            }
            //未访问
            if (isview == 0 && SiteConfig.GetSetting().SiteTotalType == 2)
            {
                PressRequest.WriteCookie(cookie, "1", 1440);
            }

            model.Post = post;
            model.MetaTitle = post.Title;

            string metaKeywords = string.Empty;


            if (post.Tag != null)
            {
                string temptags = post.Tag.Replace("{", "").Replace("}", ",");
                if (temptags.Length > 0)
                {
                    temptags = temptags.TrimEnd(',');
                }
                model.Tags = _tagService.GetTagList(temptags);

                _postService.GetRelatedPosts(post);
            }


            if (model.Tags != null)
            {
                metaKeywords = model.Tags.Aggregate(metaKeywords, (current, tag) => current + (tag.CateName + ","));
            }

 
            if (metaKeywords.Length > 0)
            {
                metaKeywords = metaKeywords.TrimEnd(',');
            }
            model.MetaKeywords = metaKeywords;

            string metaDescription = post.Summary;
            if (string.IsNullOrEmpty(post.Summary))
            {
                metaDescription = post.PostContent;
            }
            model.MetaDescription = StringHelper.CutString(StringHelper.RemoveHtml(metaDescription), 50).Replace("\n", "");

            
            //model.RelatedPosts =

            int recordCount = 0;
           // model.Comments = CommentService.GetCommentList(SiteConfig.GetSetting().PageSizeCommentCount, Pager.PageIndex, out recordCount, SiteConfig.GetSetting().CommentOrder, -1, post.PostId, 0, -1, -1, null);
            //model.Pager = Pager.CreateHtml(SiteConfig.GetSetting().PageSizeCommentCount, recordCount, post.PageUrl + "#comments");

            //同时判断评论数是否一致
            if (recordCount != post.CommentCount)
            {
                post.CommentCount = recordCount;
                _postService.UpdatePost(post);
            }
            model.IsDefault = 0;
            model.EnableVerifyCode = SiteConfig.GetSetting().EnableVerifyCode;

            return View(model);
        }

        public ActionResult Category(string slug) 
        {
            var model = new PostListModel();
            CategoryInfo cate = _categoryService.GetCategory(slug);
            model.Category = cate;
            if (cate != null)
            {
                int categoryId = cate.CategoryId;
                model.MetaKeywords = cate.CateName;
                model.MetaDescription = cate.Description;
                ViewBag.Title = cate.CateName;
                model.Url = ConfigHelper.SiteUrl + "category/" + StringHelper.SqlEncode(slug) + "/page/{0}";

                const int pageSize = 10;
                int count = 0;
                int pageIndex = PressRequest.GetInt("page", 1);
                int cateid = PressRequest.GetQueryInt("cateid", -1);
                int tagid = PressRequest.GetQueryInt("tagid", -1);
                if (cateid > 0)
                    pageIndex = pageIndex + 1;
                var cateids =categoryId+","+ _categoryService.GetCategoryList().FindAll(c => c.ParentId == categoryId).Aggregate(string.Empty, (current, t) => current + (t.CategoryId + ",")).TrimEnd(',');
                var postlist = _postService.GetPostPageList(pageSize, pageIndex, out count, cateids.TrimEnd(','), tagid, -1, -1, -1, -1, -1, "", "", "");
                model.PageList.LoadPagedList(postlist);
                model.PostList = (List<PostInfo>)postlist;
            }
            model.IsDefault = 0;
            

            return View(model.Category.ViewName,model);
        }
        /// <summary>
        /// 单页面
        /// </summary>
        /// <param name="pagename"></param>
        /// <returns></returns>
        public ActionResult Page(string pagename) 
        {
            var model = new PageModel();
            model.category = _categoryService.GetCategory(pagename);
            ViewBag.Title = model.category.CateName;
            ViewBag.PageName = pagename;
            return View("Page",model);
        }

        /// <summary>
        /// 标签
        /// </summary>
        public ActionResult Tag(string pagename)
        {
            var model = new PostListModel();
            model.SiteName = SiteConfig.GetSetting().SiteName;
            model.ThemeName = "printV1";
            model.MetaTitle = SiteConfig.GetSetting().SiteName;
            model.SiteUrl = ConfigHelper.SiteUrl;
            model.MetaKeywords = SiteConfig.GetSetting().MetaKeywords;
            model.MetaDescription = SiteConfig.GetSetting().MetaDescription;
            model.SiteDescription = SiteConfig.GetSetting().SiteDescription;
            model.FooterHtml = SiteConfig.GetSetting().FooterHtml;
            TagInfo tag = _tagService.GetTagByPageName(pagename);
            if (tag != null)
            {
                int tagId = tag.TagId;
                model.MetaKeywords = tag.CateName;
                model.MetaDescription = tag.Description;
                model.MetaTitle = tag.CateName;
                model.PostMessage = string.Format("<h2 class=\"post-message\">标签:{0}</h2>", tag.CateName);
                model.Url = ConfigHelper.SiteUrl + "tag/" + Jqpress.Framework.Utils.StringHelper.SqlEncode(pagename) + "/page/{0}";
                const int pageSize = 10;
                int count = 0;
                int pageIndex = PressRequest.GetInt("page", 1);
                int cateid = PressRequest.GetQueryInt("cateid", -1);
                int tagid = PressRequest.GetQueryInt("tagid", -1);
                if (cateid > 0)
                    pageIndex = pageIndex + 1;
                var postlist = _postService.GetPostPageList(pageSize, pageIndex, out count, cateid.ToString(), tagId, -1, -1, -1, -1, -1, "", "", "");
                model.PageList.LoadPagedList(postlist);
                model.PostList = (List<PostInfo>)postlist;

            }
            model.IsDefault = 0;
            return View("List",model);
        }

        /// <summary>
        /// 作者
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public ActionResult Author(string username)
        {
            var model = new PostListModel();
            model.SiteName = SiteConfig.GetSetting().SiteName;
            model.MetaTitle = SiteConfig.GetSetting().SiteName;
            model.SiteUrl = ConfigHelper.SiteUrl;
            model.MetaKeywords = SiteConfig.GetSetting().MetaKeywords;
            model.MetaDescription = SiteConfig.GetSetting().MetaDescription;
            model.SiteDescription = SiteConfig.GetSetting().SiteDescription;
            model.FooterHtml = SiteConfig.GetSetting().FooterHtml;
            UserInfo user = _userService.GetUser(username);
            if (user != null)
            {
                int userId = user.UserId;
                model.MetaKeywords = user.NickName;
                model.MetaDescription = user.Description;
                model.MetaTitle = user.NickName;
                model.PostMessage = string.Format("<h2 class=\"post-message\">标签:{0}</h2>", user.NickName);
                model.Url = ConfigHelper.SiteUrl + "author/" + Jqpress.Framework.Utils.StringHelper.SqlEncode(username) + "/page/{0}";
                const int pageSize = 10;
                int count = 0;
                int pageIndex = PressRequest.GetInt("page", 1);
                int cateid = PressRequest.GetQueryInt("cateid", -1);
                if (cateid > 0)
                    pageIndex = pageIndex + 1;
                var postlist = _postService.GetPostPageList(pageSize, pageIndex, out count, cateid.ToString(), 0, userId, -1, -1, -1, -1, "", "", "");
                model.PageList.LoadPagedList(postlist);
                model.PostList = (List<PostInfo>)postlist;

            }
            model.IsDefault = 0;
            return View("List", model);
        }

        #region 公共部分
        /// <summary>
        /// 头部
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Header() 
        {
            var model = new HeadModel();
            model.NavLinks = _linkService.GetLinkList((int)LinkPosition.Navigation, 1);
            return PartialView(model);
        }
        /// <summary>
        /// 侧栏
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult SiderBar() 
        {
            return PartialView();
        }
        /// <summary>
        /// 底部
        /// </summary>
        /// <returns></returns>
        [ChildActionOnly]
        public ActionResult Footer()
        {
            var model = new FootModel();
            model.RecentTags = _tagService.GetTagList(SiteConfig.GetSetting().SidebarTagCount);
            model.GeneralLinks = _linkService.GetLinkList((int)LinkPosition.General, 1);
            return PartialView(model);
        }
        #endregion
    }
}
