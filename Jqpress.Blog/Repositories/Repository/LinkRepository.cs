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
    public partial class LinkRepository:ILinkRepository
    {
        DapperHelper dapper = new DapperHelper();
        /// <summary>
        /// insert link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public int Insert(LinkInfo link)
        {
            string cmdText = string.Format(@"insert into [{0}links]
                            (
                            [type],[linkname],[linkurl],[position],[target],[description],[sortnum],[status],[createtime]
                            )
                            values
                            (
                            @type,@linkname,@linkurl,@position,@target,@description,@sortnum,@status,@createtime
                            )", ConfigHelper.Tableprefix);

            using (var conn = dapper.OpenConnection())
            {
                conn.Execute(cmdText, new
                {
                    Type = link.Type,
                    LinkName = link.LinkName,
                    LinkUrl = link.LinkUrl,
                    Postion = link.Position,
                    Target = link.Target,
                    Description = link.Description,
                    SortNum = link.SortNum,
                    Status = link.Status,
                    CreateTime = link.CreateTime.ToString()
                });
                return conn.Query<int>(string.Format("select top 1 [linkid] from [{0}links]  order by [linkid] desc", ConfigHelper.Tableprefix), null).First();
            }

        }
        /// <summary>
        /// update link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public int Update(LinkInfo link)
        {
            string cmdText = string.Format(@"update [{0}links] set
                                [type]=@type,
                                [linkname]=@linkname,
                                [linkurl]=@linkurl,
                                [position]=@position,
                                [target]=@target,
                                [description]=@description,
                                [sortnum]=@sortnum,
                                [status]=@status,
                                [createtime]=@createtime
                                where linkid=@linkid", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
               return conn.Execute(cmdText, new
                {
                    Type = link.Type,
                    LinkName = link.LinkName,
                    LinkUrl = link.LinkUrl,
                    Postion = link.Position,
                    Target = link.Target,
                    Description = link.Description,
                    SortNum = link.SortNum,
                    Status = link.Status,
                    CreateTime = link.CreateTime.ToString(),
                    Linkid = link.LinkId
                });
            }
        }
        /// <summary>
        /// delete link
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public int Delete(LinkInfo link)
        {
            string cmdText = string.Format("delete from [{0}links] where [linkid] = @linkid", ConfigHelper.Tableprefix);
            using (var conn = dapper.OpenConnection())
            {
                return conn.Execute(cmdText, new { categoryid = link.LinkId });
            }
        }

        /// <summary>
        /// 获取全部链接
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<LinkInfo> Table
        {
            get
            {
                string cmdText = string.Format("select * from [{0}links]  order by [sortnum] asc,[linkid] asc", ConfigHelper.Tableprefix);
                using (var conn = dapper.OpenConnection())
                {
                    var list = conn.Query<LinkInfo>(cmdText, null);
                    return list;
                }
            }

        }
    }
}
