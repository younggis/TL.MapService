using log4net;
using log4net.Config;
using System;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace DAL.Common.LOG
{
    public class Log4Helper
    {
        private static log4net.ILog _log = null;
        private static object lockHelper = new object();

        private static log4net.ILog Log
        {
            get
            {
                if (null == _log)
                {
                    lock (lockHelper)
                    {
                        if (null == _log && System.AppDomain.CurrentDomain.BaseDirectory != null)
                        {
                            XmlConfigurator.Configure(new System.IO.FileInfo(System.IO.Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
                            _log = LogManager.GetLogger("Logging");
                        }
                    }
                }
                return _log;
            }
        }

        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        #region static void WriteLog(Type t, Exception ex)
        public static void WriteLog(Exception ex)
        {
            Error("Error", ex);
        }
        public static void WriteLog(string msg)
        {
            Error("Error", new Exception(msg));
        }
        public static void WriteLog(Type t, Exception ex)
        {
            Error("Error", ex);
        }


        public static void WriteLog(Type t, string msg)
        {
            Error(msg);
        }

        #endregion

        #region
        public static void Debug(object message, Exception exception)
        {
            Log.Debug(message, exception);
        }

        public static void Debug(object message)
        {
            Log.Debug(message);
        }

        public static void DebugFormat(IFormatProvider provider, string format, params object[] args)
        {
            Log.DebugFormat(provider, format, args);
        }

        public static void DebugFormat(string format, object arg0, object arg1, object arg2)
        {
            Log.DebugFormat(format, arg0, arg1, arg2);
        }

        public static void DebugFormat(string format, object arg0, object arg1)
        {
            Log.DebugFormat(format, arg0, arg1);
        }

        public static void DebugFormat(string format, object arg0)
        {
            Log.DebugFormat(format, arg0);
        }

        public static void DebugFormat(string format, params object[] args)
        {
            Log.DebugFormat(format, args);
        }

        public static void Error(object message, Exception exception)
        {
            if (Log != null)
            {
                Log.Error(message, exception);
            }
        }

        public static void Error(object message)
        {
            Log.Error(message);
        }

        public static void ErrorFormat(IFormatProvider provider, string format, params object[] args)
        {
            Log.ErrorFormat(provider, format, args);
        }

        public static void ErrorFormat(string format, object arg0, object arg1, object arg2)
        {
            Log.ErrorFormat(format, arg0, arg1, arg2);
        }

        public static void ErrorFormat(string format, object arg0, object arg1)
        {
            Log.ErrorFormat(format, arg0, arg1);
        }

        public static void ErrorFormat(string format, object arg0)
        {
            Log.ErrorFormat(format, arg0);
        }

        public static void ErrorFormat(string format, params object[] args)
        {
            Log.ErrorFormat(format, args);
        }

        public static void Fatal(object message, Exception exception)
        {
            Log.Fatal(message, exception);
        }

        public static void Fatal(object message)
        {
            Log.Fatal(message);
        }

        public static void FatalFormat(IFormatProvider provider, string format, params object[] args)
        {
            Log.FatalFormat(provider, format, args);
        }

        public static void FatalFormat(string format, object arg0, object arg1, object arg2)
        {
            Log.FatalFormat(format, arg0, arg1, arg2);
        }

        public static void FatalFormat(string format, object arg0, object arg1)
        {
            Log.FatalFormat(format, arg0, arg1);
        }

        public static void FatalFormat(string format, object arg0)
        {
            Log.FatalFormat(format, arg0);
        }

        public static void FatalFormat(string format, params object[] args)
        {
            Log.FatalFormat(format, args);
        }

        public static void Info(object message, Exception exception)
        {
            Log.Info(message, exception);
        }

        public static void Info(object message)
        {
            Log.Info(message);
        }

        public static void InfoFormat(IFormatProvider provider, string format, params object[] args)
        {
            Log.InfoFormat(provider, format, args);
        }

        public static void InfoFormat(string format, object arg0, object arg1, object arg2)
        {
            Log.InfoFormat(format, arg0, arg1, arg2);
        }

        public static void InfoFormat(string format, object arg0, object arg1)
        {
            Log.InfoFormat(format, arg0, arg1);
        }

        public static void InfoFormat(string format, object arg0)
        {
            Log.InfoFormat(format, arg0);
        }

        public static void InfoFormat(string format, params object[] args)
        {
            Log.InfoFormat(format, args);
        }

        public static bool IsDebugEnabled
        {
            get { return Log.IsDebugEnabled; }
        }

        public static bool IsErrorEnabled
        {
            get { return Log.IsErrorEnabled; }
        }

        public static bool IsFatalEnabled
        {
            get { return Log.IsFatalEnabled; }
        }

        public static bool IsInfoEnabled
        {
            get { return Log.IsInfoEnabled; }
        }

        public static bool IsWarnEnabled
        {
            get { return IsWarnEnabled; }
        }

        public static void Warn(object message, Exception exception)
        {
            Log.Warn(message, exception);
        }

        public static void Warn(object message)
        {
            Log.Warn(message);
        }

        public static void WarnFormat(IFormatProvider provider, string format, params object[] args)
        {
            Log.WarnFormat(provider, format, args);
        }

        public static void WarnFormat(string format, object arg0, object arg1, object arg2)
        {
            Log.WarnFormat(format, arg0, arg1, arg2);
        }

        public static void WarnFormat(string format, object arg0, object arg1)
        {
            Log.WarnFormat(format, arg0, arg1);
        }

        public static void WarnFormat(string format, object arg0)
        {
            Log.WarnFormat(format, arg0);
        }

        public static void WarnFormat(string format, params object[] args)
        {
            Log.WarnFormat(format, args);
        }
        #endregion

    }
}