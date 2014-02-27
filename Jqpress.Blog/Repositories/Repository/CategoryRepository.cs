using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jqpress.Framework.DbProvider;
using Jqpress.Framework.Configuration;
using Jqpress.Blog.Domain;
using Jqpress.Blog.Domain.Enum;
using Jqpress.Blog.Repositories.IRepository;


namespace Jqpress.Blog.Repositories.Repository
{
    public partial class CategoryRepository:ICategoryRepository
    {
        DapperHelper dapper = new DapperHelper();

        /// <summary>
        /// 添加分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual int Insert(CategoryInfo category)
        {
            CheckSlug(category);

            string cmdText = string.Format(@"insert into [{0}category]
                            ([ParentId],[CateName],[Slug],[Description],[SortNum],[PostCount],[CreateTime])
                            values
                            (@ParentId,@CateName,@Slug,@Description,@SortNum,@PostCount,@CreateTime)", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection()) 
            {
                conn.Execute(cmdText, new { 
                    ParentId = category.ParentId,
                    CateName = category.CateName,
                    Slug = category.Slug,
                    Description = category.Description,
                    SortNum = category.SortNum,
                    PostCount = category.PostCount,
                    CreateTime = category.CreateTime.ToString()
                });
                return conn.Query<int>(string.Format("select top 1 [categoryid] from [{0}category] order by [categoryid] desc", ConfigHelper.Tableprefix), null).First();
            }
        }
        /// <summary>
        /// 更新分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public virtual int Update(CategoryInfo category)
        {
            CheckSlug(category);

            string cmdText = string.Format(@"update [{0}category] set
                                [ParentId]=@ParentId,
                                [CateName]=@CateName,
                                [Slug]=@Slug,
                                [Description]=@Description,
                                [SortNum]=@SortNum,
                                [PostCount]=@PostCount,
                                [CreateTime]=@CreateTime
                                where categoryid=@categoryid", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
               return conn.Execute(cmdText, new
                {
                    ParentId = category.ParentId,
                    CateName = category.CateName,
                    Slug = category.Slug,
                    Description = category.Description,
                    SortNum = category.SortNum,
                    PostCount = category.PostCount,
                    CreateTime = category.CreateTime.ToString(),
                    categoryid = category.CategoryId
                });
            }
        }
        /// <summary>
        /// 删除分类
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public int Delete(CategoryInfo category)
        {
            string cmdText = string.Format("delete from [{0}category] where [categoryid] = @categoryid", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                return conn.Execute(cmdText, new { categoryid = category.CategoryId });
            }
        }
        /// <summary>
        /// 获取分类
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public CategoryInfo GetById(object id)
        {
            string cmdText = string.Format("select * from [{0}category] where [categoryid] = @categoryid", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                var list = conn.Query<CategoryInfo>(cmdText, new { categoryid = (int)id });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }

        /// <summary>
        /// 获取全部分类
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<CategoryInfo> Table
        {
            get
            {
                string cmdText = string.Format("select * from [{0}category]  order by [SortNum] asc,[categoryid] asc", ConfigHelper.Tableprefix);
                using (var conn = dapper.OpenConnection())
                {
                    var list = conn.Query<CategoryInfo>(cmdText, null);
                    return list;
                }
            }

        }

        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        private  void CheckSlug(CategoryInfo cate)
        {
            while (true)
            {
                string cmdText = cate.CategoryId == 0 ? string.Format("select count(1) from [{2}category] where [Slug]='{0}' and [type]={1}", cate.Slug, (int)CategoryType.Category, ConfigHelper.Tableprefix) : string.Format("select count(1) from [{3}category] where [Slug]='{0}'  and [type]={1} and [categoryid]<>{2}", cate.Slug, (int)CategoryType.Category, cate.CategoryId, ConfigHelper.Tableprefix);
                using (var conn = dapper.OpenConnection())
                {
                    int r = conn.Query<int>(cmdText, null).First();

                    if (r == 0)
                    {
                        return;
                    }
                }
                cate.Slug += "-2";
            }
        }
    }
}
