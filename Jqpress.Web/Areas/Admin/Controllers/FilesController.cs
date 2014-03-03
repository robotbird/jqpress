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

        public ActionResult Index()
        {
            return View();
        }
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
            model.CurrentAction = "/" + aera + "/" + controller + "/" + "FilesBrowser";
            model.UserDirectory = new DirectoryInfo(Server.MapPath(urlpath));
            ViewBag.path = urlpath;
            ViewBag.UrlPath = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            model.PathUrl = GetPathUrl();
            return View(model);
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

        #region 以下需要逐步实现的
        /*
        /// <summary>
        /// 图片添加浏览页
        /// </summary>
        /// <returns></returns>
        public ActionResult FilesPicBrowser()
        {
            var imgWebService = new FileWebService();
            int pageIndex = PressRequest.GetQueryInt("page", 1);
            string basepath ="/"+ PressRequest.GetQueryString("path").Replace("//","");
            int pageSize = 20;

            if (string.IsNullOrEmpty(urlpath) && string.IsNullOrEmpty(basepath))
                urlpath = rootpath;

            if (!string.IsNullOrEmpty(basepath))
                urlpath = basepath;

            var list = imgWebService.GetFilesList(urlpath);
            var model = new ImageListModel();
            #region webservice
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
            #endregion
            if (model.FileList.Count == 0)
            {
                IPagedList<FoldInfo> foldlist = new PagedList<FoldInfo>(model.FoldList, pageIndex - 1, pageSize);
                model.FoldPageList.LoadPagedList<FoldInfo>(foldlist);
                model.FoldList = (List<FoldInfo>)foldlist;
            }
            string controller = RouteData.Values["controller"].ToString();
            model.ModelID = Convert.ToInt32(RouteData.Values["modelid"]);
            string aera = "Admin";
            model.CurrentAction = "/" + aera + "/" + controller + "/" + "FilesBrowser";
            model.CurrentAction = model.CurrentAction.Replace("//", "/");
            model.UserDirectory = new DirectoryInfo(Server.MapPath(urlpath));
            ViewBag.path = urlpath;
            ViewBag.UrlPath = System.IO.Path.GetFileName(Request.Url.AbsolutePath);
            model.PathUrl = GetPathUrl().Replace("//","");
            return View(model);            
        }
        /// <summary>
        /// 删除路径下相应文件
        /// add by qiankun 20121009
        /// </summary>
        /// <returns></returns>
        public ActionResult DeleteFile()
        {
            var imgWebService = new FileWebService();
            string paths = PressRequest.GetQueryString("path").Trim();
            string url=PressRequest.GetQueryString("url").Trim();
            if (paths.IndexOf("userfiles") >= 0)
            {
                paths = paths.Substring(paths.IndexOf("uploadfiles"));
            }
            if (paths.IndexOf("uploadfiles")>=0)
            {
                paths = paths.Substring(paths.IndexOf("uploadfiles"));                
            }
            imgWebService.DeletePic(paths);
            if (states == 1)
            {
                return RedirectToAction("FilesPicBrowser", new { path = url });
            }
            else 
            {
                return RedirectToAction("", new { path =url  });
            }
        
            }
        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveFiles()
        {
            var file = Request.Files["FileUpload"];
            string basepath = PressRequest.GetQueryString("path");
            //SaveLocalFiles();       
            SaveWebServiceFiles(file, basepath);
            if (!string.IsNullOrEmpty(basepath))
                urlpath = basepath;
           if ( states== 1)
            {
                return RedirectToAction("FilesPicBrowser", new { path = urlpath });
            }
           else
            {
                return RedirectToAction("FilesBrowser", new { path = urlpath });
            }
           
        }
        /// <summary>
        /// 保存本地路径
        /// </summary>
        void SaveLocalFiles(HttpPostedFileBase file)
        {
            if (file != null)
            {
                if (string.IsNullOrEmpty(urlpath))
                {
                    urlpath = rootpath;
                }
                var savepath = Server.MapPath(urlpath + "/" + file.FileName);
                file.SaveAs(savepath);
            }
        }

        /// <summary>
        /// 保存到远程服务器
        /// </summary>
        void SaveWebServiceFiles(HttpPostedFileBase file,string basepath="")
        {
            var imgWebService = new FileWebService();
            string dir = urlpath+"/";
            dir = dir.Replace(rootpath, "");
            if (basepath!="")
            {
                dir = dir.Replace(basepath, "");
            }
            string filename = file.FileName;
            var img = new ImageInfo { FileName = filename, Path = dir, ByteImage = FileHelper.HttpPostedFileToByte(file) };
            imgWebService.UploadImageBase(img, basepath);
        }
        */
        #endregion
    }
}
