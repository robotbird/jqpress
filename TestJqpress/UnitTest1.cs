using System;
using System.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Jqpress.Core.Domain;
using Jqpress.Framework.Configuration;
using Jqpress.Framework.DbProvider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestJqpress
{
    [TestClass]
    public class UnitTest1
    {
        //public static string ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=Jqpress.mdb";
        private readonly string sqlconnection = ConfigurationManager.ConnectionStrings["Ems_Connection"].ConnectionString;
        public static string ConnectionString = string.Format("Data Source=Jqpress.db;Pooling=true;FailIfMissing=false");//for windows

        [TestMethod]
        public void TestMethod1()
        {
            //var connection = new DapperHelper().OpenConnection();
            //var sql = @"select q.*,t.typeid,t.typename from ems_Question q,ems_Questiontype t where t.typeid=q.typeid";
            //var data = connection.Query<Question, QuestionType, Question>(sql, (quest, type) =>
            //{
            //    quest.QuestionType = type;
            //    return quest;
            //}, splitOn: "typeid,qstid");

            //foreach (Question q in data)
            //{
            //    Console.WriteLine(q.QstDescription + "   " + q.QuestionType.TypeName);
            //}
            //connection.Close();


           // string cmdText = string.Format("select p.*,u.UserId,u.UserName,u.NickName from [{0}posts] p,[{0}Users] u where p.UserId=u.UserId ", ConfigHelper.Tableprefix);
            // using (var conn = new DapperHelper().OpenConnection(ConnectionString))
            string cmdText = string.Format("select  p.*,u.UserId,u.UserName,c.CategoryId,c.CateName from [{0}posts] p,[{0}Users] u,[{0}category] c where p.PostId=@PostId and p.UserId=u.UserId and p.CategoryId=c.CategoryId", ConfigHelper.Tableprefix);
           
            using (var conn = new DapperHelper().OpenConnection(ConnectionString))
            {
                // var list = conn.Query<PostInfo>(cmdText, new { PostId = (int)id });
                // return list.Any() ? list.ToList()[0] : null;
                var list = conn.Query<PostInfo, UserInfo, CategoryInfo, PostInfo>(cmdText, (post, user, cate) =>
                {
                    post.Author = user;
                    post.Category = cate;
                    return post;
                }, new { PostId = 384 }, splitOn: "UserId,CategoryId");

                foreach (var postInfo in list)
                {
                    Console.WriteLine(postInfo.PostId + "  " + postInfo.Title);
                }
            }

        }
    }
}
