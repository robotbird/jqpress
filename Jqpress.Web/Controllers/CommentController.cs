using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Web.Models;
using Jqpress.Core.Services;

namespace Jqpress.Web.Controllers
{
    public class CommentController : Controller
    {
        private CommentService _commentService = new CommentService();
        public ActionResult List(int id)
        {
            var model = new CommentModel();
            model.Comments = _commentService.GetCommentsByPost(id);
            return View("Comment",model);
        }

    }
}
