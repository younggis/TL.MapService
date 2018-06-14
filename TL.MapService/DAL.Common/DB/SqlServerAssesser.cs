using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Common.DB
{
    public class SqlServerAssesser : IBaseEntity
    {
        private SqlConnection _conn;
        /// <summary>
        /// 连接数据库实例
        /// </summary>
        public string ConnectionString { get; set; }

        public SqlServerAssesser(string connString = null)
        {
            if (string.IsNullOrEmpty(connString))
            {
                ConnCore conncore = new ConnCore();
                ConnectionString = conncore.ConnectionString;
            }
        }
        /// <summary>
        /// 打开连接
        /// </summary>
        public void OpenConnection()
        {
            if (_conn == null)
            { _conn = new SqlConnection(ConnectionString); }
            if (_conn.State == ConnectionState.Closed)
            { _conn.Open(); }
        }
        /// <summary>
        /// 关闭连接
        /// </summary>
        public void CloseConnection()
        {
            if (_conn != null && _conn.State != ConnectionState.Closed)
            { _conn.Close(); }
        }


        public int ExecuteNonQuery(string strSql)
        {
            int row = -1;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(strSql, _conn);
                row = cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "执行语句：" + strSql + "，数据库：" + _conn.DataSource, ex);
            }
            finally
            {
                CloseConnection();
            }
            return row;
        }

        public object ExecuteScalar(string strSql)
        {
            object result = null;
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(strSql, _conn);
                result = cmd.ExecuteScalar();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "执行语句：" + strSql + "，数据库：" + _conn.DataSource, ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

        public System.Data.DataSet ExecuteDataSet(string strSql)
        {
            DataSet result = new DataSet();
            try
            {
                OpenConnection();
                SqlCommand cmd = new SqlCommand(strSql, _conn);
                SqlDataAdapter adapter = new SqlDataAdapter(cmd);
                adapter.Fill(result);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + "执行语句：" + strSql + "，数据库：" + _conn.DataSource, ex);
            }
            finally
            {
                CloseConnection();
            }
            return result;
        }

    }
}
