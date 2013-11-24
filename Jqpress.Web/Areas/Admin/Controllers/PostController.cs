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
using System.Net;
using System.Text.RegularExpressions;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Configuration;
using Jqpress.Blog.Common;
using Jqpress.Blog.Entity;
using Jqpress.Blog.Services;
using Jqpress.Blog.Entity.Enum;
using Jqpress.Blog.Configuration;
using Jqpress.Web.Areas.Admin.Models;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    //TODO: 文章搜索，增加时间
    //TODO: 文章状态
    //TODO: 删除提示
    //TODO: 参考wordpress列表改造
    //TODO: 列表分页样式
    //TODO: post 的model重构
    public class PostController : BaseAdminController
    {
        public ActionResult List()
        {
            var model = new PostListModel();
            string keyword = StringHelper.SqlEncode(PressRequest.GetQueryString("keyword"));
            int categoryId = PressRequest.GetQueryInt("categoryid", -1);
            int userId = PressRequest.GetQueryInt("userid", -1);
            int hide = PressRequest.GetQueryInt("hide", -1);

            // txtKeyword.Text = keyword; 暂时注释
            // ddlCategory.SelectedValue = categoryId.ToString();
            //ddlAuthor.SelectedValue = userId.ToString();
            //  chkRecommend.Checked = recommend == 1 ? true : false; 暂时注释
            //chkHideStatus.Checked = hide == 1 ? true : false; 暂时注释

            var catelist = CategoryService.GetCategoryList();

            const int pageSize = 10;
            int count = 0;
            int pageIndex = PressRequest.GetInt("page", 1);
            int cateid = PressRequest.GetQueryInt("cateid", -1);
            int tagid = PressRequest.GetQueryInt("tagid", -1);

            catelist.Add(new CategoryInfo() { CateName="全部",CategoryId=-1});
            model.CateSelectItem = catelist.ConvertAll(c => new SelectListItem { Text = c.CateName, Value = c.CategoryId.ToString(), Selected = c.CategoryId == cateid });

            if (cateid > 0)
                pageIndex = pageIndex + 1;
            var postlist = PostService.GetPostPageList(pageSize, pageIndex, out count, categoryId, tagid, -1, -1, -1, -1, -1, "", "", keyword);
            model.PageList.LoadPagedList(postlist);
            model.PostList = (List<PostInfo>)postlist;
            return View(model);
        }

        /// <summary>
        /// delete article
        /// </summary>
        /// <returns></returns>
        public ActionResult Delete()
        {
            int postId = PressRequest.GetQueryInt("id");
            PostInfo post = PostService.GetPost(postId);
            if (post == null)
            {
                return RedirectToAction("list");
            }
            if (CurrentUser.UserType != (int)UserType.Administrator && CurrentUser.UserId != post.UserId)
            {
                return RedirectToAction("list");
            }

            PostService.DeletePost(postId);

            return RedirectToAction("list");
        }
        /// <summary>
        /// get article
        /// </summary>
        /// <returns></returns>
        public ActionResult Edit()
        {
            int postid = PressRequest.GetInt("id", 0);
            var model = new PostModel();
            var catelist = CategoryService.GetCategoryList();
            model.TagList = TagService.GetTagList();

            if (postid > 0)
            {
                model.Post = PostService.GetPost(postid);
                model.Post.Tag = model.Post.Tags.Aggregate(string.Empty, (current, t) => current + (t.CateName + ",")).TrimEnd(',');
                model.CateSelectItem = catelist.ConvertAll(c => new SelectListItem { Text = c.CateName, Value = c.CategoryId.ToString(), Selected = c.CategoryId == model.Post.CategoryId });
            }
            else
            {
                model.CateSelectItem = catelist.ConvertAll(c => new SelectListItem { Text = c.CateName, Value = c.CategoryId.ToString() });
            }
            return View(model);
        }

        /// <summary>
        /// save add or modify article
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost, ActionName("SavePost"), ValidateInput(false)]
        public ActionResult SavePost(PostInfo p)
        {
            int pages = PressRequest.GetFormInt("page", 1);

            p.UpdateTime = DateTime.Now;
            p.Tag = TagService.GetTagIdList(p.Tag);
            p.UserId = CurrentUserId;
            p.Slug = TypeConverter.ObjectToString(p.Slug);
            p.Summary = TypeConverter.ObjectToString(p.Summary);
            p.Status = PressRequest.GetFormInt("chkStatus", 1);
            var action = "edit";
            if (p.PostId > 0) action += "?id="+p.PostId;
            if (string.IsNullOrEmpty(p.Title))
            {
                ErrorNotification("标题不能为空");
                return Redirect(action);
            }
            if (string.IsNullOrEmpty(p.PostContent))
            {
                ErrorNotification("内容不能为空");
                return Redirect(action);
            }

            var isSaveMsg = PressRequest.GetFormInt("chkSaveImage",0);
            if (isSaveMsg>0)
            {
                p.PostContent = PostService.SaveRemoteImage(p.PostContent);
            }
            if (p.PostId>0)
            {
               var  post = PostService.GetPost(p.PostId);
                p.ViewCount = post.ViewCount;
                p.CommentCount = post.CommentCount;
                PostService.UpdatePost(p);
                string url = "http://" + PressRequest.GetCurrentFullHost() + "/post/" + (!string.IsNullOrEmpty(p.Slug) ? p.Slug : p.PostId.ToString());
                SuccessNotification("修改成功。<a href=\"" + url + "\">查看文章</a> ");
            }
            else
            {
                p.PostTime = DateTime.Now;
                p.PostId = PostService.InsertPost(p);
                string url = "http://" + PressRequest.GetCurrentFullHost() + "/post/" + (!string.IsNullOrEmpty(p.Slug) ? p.Slug : p.PostId.ToString());
                SuccessNotification("发布成功。<a href=\"" + url + "\">查看文章</a> ");
            }
            return Redirect("edit?id="+p.PostId);
        }

    }
}
