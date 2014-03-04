using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.Utils;
using Jqpress.Framework.Web;
using Jqpress.Framework.Mvc;
using Jqpress.Blog.Domain;
using Jqpress.Web.Areas.Admin.Models;
using FileInfo =  Jqpress.Blog.Domain.FileInfo;

namespace Jqpress.Web.Areas.Admin.Controllers
{
    public class FilesController : BaseAdminController
    {
        public string urlpath = PressRequest.GetQueryString("path");
        public string rootpath = "/upfiles";

        private string FileName;

        /// <summary>
        /// 文件浏览
        /// </summary>
        /// <returns></returns>
        public ActionResult List()
        {
            int pageIndex = PressRequest.GetQueryInt("page", 1);
            int pageSize = 30;
            if (string.IsNullOrEmpty(urlpath))
            {
                urlpath = rootpath;
            }
            var list = GetFilesList(urlpath);
            var model = new FilesModel();
            model.FoldList = list.FoldList.Select(fold => new FoldInfo
                                                              {
                                                                  FoldName = fold.FoldName,
                                                                  FoldPath = fold.FoldPath,
                                                                  FileSystemInfosLength = fold.FileSystemInfosLength
                                                              }).ToList();
            model.FileList = list.FileList.Select(f => new FileInfo
                                                           {
                                                               FileName = f.FileName,
                                                               Extension = f.Extension,
                                                               FileLength = f.FileLength,
                                                               FilePath = f.FilePath,
                                                               FileUrl = f.FileUrl
                                                           }).ToList();
            if (model.FileList.Count == 0)
            {
                IPagedList<FoldInfo> foldlist = new PagedList<FoldInfo>(model.FoldList, pageIndex - 1, pageSize);
                model.FoldPageList.LoadPagedList<FoldInfo>(foldlist);          
                model.FoldList = (List<FoldInfo>)foldlist;             
            }
            string controller = RouteData.Values["controller"].ToString();
            string aera = "Admin";
            model.CurrentAction = "/" + aera + "/" + controller + "/" + "list";
            model.UserDirectory = new DirectoryInfo(Server.MapPath(urlpath));
            ViewBag.path = urlpath;
            ViewBag.UrlPath = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            model.PathUrl = GetPathUrl();
            return View(model);
        }


        /// <summary>
        /// 上传保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Upload()
        {
            var file = Request.Files["FileUpload"];
            string basepath = PressRequest.GetQueryString("path");
            if (file != null)
            {
                if (string.IsNullOrEmpty(urlpath))
                {
                    urlpath = rootpath;
                }
                var savepath = Server.MapPath(urlpath + "/" + file.FileName);
                file.SaveAs(savepath);
            }
            return RedirectToAction("list", new { path = urlpath });
        }

        /// <summary>
        /// 删除路径下相应文件
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteFile()
        {
            string path = PressRequest.GetQueryString("path").Trim();
            string url = PressRequest.GetQueryString("url").Trim();
            if (System.IO.File.Exists(path) || Directory.Exists(path))
            {
                if (System.IO.File.Exists(path))
                {
                    System.IO.FileInfo file = new System.IO.FileInfo(path);
                    file.Delete();
                }
                else
                {
                    if (Directory.Exists(path))
                    {
                        DirectoryInfo di = new DirectoryInfo(path);
                        di.Delete(true);
                    }
                }
            }
            return RedirectToAction("list", new { path = url });
        }

        public FilesModel GetFilesList(string path)
        {
            path = path.Replace("//", "/");
            var model = new FilesModel();
            if (!Directory.Exists(Server.MapPath(path)))
                return model;

            var dic = new DirectoryInfo(Server.MapPath(path));
            foreach (var d in dic.GetFileSystemInfos())
            {
                if (d is DirectoryInfo)
                {
                    var fold = new FoldInfo
                    {
                        FoldName = d.Name,
                        FoldPath = path,
                        FileSystemInfosLength = ((DirectoryInfo)d).GetFileSystemInfos().Length
                    };
                    model.FoldList.Add(fold);
                }
                else
                {
                    var file = new FileInfo
                    {
                        FileName = d.Name,
                        FileLength = FileHelper.ConvertUnit(((System.IO.FileInfo)d).Length),
                        Extension = d.Extension,
                        FileUrl = path + "/" + d.Name,
                        FilePath = d.FullName
                    };
                    model.FileList.Add(file);
                }
            }
            return model;
        }
        /// <summary>
        /// 获取当前路径所有连接
        /// </summary>
        protected string GetPathUrl()
        {
            string pathLink = string.Empty;
            FileName = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            urlpath = urlpath.Replace("//", "");
            string path2 = urlpath.Substring(1, urlpath.Length - 1);

            string[] tempPath = path2.Split('/');

            string temp = "/";

            pathLink = ConfigHelper.SitePath.TrimEnd('/');

            for (int i = 0; i < tempPath.Length; i++)
            {

                temp += (i != 0 ? "/" : "") + tempPath[i];
                if (i == 0 && ConfigHelper.SitePath.Length > 1) //有虚拟目录
                {
                    continue;
                }
                pathLink += string.Format("/<a href='{2}?path={0}'>{1}</a>", temp, tempPath[i], FileName);
            }
            return pathLink.Replace("//", "");
        }
    }
}
