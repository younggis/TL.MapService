using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Common.DB
{
    public class DBAsser : IBaseEntity
    {
        private string _ConnectionString;
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                _ConnectionString = value;
            }
        }

        public IBaseEntity m_DBAssesser { get; set; }

        /// <summary>
        /// 数据库类型，目前只支持SQL Server
        /// </summary>
        /// <param name="DBType">数据库类型</param>
        /// <param name="connString">数据库连接信息，为null，为系统级数据库连接</param>
        public DBAsser(DBEnum DBType = DBEnum.SqlServer, string connString = null)
        {
            ConnectionString = connString;
            if (DBType == DBEnum.SqlServer)//暂时只支持SQL Server数据库
                m_DBAssesser = new SqlServerAssesser(ConnectionString);
        }

        /// <summary>
        /// 执行返回dataset数据
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public System.Data.DataSet ExecuteDataSet(string strSql)
        {
            return m_DBAssesser.ExecuteDataSet(strSql);
        }

        /// <summary>
        /// 直接执行SQL语句
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public int ExecuteNonQuery(string strSql)
        {
            
            return m_DBAssesser.ExecuteNonQuery(strSql);
        }
        /// <summary>
        /// 直接执行SQL语句
        /// </summary>
        /// <param name="strSql"></param>
        /// <returns></returns>
        public object ExecuteScalar(string strSql)
        {
            return m_DBAssesser.ExecuteScalar(strSql);
        }

    }
}
