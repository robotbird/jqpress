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
    //TODO:列表分页
    public class TagController : Controller
    {
        public ActionResult List()
        {
            var model = new TagListModel();
            var catelist = TagService.GetTagList();
            const int pageSize = 10;
            int count = 0;
            int pageIndex = PressRequest.GetInt("page", 1);
            List<TagInfo> list = TagService.GetTagList(Pager1.PageSize, Pager1.PageIndex, out recordCount);
            model.TagList = catelist;
            return View(model);
        }

        /// <summary>
        /// get category by id
        /// </summary>
        public JsonResult Edit(int? id)
        {
            var cid = id ?? 0;
            TagInfo cat = TagService.GetTag(cid);
            return Json(cat, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// delete categroy by id
        /// </summary>
        public ContentResult Delete(int? id)
        {
            var cid = id ?? 0;
            TagService.DeleteTag(cid);
            return Content("success");
        }
        /// <summary>
        /// insert or update category
        /// </summary>
        /// <param name="cat"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Save"), ValidateInput(false)]
        public JsonResult Save(TagInfo cat)
        {

            if (string.IsNullOrEmpty(cat.Slug))
            {
                cat.Slug = cat.CateName;
            }

            cat.Slug = HttpHelper.HtmlEncode(StringHelper.FilterSlug(cat.Slug, "tag"));
            cat.SortNum = TypeConverter.StrToInt(cat.SortNum, 1000);
            if (cat.TagId > 0)
            {
                var oldcat = TagService.GetTag(cat.TagId);
                cat.CreateTime = oldcat.CreateTime;
                cat.PostCount = oldcat.PostCount;
                TagService.UpdateTag(cat);
            }
            else
            {
                cat.CreateTime = DateTime.Now;
                cat.PostCount = 0;
                TagService.InsertTag(cat);
            }
            return Json(cat, JsonRequestBehavior.AllowGet);
        }

    }
}
