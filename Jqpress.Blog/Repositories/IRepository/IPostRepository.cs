using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Domain;

namespace Jqpress.Blog.Repositories.IRepository
{
    public partial interface IPostRepository:IRepository<PostInfo>
    {
        /// <summary>
        /// 根据slug获取实体
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        PostInfo GetPostbySlug(string slug);
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordCount"></param>
        /// <param name="categoryId"></param>
        /// <param name="tagId"></param>
        /// <param name="userId"></param>
        /// <param name="recommend"></param>
        /// <param name="status"></param>
        /// <param name="topstatus"></param>
        /// <param name="PostStatus"></param>
        /// <param name="begindate"></param>
        /// <param name="enddate"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
         List<PostInfo> GetPostList(int pageSize, int pageIndex, out int recordCount, int categoryId, int tagId, int userId, int recommend, int status, int topstatus, int PostStatus, string begindate, string enddate, string keyword);
        /// <summary>
        /// 获取相关文章
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="rowCount"></param>
        /// <returns></returns>
        List<PostInfo> GetPostListByRelated(int postId, int rowCount);

        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <returns></returns>
        void CheckSlug(PostInfo post);
        /// <summary>
        /// 获取文章归档统计
        /// </summary>
        /// <returns></returns>
        List<ArchiveInfo> GetArchive();
        /// <summary>
        /// 更新访问量
        /// </summary>
        /// <param name="postId"></param>
        /// <param name="addCount"></param>
        /// <returns></returns>
        int UpdatePostViewCount(int postId, int addCount);

    }
}
