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
    public class CommentController : Controller
    {
        //
        // GET: /Admin/Comment/

        public ActionResult List()
        {
            var model = new CommentModel();
            const int pageSize = 10;
            int count = 0;
            int pageIndex = PressRequest.GetInt("page", 1);
            int approved = PressRequest.GetQueryInt("approved", -1);

            var list = CommentService.GetCommentListPage(pageSize, pageIndex, out count, 1, -1, -1, -1, approved, -1, string.Empty);

            model.PageList.LoadPagedList(list);
            model.CommentList = (List<CommentInfo>)list;
            return View(model);
        }
    }
}
