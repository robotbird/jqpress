using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jqpress.Core.Domain;
using Jqpress.Core.Domain.Enum;
using Jqpress.Core.Services;


namespace Jqpress.Core.Common
{
    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperateType
    {
        /// <summary>
        /// 添加
        /// </summary>
        Insert = 0,
        /// <summary>
        /// 更新
        /// </summary>
        Update = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Delete = 2,
    }
    /// <summary>
    /// 后台基类
    /// </summary>
    public class AdminPage : BasePage
    {
         protected SiteConfigInfo setting;

        public AdminPage()
        {
            CheckLoginAndPermission();
            setting = Jqpress.Core.Configuration.SiteConfig.GetSetting();
        }

        /// <summary>
        /// 检查登录和权限
        /// </summary>
        protected void CheckLoginAndPermission()
        {
            if (!IsLogin)
            {
                HttpContext.Current.Response.Redirect("/admin/login?returnurl=" + HttpContext.Current.Server.UrlEncode(Jqpress.Framework.Web.PressRequest.CurrentUrl));
                HttpContext.Current.Response.End();
            }
            else
            {
                UserInfo user = new UserService().GetUser(CurrentUserId);

                if (user == null)       //删除已登陆用户时有效
                {
                    RemoveUserCookie();
                    HttpContext.Current.Response.Redirect("/admin/login?returnurl=" + HttpContext.Current.Server.UrlEncode(Jqpress.Framework.Web.PressRequest.CurrentUrl));

                }

                if (Jqpress.Framework.Utils.EncryptHelper.MD5(user.UserId + HttpContext.Current.Server.UrlEncode(user.UserName) + user.Password) != CurrentKey)
                {
                    RemoveUserCookie();
                    HttpContext.Current.Response.Redirect("/admin/login?returnurl=" + HttpContext.Current.Server.UrlEncode(Jqpress.Framework.Web.PressRequest.CurrentUrl));
                }

                if (CurrentUser.Status == 0)
                {
                    ResponseError("您的用户名已停用", "您的用户名已停用,请与管理员联系!");
                }

                string[] plist = new string[] { "theme/list", "theme/edit", "link/list", "user/list", "setting", "category/list", "tag/list", "comment" };
                if (CurrentUser.Role == (int)UserRole.Author)
                {
                    string pageName = System.IO.Path.GetFileName(HttpContext.Current.Request.Url.ToString()).ToLower();

                    foreach (string p in plist)
                    {
                        if (pageName == p)
                        {
                            ResponseError("没有权限", "您没有权限使用此功能,请与管理员联系!");
                        }
                    }
                }

            }



        }
    }
}
