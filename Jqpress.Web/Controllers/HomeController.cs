using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Themes;
using Jqpress.Framework.Web;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Configuration;
using Jqpress.Blog.Services;
using Jqpress.Blog.Entity;
using Jqpress.Blog.Entity.Enum;
using Jqpress.Blog.Configuration;
using Jqpress.Web.Models;

namespace Jqpress.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new IndexModel();

            model.SiteName = BlogConfig.GetSetting().SiteName;
            model.ThemeName = "prowerV5";
            model.PageTitle = BlogConfig.GetSetting().SiteName;
            model.SiteUrl = ConfigHelper.SiteUrl;
            model.MetaKeywords = BlogConfig.GetSetting().MetaKeywords;
            model.MetaDescription = BlogConfig.GetSetting().MetaDescription;
            model.SiteDescription = BlogConfig.GetSetting().SiteDescription;
            model.NavLinks = LinkService.GetLinkList((int)LinkPosition.Navigation, 1);
            model.RecentTags = TagService.GetTagList(BlogConfig.GetSetting().SidebarTagCount);
            model.FooterHtml = BlogConfig.GetSetting().FooterHtml;
            model.GeneralLinks = LinkService.GetLinkList((int)LinkPosition.General, 1);

            ThemeService.InitTheme(0);

            const int pageSize = 10;
            int count=0;
            int pageIndex = PressRequest.GetInt("page", 1);
            int cateid= PressRequest.GetQueryInt("cateid",-1);
            int tagid = PressRequest.GetQueryInt("tagid",-1);
            if (cateid > 0)
                pageIndex = pageIndex + 1;
            var postlist = PostService.GetPostPageList(pageSize,pageIndex,out count,cateid,tagid,-1,-1,-1,-1,-1,"","","");
            model.PageList.LoadPagedList(postlist);
            model.PostList = (List<PostInfo>)postlist;
            return View(model);
        }

        public ActionResult Post(int id) 
        {
            var model = new PostModel();

            model.SiteName = BlogConfig.GetSetting().SiteName;
            model.ThemeName = "prowerV5";
            model.SiteUrl = ConfigHelper.SiteUrl;
            model.MetaKeywords = BlogConfig.GetSetting().MetaKeywords;
            model.MetaDescription = BlogConfig.GetSetting().MetaDescription;
            model.SiteDescription = BlogConfig.GetSetting().SiteDescription;
            model.NavLinks = LinkService.GetLinkList((int)LinkPosition.Navigation, 1);
            model.RecentTags = TagService.GetTagList(BlogConfig.GetSetting().SidebarTagCount);
            model.FooterHtml = BlogConfig.GetSetting().FooterHtml;
            model.GeneralLinks = LinkService.GetLinkList((int)LinkPosition.General, 1);

            int postId = id;
            PostInfo post = null;

            string name = Jqpress.Framework.Web.PressRequest.GetQueryString("name");

            if (!Jqpress.Framework.Utils.Validate.IsInt(name))
            {
                post = PostService.GetPost(StringHelper.SqlEncode(name));
            }
            else
            {
                post = PostService.GetPost(id);
            }

            if (post == null)
            {
                return View("404",model);
                //BasePage.ResponseError("文章未找到", "囧！没有找到此文章！", 404);
            }

            if (post.Status == (int)PostStatus.Draft)
            {
               // BasePage.ResponseError("文章未发布", "囧！此文章未发布！");
            }

            string cookie = "isviewpost" + post.PostId;
            int isview = Jqpress.Framework.Utils.TypeConverter.StrToInt(Jqpress.Framework.Web.PressRequest.GetCookie(cookie), 0);
            //未访问或按刷新统计
            if (isview == 0 || BlogConfig.GetSetting().SiteTotalType == 1)
            {
                PostService.UpdatePostViewCount(post.PostId, 1);
            }
            //未访问
            if (isview == 0 && BlogConfig.GetSetting().SiteTotalType == 2)
            {
                Jqpress.Framework.Web.PressRequest.WriteCookie(cookie, "1", 1440);
            }

            model.Post = post;
            model.PageTitle = post.Title;

            string metaKeywords = string.Empty;
            foreach (TagInfo tag in post.Tags)
            {
                metaKeywords += tag.CateName + ",";
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

            int recordCount = 0;
           // model.Comments = CommentService.GetCommentList(BlogConfig.GetSetting().PageSizeCommentCount, Pager.PageIndex, out recordCount, BlogConfig.GetSetting().CommentOrder, -1, post.PostId, 0, -1, -1, null);
            //model.Pager = Pager.CreateHtml(BlogConfig.GetSetting().PageSizeCommentCount, recordCount, post.PageUrl + "#comments");

            //同时判断评论数是否一致
            if (recordCount != post.CommentCount)
            {
                post.CommentCount = recordCount;
                PostService.UpdatePost(post);
            }
            model.IsDefault = 0;
            model.EnableVerifyCode = BlogConfig.GetSetting().EnableVerifyCode;

            return View(model);
        }

        public ActionResult Category(string slug) 
        {
            var model = new PostListModel();

            model.SiteName = BlogConfig.GetSetting().SiteName;
            model.ThemeName = "prowerV5";
            model.PageTitle = BlogConfig.GetSetting().SiteName;
            model.SiteUrl = ConfigHelper.SiteUrl;
            model.MetaKeywords = BlogConfig.GetSetting().MetaKeywords;
            model.MetaDescription = BlogConfig.GetSetting().MetaDescription;
            model.SiteDescription = BlogConfig.GetSetting().SiteDescription;
            model.NavLinks = LinkService.GetLinkList((int)LinkPosition.Navigation, 1);
            model.RecentTags = TagService.GetTagList(BlogConfig.GetSetting().SidebarTagCount);
            model.FooterHtml = BlogConfig.GetSetting().FooterHtml;
            model.GeneralLinks = LinkService.GetLinkList((int)LinkPosition.General, 1);

            CategoryInfo cate = CategoryService.GetCategory(slug);
            if (cate != null)
            {
                int categoryId = cate.CategoryId;
                model.MetaKeywords = cate.CateName;
                model.MetaDescription = cate.Description;
                model.PageTitle = cate.CateName;
                model.PostMessage = string.Format("<h2 class=\"post-message\">分类:{0}</h2>", cate.CateName);
                model.Url = ConfigHelper.SiteUrl + "category/" + Jqpress.Framework.Utils.StringHelper.SqlEncode(slug) + "/page/{0}";

                const int pageSize = 10;
                int count = 0;
                int pageIndex = PressRequest.GetInt("page", 1);
                int cateid = PressRequest.GetQueryInt("cateid", -1);
                int tagid = PressRequest.GetQueryInt("tagid", -1);
                if (cateid > 0)
                    pageIndex = pageIndex + 1;
                var postlist = PostService.GetPostPageList(pageSize, pageIndex, out count, categoryId, tagid, -1, -1, -1, -1, -1, "", "", "");
                model.PageList.LoadPagedList(postlist);
                model.PostList = (List<PostInfo>)postlist;
            }
            model.IsDefault = 0;
            return View("List",model);
        }

        /// <summary>
        /// 标签
        /// </summary>
        public ActionResult Tag(string slug)
        {
            var model = new PostListModel();
            model.SiteName = BlogConfig.GetSetting().SiteName;
            model.ThemeName = "prowerV5";
            model.PageTitle = BlogConfig.GetSetting().SiteName;
            model.SiteUrl = ConfigHelper.SiteUrl;
            model.MetaKeywords = BlogConfig.GetSetting().MetaKeywords;
            model.MetaDescription = BlogConfig.GetSetting().MetaDescription;
            model.SiteDescription = BlogConfig.GetSetting().SiteDescription;
            model.NavLinks = LinkService.GetLinkList((int)LinkPosition.Navigation, 1);
            model.RecentTags = TagService.GetTagList(BlogConfig.GetSetting().SidebarTagCount);
            model.FooterHtml = BlogConfig.GetSetting().FooterHtml;
            model.GeneralLinks = LinkService.GetLinkList((int)LinkPosition.General, 1);
            TagInfo tag = TagService.GetTagBySlug(slug);
            if (tag != null)
            {
                int tagId = tag.TagId;
                model.MetaKeywords = tag.CateName;
                model.MetaDescription = tag.Description;
                model.PageTitle = tag.CateName;
                model.PostMessage = string.Format("<h2 class=\"post-message\">标签:{0}</h2>", tag.CateName);
                model.Url = ConfigHelper.SiteUrl + "tag/" + Jqpress.Framework.Utils.StringHelper.SqlEncode(slug) + "/page/{0}";
                const int pageSize = 10;
                int count = 0;
                int pageIndex = PressRequest.GetInt("page", 1);
                int cateid = PressRequest.GetQueryInt("cateid", -1);
                int tagid = PressRequest.GetQueryInt("tagid", -1);
                if (cateid > 0)
                    pageIndex = pageIndex + 1;
                var postlist = PostService.GetPostPageList(pageSize, pageIndex, out count, cateid, tagId, -1, -1, -1, -1, -1, "", "", "");
                model.PageList.LoadPagedList(postlist);
                model.PostList = (List<PostInfo>)postlist;

            }
            model.IsDefault = 0;
            return View("List",model);
        }
    }
}
