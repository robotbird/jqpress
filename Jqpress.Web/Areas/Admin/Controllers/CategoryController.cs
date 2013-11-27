using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Blog.Services;
using Jqpress.Web.Areas.Admin.Models;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List() 
        {
            var model = new CateListModel();
            model.CateList = CategoryService.GetCategoryList(); 
            return View(model);
        }

    }
}
