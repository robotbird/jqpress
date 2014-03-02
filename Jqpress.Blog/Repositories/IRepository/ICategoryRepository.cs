using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Blog.Domain;


namespace Jqpress.Blog.Repositories.IRepository
{
    public partial interface ICategoryRepository : IRepository<CategoryInfo>
    {
        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        CategoryInfo GetCategoryBySlug(string slug);
    }
}
