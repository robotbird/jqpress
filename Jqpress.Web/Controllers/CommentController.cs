using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Core.Domain;
using Jqpress.Core.Services;
using Jqpress.Web.Models;

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

        [HttpPost]
        public ActionResult Add(CommentInfo comment) 
        {

            return Content("success");
        }

    }
}
