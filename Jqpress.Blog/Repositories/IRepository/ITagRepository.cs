using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Domain;

namespace Jqpress.Blog.Repositories.IRepository
{
    public partial interface ITagRepository : IRepository<TagInfo>
    {
       /// <summary>
        /// 根据名称获取标签
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        TagInfo GetTagByName(string name);

        /// <summary>
        /// 根据slug获取标签
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        TagInfo GetTagBySlug(string slug);
    }
}
