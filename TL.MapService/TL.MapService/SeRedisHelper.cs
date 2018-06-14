using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices;
using System.Linq;
using System.Web;
using System.Threading;
using System.Threading.Tasks;

namespace TL.MapService
{
    public class SeRedisHelper
    {
        private static readonly object Locker = new object();

        private static ConnectionMultiplexer _redis;
        private static ConnectionMultiplexer _slaveredis;

        public static ConnectionMultiplexer Manager
        {
            get
            {
                if (_redis == null)
                {
                    lock (Locker)
                    {
                        if (_redis != null) return _redis;
                        _redis = GetManager();
                        ThreadPool.SetMinThreads(128, 128);
                        return _redis;
                    }
                }
                return _redis;
            }
        }

        private static ConnectionMultiplexer GetManager()
        {
            string redisrewr = ConfigurationManager.AppSettings["redisrewr"];//是否读写分离
            if (redisrewr == "true" || redisrewr == "TRUE")
            {
                return ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["masterconn"]);    
            }
            else
            {
                return ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["rediscon"]);    
            }  
        }
        //如果读写分离，主写从存
        public static ConnectionMultiplexer GetSlaveManager()
        {
            if (_slaveredis != null)
            {
                return _slaveredis;
            }
            else
            {
                string redisrewr = ConfigurationManager.AppSettings["redisrewr"];//是否读写分离
                if (redisrewr == "true" || redisrewr == "TRUE")
                {
                    return _slaveredis=ConnectionMultiplexer.Connect(ConfigurationManager.AppSettings["slaveconn"]);
                }
                else
                {
                    return _slaveredis=_redis;
                }  
            }
        }
        #region string类型
        /// <summary>
        /// 根据Key获取值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns>System.String.</returns>
        public static string StringGet(string key)
        {
            try
            {
                return GetSlaveManager().GetDatabase().StringGet(key);
            }
            catch (Exception e)
            {
                Log4Helper.WriteLog(e);
                return null;
            }
        }
        /// <summary>
        /// 单条存值
        /// </summary>
        /// <param name="key">key</param>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static void StringSet(string key, string value)
        {
         
            try
            {
                Manager.GetDatabase().StringSet(key, value);
            }
            catch (Exception e)
            {
                Log4Helper.WriteLog(e);
            }

        }

        /// <summary>
        /// 异步设置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static async Task SetAsync(string key, string value)
        {
            await Manager.GetDatabase().StringSetAsync(key, value);
        }

        /// <summary>
        /// 根据key获取缓存对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static async Task<object> GetAsync(string key)
        {
            object value = await GetSlaveManager().GetDatabase().StringGetAsync(key);
            return value;
        }


        /// <summary>
        /// 是否存在key
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool HasKey(string key)
        {
            try
            {
                return GetSlaveManager().GetDatabase().KeyExists(key);
            }
            catch (Exception e)
            {
                Log4Helper.WriteLog(e);
                return false;
            }
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static bool KeyDelete(string key)
        {
            try
            {
                return Manager.GetDatabase().KeyDelete(key);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="keys">键</param>
        /// <returns></returns>
        public static long KeyDelete(List<string> keys)
        {
            try
            {
                return Manager.GetDatabase().KeyDelete(ConvertRedisKeys(keys,null));
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static RedisKey[] ConvertRedisKeys(List<string> redisKeys, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                return redisKeys.Select(redisKey => (RedisKey)redisKey).ToArray();
            }
            else
            {
                return redisKeys.Select(redisKey => (RedisKey)(prefix + ":" + redisKey)).ToArray();
            }
        }


        #endregion
    }
}