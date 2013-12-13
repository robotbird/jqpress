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

        /// <summary>
        /// delete
        public ContentResult Delete(int? id)
        {
            var commentId = id ?? 0;
            CommentService.DeleteComment(commentId);
            return Content("success");
        }
        /// <summary>
        /// Approve
        /// </summary>
        public ContentResult Approve(int? id)
        {
            var commentId = id ?? 0;
            CommentInfo c = CommentService.GetComment(commentId);
            if (c != null)
            {
                if (c.Approved == 1)
                {
                    c.Approved = 0;
                }
                else 
                {
                    c.Approved = 1;                
                }
                if (CommentService.UpdateComment(c) > 0)
                {
                    if (c.Approved == 1)
                    {
                        return Content("approve");
                    }
                    else {
                        return Content("unapprove");                    
                    }
                }
            }
            return Content("failure");
        }

    }
}
