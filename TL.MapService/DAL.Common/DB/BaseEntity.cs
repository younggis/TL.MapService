using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Common.DB
{
    public interface IBaseEntity
    {
        string ConnectionString { get; set; }
        System.Data.DataSet ExecuteDataSet(string strSql);
        int ExecuteNonQuery(string strSql);
        object ExecuteScalar(string strSql);
    }
}
