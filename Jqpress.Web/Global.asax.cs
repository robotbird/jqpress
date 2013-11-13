﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Jqpress.Framework.Themes;

namespace Jqpress.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            #region 忽略静态文件路由
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.IgnoreRoute("{*jpg}", new { jpg = @".*\.jpg(/.*)?" });
            routes.IgnoreRoute("{*gif}", new { gif = @".*\.gif(/.*)?" });
            routes.IgnoreRoute("{*js}", new { js = @".*\.js(/.*)?" });
            routes.IgnoreRoute("{*css}", new { css = @".*\.css(/.*)?" });
            routes.IgnoreRoute("{*html}", new { css = @".*\.html(/.*)?" });
            routes.IgnoreRoute("{*htm}", new { css = @".*\.htm(/.*)?" });
            routes.IgnoreRoute("{*shtml}", new { css = @".*\.shtml(/.*)?" });
            routes.IgnoreRoute("{*htc}", new { css = @".*\.htc(/.*)?" });//处理css样式文件
            #endregion

            routes.MapRoute(
              name: "Post",
              url: "post/{id}",
              defaults: new { controller = "Home", action = "Post", id = UrlParameter.Optional },
              namespaces: new[] { "Jqpress.Web.Controllers" }
             );

            routes.MapRoute(
             name: "List",
             url: "tag/{id}",
             defaults: new { controller = "Home", action = "Post", id = UrlParameter.Optional },
             namespaces: new[] { "Jqpress.Web.Controllers" }
            );

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new[] { "Jqpress.Web.Controllers" }
            );
           

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            ViewEngines.Engines.Add(new ThemeableRazorViewEngine());

        }
    }
}