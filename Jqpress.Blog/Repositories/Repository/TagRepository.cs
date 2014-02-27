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
    public partial class TagRepository
    {
        DapperHelper dapper = new DapperHelper();
        /// <summary>
        /// 检查别名是否重复
        /// </summary>
        /// <param name="cate"></param>
        /// <returns></returns>
        private  void CheckSlug(TagInfo cate)
        {
            while (true)
            {
                string cmdText = cate.TagId == 0 ? string.Format("select count(1) from [{2}category] where [Slug]='{0}' and [type]={1}", cate.Slug, (int)CategoryType.Tag, ConfigHelper.Tableprefix) : string.Format("select count(1) from [{3}category] where [Slug]='{0}'  and [type]={1} and [categoryid]<>{2}", cate.Slug, (int)CategoryType.Tag, cate.TagId, ConfigHelper.Tableprefix);
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

        public int Insert(TagInfo tag)
        {
            CheckSlug(tag);

            string cmdText = string.Format(@"insert into [{0}category]
                            (
                            [Type],[ParentId],[CateName],[Slug],[Description],[SortNum],[PostCount],[CreateTime]
                            )
                            values
                            (
                            @Type,@ParentId,@CateName,@Slug,@Description,@SortNum,@PostCount,@CreateTime
                            )", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                conn.Execute(cmdText, new
                {
                    Type = (int)CategoryType.Tag,
                    ParentId = 0,
                    CateName = tag.CateName,
                    Slug = tag.Slug,
                    Description = tag.Description,
                    SortNum = tag.SortNum,
                    PostCount = tag.PostCount,
                    CreateTime = tag.CreateTime.ToString()
                });
                return conn.Query<int>(string.Format("select top 1 [categoryid] from [{0}category] order by [categoryid] desc", ConfigHelper.Tableprefix), null).First();
            }
        }

        public int Update(TagInfo tag)
        {
            CheckSlug(tag);

            string cmdText = string.Format(@"update [{0}category] set
                                [Type]=@Type,
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
                    Type = (int)CategoryType.Tag,
                    CateName = tag.CateName,
                    Slug = tag.Slug,
                    Description = tag.Description,
                    SortNum = tag.SortNum,
                    PostCount = tag.PostCount,
                    CreateTime = tag.CreateTime.ToString(),
                    categoryid = tag.TagId
                });
            }

        }

        public int Delete(TagInfo tag)
        {
            string cmdText = string.Format("delete from [{0}category] where [categoryid] = @categoryid", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                return conn.Execute(cmdText, new { categoryid = tag.TagId });
            }
        }

        public TagInfo GetById(object id)
        {
            string cmdText = string.Format("select * from [{0}category] where [categoryid] = @categoryid", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                var list = conn.Query<TagInfo>(cmdText, new { categoryid = (int)id });
                return list.ToList().Count > 0 ? list.ToList()[0] : null;
            }
        }
        /// <summary>
        /// 获取全部标签
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TagInfo> Table
        {
            get
            {
                string condition = " [type]=" + (int)CategoryType.Tag;

                string cmdText = string.Format("select * from [{0}category] where " + condition + " order by [SortNum] asc,[categoryid] asc", ConfigHelper.Tableprefix);
                using (var conn = dapper.OpenConnection())
                {
                    var list = conn.Query<TagInfo>(cmdText, null);
                    return list;
                }
            }

        }

        public List<TagInfo> GetTagList(int[] ids)
        {
            var result = from t in Table
                         where ids.Contains(t.TagId)
                         select t;
            return result.ToList();
        }


    }
}
