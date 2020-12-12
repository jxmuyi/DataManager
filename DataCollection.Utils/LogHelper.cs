using System;
using System.Collections.Generic;
using System.Text;
using log4net;
using System.IO;

namespace DataCollection.Utils
{
    public class LogHelper
    {
        private readonly static ILog _log;
        static LogHelper()
        {
            var logFilePath = AppDomain.CurrentDomain.BaseDirectory + @"\Log4net.config";
            FileInfo fileInfo = new FileInfo(logFilePath);
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            if(_log is null)
            {
                _log = LogManager.GetLogger("DataCollection");
            }
        }

        public static void DebugConsole(string msg)
        {
            Console.WriteLine($"{DateTime.Now}  Debug:{msg}");
        }

        public static void Info(string msg)
        {
            _log.Info($@"{DateTime.Now} info: {msg}");
            Console.WriteLine($"{DateTime.Now}  Info:{msg}");
        }

        public static void Error(Exception exception)
        {
            _log.Error($@"{DateTime.Now} error: {exception}");
            Console.WriteLine($"{DateTime.Now}  error:{exception.Message}");
        }


    }
}
