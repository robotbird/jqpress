using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Themes;
using Jqpress.Framework.Web;
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
            model.MetaKeywords = BlogConfig.GetSetting().MetaKeywords;
            model.MetaDescription = BlogConfig.GetSetting().MetaDescription;
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

        public ActionResult About()
        {
            return View();
        }
    }
}
