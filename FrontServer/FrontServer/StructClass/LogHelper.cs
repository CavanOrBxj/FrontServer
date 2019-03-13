using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace FrontServer
{
    public class LogHelper
    {
        /// <summary>
        /// 输出日志到Log4Net
        /// </summary>
        /// <param name="t"></param>
        /// <param name="ex"></param>
        #region static void WriteLog(Type t, Exception ex)

        public static void WriteLog(Type t, Exception ex)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            log.Error("Error", ex);
        }

        #endregion

        /// <summary>
        /// 输出日志到Log4Net  Error  Debug   Fatal  Info  Warn
        /// </summary>
        /// <param name="t"></param>
        /// <param name="msg"></param>
        /// <param name="type">  </param>
        #region static void WriteLog(Type t, string msg)

        public static void WriteLog(Type t, string msg,string type)
        {
            log4net.ILog log = log4net.LogManager.GetLogger(t);
            switch (type)
            {
                case "Error":
                    log.Error(msg);
                    break;

                case "Debug":
                    log.Debug(msg);
                    break;

                case "Fatal":
                    log.Fatal(msg);
                    break;

                case "Info":
                    log.Info(msg);
                    break;

                case "Warn":
                    log.Warn(msg);
                    break;
            }

            log.Error(msg);
        }

        #endregion


    }
}
