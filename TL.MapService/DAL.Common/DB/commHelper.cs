using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace DAL.Common.DB
{
    public class commHelper
    {
        public DataTable getDataTable(string sqlStr)
        {
            DataTable dtTable = new DataTable();
            if (string.IsNullOrEmpty(sqlStr)) { return dtTable; }
            DBAsser dbassesser = new DBAsser();
            DataSet dtSet = dbassesser.ExecuteDataSet(sqlStr);
            dtTable = dtSet.Tables[0];
            return dtTable;
        }

        public int insertTable(string sqlStr)
        {
            if (string.IsNullOrEmpty(sqlStr)) { return 0; }
            DBAsser dbassesser = new DBAsser();
            int result = dbassesser.ExecuteNonQuery(sqlStr);
            return result;
        }
        public DataTable getAccessDataTable(string sqlStr)
        {
            DataTable dtTable = new DataTable();
            if (string.IsNullOrEmpty(sqlStr)) { return dtTable; }
            DataSet dtSet = DbHelperOleDb.Query(sqlStr);
            dtTable = dtSet.Tables[0];
            return dtTable;
        }

        public int insertAccessTable(string sqlStr)
        {
            if (string.IsNullOrEmpty(sqlStr)) { return 0; }
            int result = DbHelperOleDb.ExecuteSql(sqlStr);
            return result;
        }


        /// <summary>
        /// datatable 转实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static List<T> TableToEntity<T>(DataTable dt) where T : class,new()
        {
            Type type = typeof(T);
            List<T> list = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                PropertyInfo[] pArray = type.GetProperties();
                T entity = new T();
                foreach (PropertyInfo p in pArray)
                {
                    p.SetValue(entity, Convert.ToString(row[p.Name]), null);
                }
                list.Add(entity);
            }
            return list;
        }
    }
}
