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
    public class UserController : BaseAdminController
    {
        public ActionResult List()
        {
            var model = new UserListModel();


            List<UserInfo> list = UserService.GetUserList();
            model.UserList = list;
            var RolesDic = Enum.GetValues(typeof (Jqpress.Blog.Entity.Enum.UserRole)).Cast<int>().ToDictionary(s => Enum.GetName(typeof (Jqpress.Blog.Entity.Enum.UserRole), s));
            
            var dic = new Dictionary<string,string>();
            dic.Add("Administrator","管理员");
            dic.Add("Editor","编辑");
            dic.Add("Subscriber","订阅者");
            dic.Add("Contributor","投稿者");
            dic.Add("Author","作者");
            foreach (int id in Enum.GetValues(typeof(Jqpress.Blog.Entity.Enum.UserRole)))
            {
                string name = Enum.GetName(typeof(Jqpress.Blog.Entity.Enum.UserRole), id);
                foreach(KeyValuePair<string,string> kv in dic)
                {
                    if(kv.Key == name)
                    {
                        var Item = new SelectListItem(){Text=kv.Value,Value=id.ToString()};
                        model.RolesCateItem.Add(Item);    
                    }
                }
            }

            return View(model);
        }

        /// <summary>
        /// get user by id
        /// </summary>
        public JsonResult Edit(int? id)
        {
            var uid = id ?? 0;
            var user = UserService.GetUser(uid);
            return Json(user, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// delete user by id
        /// </summary>
        public ContentResult Delete(int? id)
        {
            var uid = id ?? 0;
            UserService.DeleteUser(uid);
            return Content("success");
        }
        /// <summary>
        /// insert or update user
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        [HttpPost, ActionName("Save"), ValidateInput(false)]
        public JsonResult Save(UserInfo u)
        {
            var password2 = Request.Form["password2"];
            if (u.UserId>0)
            {
                u = UserService.GetUser(u.UserId);
            }
            else
            {
                u.CommentCount = 0;
                u.CreateTime = DateTime.Now;
                u.PostCount = 0;
            }

            if (!string.IsNullOrEmpty(u.Password))
            {
                u.Password = EncryptHelper.MD5(u.Password);
            }
            if (!string.IsNullOrEmpty(u.Password) && u.Password != password2)
            {
                return Json("两次密码输入不相同!");
            }

            if (u.UserId>0)//更新操作
            {
                UserService.UpdateUser(u);

                //  如果修改当前用户,则更新COOKIE
                if (!string.IsNullOrEmpty(u.Password) && u.UserId == CurrentUserId)
                {
                   UserService.WriteUserCookie(u.UserId, u.UserName, u.Password, 0);
                }
                return Json(u);
            }
            else//添加操作
            {
                #region 验证处理
                if (string.IsNullOrEmpty(u.UserName))
                {
                    return Json("请输入登陆用户名!");
                }

                System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex("[A-Za-z0-9\u4e00-\u9fa5-]");
                if (!reg.IsMatch(u.UserName))
                {
                    return Json("用户名限字母,数字,中文,连字符!");
                }
                if (Jqpress.Framework.Utils.Validate.IsInt(u.UserName))
                {
                    return Json("用户名不能为全数字!");
                }

                if (string.IsNullOrEmpty(u.Password))
                {
                    return Json("请输入密码!");
                }
                if (UserService.ExistsUserName(u.UserName))
                {
                    return Json("该用户名已存在,请换之");
                }
                #endregion

                u.UserId = UserService.InsertUser(u);
                return Json(u);
            }
        }
    }
}
