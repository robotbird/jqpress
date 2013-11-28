using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Web;
using Jqpress.Blog.Services;
using Jqpress.Blog.Entity;
using Jqpress.Web.Areas.Admin.Models;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    public class CategoryController : Controller
    {

        public ActionResult List() 
        {
            var model = new CateListModel();

            int cateid = PressRequest.GetQueryInt("cateid", -1);

            var catelist = CategoryService.GetCategoryTreeList();
            model.CateList = catelist;
            catelist.Add(new CategoryInfo() { CateName = "全部", CategoryId = -1 });

            model.CateSelectItem = catelist.ConvertAll(c => new SelectListItem { Text = c.CateName, Value = c.CategoryId.ToString(), Selected = c.CategoryId == cateid });

            return View(model);
        }

        /// <summary>
        /// ajax获取json值
        /// </summary>
        public JsonResult Edit(int? id)
        {
            var cid = id??0;
            CategoryInfo term = CategoryService.GetCategory(cid);
            return Json(term, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// ajax删除操作
        /// </summary>
        public ContentResult Delete(int? id)
        {
            var cid = id ?? 0;
            CategoryService.DeleteCategory(cid);
            return Content("success");
        }
        /// <summary>
        /// insert or update category
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
        public ContentResult Save(CategoryInfo cat)
        {
             return Content("");
        }

    }
}
