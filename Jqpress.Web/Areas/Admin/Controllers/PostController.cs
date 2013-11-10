using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Xml.XPath;
using System.Xml.Schema;
using System.IO;
using Jqpress.Blog.Common;
using Jqpress.Blog.Entity;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Configuration;
using Jqpress.Blog.Services;
using Jqpress.Blog.Entity.Enum;
using Jqpress.Blog.Configuration;
using Jqpress.Web.Areas.Admin.Models;


namespace Jqpress.Web.Areas.Admin.Controllers
{
    public class PostController : Controller
    {
        //
        // GET: /Admin/Post/

        public ActionResult Index()
        {
            return View();
        }

        #region 文章
        public ActionResult List()
        {
            var model = new PostListModel();
            string keyword = StringHelper.SqlEncode(PressRequest.GetQueryString("keyword"));
            int categoryId = PressRequest.GetQueryInt("categoryid", -1);
            int userId = PressRequest.GetQueryInt("userid", -1);
            int recommend = PressRequest.GetQueryInt("recommend", -1);
            int hide = PressRequest.GetQueryInt("hide", -1);

            int pageindex = 0;
            const int pagesize = 20;
            pageindex = PressRequest.GetInt("page", 1);
            ViewBag.Page = pageindex;
            int count = 0;

            // txtKeyword.Text = keyword; 暂时注释
            // ddlCategory.SelectedValue = categoryId.ToString();
            //ddlAuthor.SelectedValue = userId.ToString();
            //  chkRecommend.Checked = recommend == 1 ? true : false; 暂时注释
            //chkHideStatus.Checked = hide == 1 ? true : false; 暂时注释

            int totalRecord = 0;

            // List<PostInfo> list = PostService.GetPostList(Pager1.PageSize, Pager1.PageIndex, out totalRecord, categoryId, -1, userId, recommend, -1, -1, hide, string.Empty, string.Empty, keyword);

            return View(model);
        }
        #endregion



    }
}
