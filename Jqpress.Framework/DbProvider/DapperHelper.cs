using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using Jqpress.Framework.Configuration;
using System.Data.SQLite;

namespace Jqpress.Framework.DbProvider
{
    public class DapperHelper
    {
        private static string _mdbpath = "";
        public static string ConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + _mdbpath;

        //连接数据库字符串。
       // private readonly string sqlconnection = ConfigurationManager.ConnectionStrings["Lee_Creek"].ConnectionString;
        private readonly string sqlconnection = "";
       // public readonly string mysqlconnectionString = @"server=127.0.0.1;database=test;uid=renfb;pwd=123456;charset='gbk'";
        //获取Sql Server的连接数据库对象。SqlConnection

        //public OleDbConnection OpenConnection()
        //{

        //    OleDbConnection conn = new OleDbConnection(ConnectionString);
        //     conn.Open();
        //     return conn;
        //}

        //for windows
        public SQLiteConnection OpenConnection()
        {
            if (System.Web.HttpContext.Current != null)
            {
                _mdbpath = System.Web.HttpContext.Current.Server.MapPath(ConfigHelper.SitePath + ConfigHelper.DbConnection);
            }
            string ConnectionString = string.Format("Data Source={0};Pooling=true;FailIfMissing=false", _mdbpath);//for windows
            SQLiteConnection conn = new SQLiteConnection(ConnectionString);
            conn.Open();
            return conn;
        }
        public SQLiteConnection OpenConnection(string connectionString)
        {
            SQLiteConnection conn = new SQLiteConnection(connectionString);
            conn.Open();
            return conn;
        }

        //for mono
        //public SqliteConnection OpenConnection()// for sqlite & liunx
        //{
        //    // System.Web.HttpContext.Current.Response.Write(ConnectionString);
        //    // System.Web.HttpContext.Current.Response.End();
        //    SqliteConnection conn = new SqliteConnection(ConnectionString);
        //    conn.Open();
        //    return conn;
        //}

      
        public SqlConnection OpenConnectionSql()
        {
            SqlConnection connection = new SqlConnection(sqlconnection);
            connection.Open();
            return connection;
        }

        public SqlConnection OpenConnectionSql(string Sqlconnection)
        {
            SqlConnection connection = new SqlConnection(Sqlconnection);
            connection.Open();
            return connection;
        }

        /// <summary>
        /// 获取分页Sql for sqlite
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="colName"></param>
        /// <param name="colList"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageIndex"></param>
        /// <param name="orderBy"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public string GetPageSql(string tableName, string colName, string colList, int pageSize, int pageIndex, int orderBy, string condition)
        {
            string temp = string.Empty;
            string sql = string.Empty;
            if (string.IsNullOrEmpty(condition))
            {
                condition = " 1=1 ";
            }


            temp = "select {1} from {2} where {3} order by {4} {5} limit {0} OFFSET {6}";
            sql = string.Format(temp, pageSize, colList, tableName, condition, colName, orderBy == 1 ? "desc" : "asc", pageSize * (pageIndex - 1));

            //第一页
            if (pageIndex == 1)
            {
                temp = "select {1} from {2} where {3} order by {4} {5} limit {0}";
                sql = string.Format(temp, pageSize, colList, tableName, condition, colName, orderBy == 1 ? "desc" : "asc");
            }

            return sql;
        }
    }
}
