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

            // txtKeyword.Text = keyword; 暂时注释
            // ddlCategory.SelectedValue = categoryId.ToString();
            //ddlAuthor.SelectedValue = userId.ToString();
            //  chkRecommend.Checked = recommend == 1 ? true : false; 暂时注释
            //chkHideStatus.Checked = hide == 1 ? true : false; 暂时注释

            const int pageSize = 10;
            int count = 0;
            int pageIndex = PressRequest.GetInt("page", 1);
            int cateid = PressRequest.GetQueryInt("cateid", -1);
            int tagid = PressRequest.GetQueryInt("tagid", -1);
            if (cateid > 0)
                pageIndex = pageIndex + 1;
            var postlist = PostService.GetPostPageList(pageSize, pageIndex, out count, categoryId, tagid, -1, -1, -1, -1, -1, "", "", "");
            model.PageList.LoadPagedList(postlist);
            model.PostList = (List<PostInfo>)postlist;
            return View(model);
        }
        #endregion



    }
}
