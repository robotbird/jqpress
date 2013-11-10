using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Themes;
using Jqpress.Blog.Services;

namespace Jqpress.Web.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Welcome to ASP.NET MVC!";
            ThemeService.InitTheme(0);
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
