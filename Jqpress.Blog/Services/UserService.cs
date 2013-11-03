using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Jqpress.Blog.Data;
using Jqpress.Blog.Entity;
using Jqpress.Blog.Common;
using Jqpress.Blog.Entity;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Configuration;
using Jqpress.Blog.Services;

namespace Jqpress.Blog.Services
{
   public class UserService
    {
               /// <summary>
        /// 列表
        /// </summary>
        private static List<UserInfo> _users;

        /// <summary>
        /// lock
        /// </summary>
        private static object lockHelper = new object();

        static UserService()
        {
            LoadUser();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public static void LoadUser()
        {
            if (_users == null)
            {
                lock (lockHelper)
                {
                    if (_users == null)
                    {
                        _users = DatabaseProvider.Instance.GetUserList();

                        //   BuildUser();
                    }
                }
            }
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="_userinfo"></param>
        /// <returns></returns>
        public static int InsertUser(UserInfo _userinfo)
        {
            _userinfo.UserId = DatabaseProvider.Instance.InsertUser(_userinfo);
            _users.Add(_userinfo);
            _users.Sort();

            return _userinfo.UserId;
        }

        /// <summary>
        /// 修改用户
        /// </summary>
        /// <param name="_userinfo"></param>
        /// <returns></returns>
        public static int UpdateUser(UserInfo _userinfo)
        {
            _users.Sort();
            return DatabaseProvider.Instance.UpdateUser(_userinfo);
        }

        /// <summary>
        /// 更新用户文章数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public static int UpdateUserPostCount(int userId, int addCount)
        {
            UserInfo user = GetUser(userId);
            if (user != null)
            {
                user.PostCount += addCount;
                return UpdateUser(user);
            }
            return 0;
        }

        /// <summary>
        /// 更新用户评论数
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        public static int UpdateUserCommentCount(int userId, int addCount)
        {
            UserInfo user = GetUser(userId);
            if (user != null)
            {
                user.CommentCount += addCount;

                return UpdateUser(user);
            }
            return 0;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static int DeleteUser(int userId)
        {
            UserInfo user = GetUser(userId);
            if (user != null)
            {
                _users.Remove(user);
            }

            return DatabaseProvider.Instance.DeleteUser(userId);
        }


        /// <summary>
        /// 获取全部用户
        /// </summary>
        /// <returns></returns>
        public static List<UserInfo> GetUserList()
        {
            return _users;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static bool ExistsUserName(string userName)
        {
            return DatabaseProvider.Instance.ExistsUserName(userName);
        }

        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public static UserInfo GetUser(int userId)
        {
            foreach (UserInfo user in _users)
            {
                if (user.UserId == userId)
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据用户名获取用户 
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static UserInfo GetUser(string userName)
        {

            foreach (UserInfo user in _users)
            {
                if (user.UserName.ToLower() == userName.ToLower())
                {
                    return user;
                }
            }
            return null;
        }

        /// <summary>
        /// 根据用户名和密码获取用户
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static UserInfo GetUser(string userName, string password)
        {
            foreach (UserInfo user in _users)
            {
                if (  user.UserName.ToLower() == userName.ToLower() && user.Password.ToLower() == password.ToLower())
                {
                    return user;
                }
            }
            return null;
        }

        #region 用户COOKIE操作
        /// <summary>
        /// 用户COOKIE名
        /// </summary>
        private static readonly string CookieUser = ConfigHelper.SitePrefix + "amdin";
        /// <summary>
        /// 读当前用户COOKIE
        /// </summary>
        /// <returns></returns>
        public static HttpCookie ReadUserCookie()
        {
            return HttpContext.Current.Request.Cookies[CookieUser];
        }

        /// <summary>
        /// 移除当前用户COOKIE
        /// </summary>
        /// <returns></returns>
        public static bool RemoveUserCookie()
        {
            HttpCookie cookie = new HttpCookie(CookieUser);
            cookie.Values.Clear();
            cookie.Expires = DateTime.Now.AddYears(-1);

            HttpContext.Current.Response.AppendCookie(cookie);
            return true;
        }

        /// <summary>
        /// 写/改当前用户COOKIE
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="expires"></param>
        /// <returns></returns>
        public static bool WriteUserCookie(int userID, string userName, string password, int expires)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies[CookieUser];
            if (cookie == null)
            {
                cookie = new HttpCookie(CookieUser);
            }
            if (expires > 0)
            {
                cookie.Values["expires"] = expires.ToString();
                cookie.Expires = DateTime.Now.AddMinutes(expires);
            }
            else
            {
                int temp_expires = Convert.ToInt32(cookie.Values["expires"]);
                if (temp_expires > 0)
                {
                    cookie.Expires = DateTime.Now.AddMinutes(temp_expires);
                }
            }
            cookie.Values["userid"] = userID.ToString();
            cookie.Values["username"] = HttpContext.Current.Server.UrlEncode(userName);
            cookie.Values["key"] = Jqpress.Framework.Utils.EncryptHelper.MD5(userID + HttpContext.Current.Server.UrlEncode(userName) + password);

            HttpContext.Current.Response.AppendCookie(cookie);
            return true;
        }
        #endregion
    }
}
