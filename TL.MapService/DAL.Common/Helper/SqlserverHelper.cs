using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Threading.Tasks;
using DAL.Common.DB;
using System.Configuration;

namespace DAL.Common.Helper
{
    public class SqlserverHelper
    {
        //插入在线请求数
        public static int OnlineRequest(long numbers)
        {
            if (numbers == 0)
            {
                return 1;
            }
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO OnlineRequest([NUMBER],[DATETIME]) VALUES({0},FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))", numbers);
            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            int result = helper.insertAccessTable(sqlstr);
            return result;
        }
        //删除几天前所有记录
        public int DeleteOnline(int day)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("DELETE FROM OnlineRequest WHERE DateDiff('d',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))>={0}", day);
            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            int result = helper.insertAccessTable(sqlstr);
            return result;
        }


        //插入请求成功次数请求数
        public static int SuccessResponse(long numbers, string type)
        {
            if (numbers == 0)
            {
                return 1;
            }
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO SuccessResponse([NUMBER],[DATETIME],[SUCCESSTYPE]) VALUES ({0},FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'),'{1}')", numbers, type);
            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            int result = helper.insertAccessTable(sqlstr);
            return result;
        }

        //删除几天前所有记录
        public int DeleteSuccess(int day)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("DELETE FROM SuccessResponse WHERE DateDiff('d',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))>={0}", day);
            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            int result = helper.insertAccessTable(sqlstr);
            return result;
        }
        //插入请求成功次数请求数
        public static int FailResponse(long numbers, string type)
        {
            if (numbers == 0)
            {
                return 1;
            }
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("INSERT INTO FailResponse([NUMBER],[DATETIME],[ERRORTYPE]) VALUES ({0},FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'),'{1}')", numbers, type);
            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            int result = helper.insertAccessTable(sqlstr);
            return result;
        }
        //删除几天前所有记录
        public int DeleteFail(int day)
        {
            StringBuilder sql = new StringBuilder();
            sql.AppendFormat("DELETE FROM FailResponse WHERE DateDiff('d',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))>={0}", day);
            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            int result = helper.insertAccessTable(sqlstr);
            return result;
        }

        //按 分钟、小时、天、周统计请求次数
        public DataTable GetRequestNumber(string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                return new DataTable();
            }
            StringBuilder sql = new StringBuilder();
            if (type == "minute")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm:ss'),19) as requesttime FROM OnlineRequest WHERE DateDiff('s',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=60 GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm:ss'),19)");
            }
            else if (type == "hour")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm'),16) as requesttime FROM OnlineRequest WHERE DateDiff('n',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=60 GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm'),16)");
            }
            else if (type == "day")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh'),13) as requesttime FROM OnlineRequest WHERE DateDiff('h',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=24 GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh'),13)");
            }
            else if (type == "week")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd'),10) as requesttime FROM OnlineRequest WHERE DateDiff('d',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=7 GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd'),10)");
            }
            
            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            DataTable dtTable = helper.getAccessDataTable(sqlstr);
            if (dtTable == null || dtTable.Rows.Count <= 0)
            {
                return new DataTable();
            }
            else
            {
                return dtTable;
            }
        }

        //按 分钟、小时、天、周统计请求成功次数
        public DataTable GetSuccessResponseNumber(string type,string success)
        {
            string condition="";
            if (string.IsNullOrEmpty(type))
            {
                return new DataTable();
            }
            if (string.IsNullOrEmpty(success))
            {
                condition = " AND 1=1";
            }
            else
            {
                condition = " AND [SUCCESSTYPE]='"+success+"'";
            }
            StringBuilder sql = new StringBuilder();
            if (type == "minute")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm:ss'),19) as requesttime FROM SuccessResponse WHERE DateDiff('s',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=60{0} GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm:ss'),19)", condition);
            }
            else if (type == "hour")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm'),16) as requesttime FROM SuccessResponse WHERE DateDiff('n',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=60{0} GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm'),16)", condition);
            }
            else if (type == "day")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh'),13) as requesttime FROM SuccessResponse WHERE DateDiff('h',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=24{0} GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh'),13)", condition);
            }
            else if (type == "week")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd'),10) as requesttime FROM SuccessResponse WHERE DateDiff('d',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=7{0} GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd'),10)", condition);
            }

            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            DataTable dtTable = helper.getAccessDataTable(sqlstr);
            if (dtTable == null || dtTable.Rows.Count <= 0)
            {
                return new DataTable();
            }
            else
            {
                return dtTable;
            }
        }

        //按 分钟、小时、天、周统计请求成功次数
        public DataTable GetErrorResponseNumber(string type, string success)
        {
            string condition = "";
            if (string.IsNullOrEmpty(type))
            {
                return new DataTable();
            }
            if (string.IsNullOrEmpty(success))
            {
                condition = " AND 1=1";
            }
            else
            {
                condition = " AND [ERRORTYPE]='" + success + "'";
            }
            StringBuilder sql = new StringBuilder();
            if (type == "minute")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm:ss'),19) as requesttime FROM FailResponse WHERE DateDiff('s',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=60{0} GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm:ss'),19)", condition);
            }
            else if (type == "hour")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm'),16) as requesttime FROM FailResponse WHERE DateDiff('n',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=60{0} GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh:mm'),16)", condition);
            }
            else if (type == "day")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh'),13) as requesttime FROM FailResponse WHERE DateDiff('h',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=24{0} GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd hh'),13)", condition);
            }
            else if (type == "week")
            {
                sql.AppendFormat("SELECT SUM([NUMBER]) as requestcount,LEFT(FORMAT(DATETIME,'yyyy-mm-dd'),10) as requesttime FROM FailResponse WHERE DateDiff('d',[DATETIME], FORMAT(NOW(),'yyyy-mm-dd hh:mm:ss'))<=7{0} GROUP BY LEFT(FORMAT(DATETIME,'yyyy-mm-dd'),10)", condition);
            }

            string sqlstr = sql.ToString();
            commHelper helper = new commHelper();
            DataTable dtTable = helper.getAccessDataTable(sqlstr);
            if (dtTable == null || dtTable.Rows.Count <= 0)
            {
                return new DataTable();
            }
            else
            {
                return dtTable;
            }
        }



    }
}
