using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Common.DB
{
    public enum DBEnum
    {
        //Oracle = 0,
        SqlServer = 1,
        Access=2
    }
    public class ConnCore
    {
        /// <summary>
        /// 连接字符串
        /// </summary>
        public string ConnectionString
        {
            get
            {
                string ConnectionString = ConfigurationManager.AppSettings["ConnectionString"];
                return ConnectionString;
            }
        }
    }
}
