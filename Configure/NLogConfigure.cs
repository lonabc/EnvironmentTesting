using NLog;

namespace TempModbusProject.Configure
{
    public class NLogConfigure
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public static void WirteLogTest(string content)
        {
            logger.Info(content);
        }


        public static void WirteLogError(string content)
        {
            logger.Error(content);
        }

    }
}
