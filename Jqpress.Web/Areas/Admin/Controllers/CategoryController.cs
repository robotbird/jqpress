using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Web;
using Jqpress.Framework.Utils;
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
            catelist.Add(new CategoryInfo() { CateName = "作为一级分类", CategoryId = -1 });

            model.CateSelectItem = catelist.ConvertAll(c => new SelectListItem { Text = c.TreeChar+c.CateName, Value = c.CategoryId.ToString(), Selected = c.CategoryId == cateid });

            return View(model);
        }

        /// <summary>
        /// get category by id
        /// </summary>
        public JsonResult Edit(int? id)
        {
            var cid = id??0;
            CategoryInfo cat = CategoryService.GetCategory(cid);
            return Json(cat, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// delete categroy by id
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
        [HttpPost, ActionName("Save"), ValidateInput(false)]        
        public JsonResult Save(CategoryInfo cat)
        {

            if (string.IsNullOrEmpty(cat.Slug))
            {
                cat.Slug = cat.CateName;
            }

            cat.Slug = HttpHelper.HtmlEncode(StringHelper.FilterSlug(cat.Slug, "cate"));
            cat.SortNum = TypeConverter.StrToInt(cat.SortNum, 1000);
            if (cat.CategoryId > 0)
            {
                var  oldcat = CategoryService.GetCategory(cat.CategoryId);
                cat.CreateTime = oldcat.CreateTime;
                cat.PostCount = oldcat.PostCount;
                CategoryService.UpdateCategory(cat);
            }
            else 
            {
                cat.CreateTime = DateTime.Now;
                cat.PostCount = 0;
                CategoryService.InsertCategory(cat);
            }
            cat.TreeChar = CategoryService.GetCategoryTreeList().Find(c => c.CategoryId == cat.CategoryId).TreeChar;

            return Json(cat, JsonRequestBehavior.AllowGet);
        }

    }
}
