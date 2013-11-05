using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Blog.Common;
using Jqpress.Blog.Entity;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Blog.Services;

namespace Jqpress.Web.Controllers
{
    public partial class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LoginOn() 
        {
            if (VerifyLogin())
            {
                return RedirectToAction("index", "admin");
            }
            return View();
        }

        /// <summary>
        /// 登录验证
        /// </summary>
        public bool VerifyLogin()
        {
            UserInfo user = null;
            string userName = PressRequest.GetFormString("username");
            string password = Jqpress.Framework.Utils.EncryptHelper.MD5(PressRequest.GetFormString("password"));
            int expires = PressRequest.GetFormString("rememberme") == "forever" ? 43200 : 0;

            user = UserService.GetUser(userName, password);

            if (user != null)
            {
                if (user.Status == 0)
                {
                    ModelState.AddModelError("", "此用户已停用");
                }
                UserService.WriteUserCookie(user.UserId, user.UserName, user.Password, expires);
                return true;
            }
            else
            {
                ModelState.AddModelError("", "用户名或密码错误!");
            }
            return false;
        }

        

    }
}
