using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Jqpress.Blog.Domain;
using Jqpress.Framework.Cache;
using Jqpress.Blog.Repositories.Repository;
using Jqpress.Blog.Repositories.IRepository;

namespace Jqpress.Blog.Services
{
    /// <summary>
    /// 归档管理
    /// </summary>
    public class ArchiveService
    {
        private IPostRepository _postRepository;

        #region 构造函数
        /// <summary>
        /// 构造器方法
        /// </summary>
        public ArchiveService()
            : this(new PostRepository())
        {
        }
        /// <summary>
        /// 构造器方法
        /// </summary>
        /// <param name="postRepository"></param>
        public ArchiveService(IPostRepository postRepository)
        {
            this._postRepository = postRepository;
        }
        #endregion

        /// <summary>
        /// 获取归档文章并存入缓存
        /// </summary>
        /// <returns></returns>
        public  List<ArchiveInfo> GetArchive()
        {
            string archiveCacheKey = "archive";
            List<ArchiveInfo> list = (List<ArchiveInfo>)CacheHelper.Get(archiveCacheKey);

            if (list == null)
            {
                list = _postRepository.GetArchive();

                CacheHelper.Insert(archiveCacheKey, list, CacheHelper.HourFactor * 12);
            }
            return list;
        }
    }
}
