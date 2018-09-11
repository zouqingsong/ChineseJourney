using System.IO;
using Serilog;
using ZibaobaoLib;
using ZibaobaoLib.Helpers;

namespace SamplyGame.Helpers
{
    public class X1LocalLogger : IX1Log
    {
        static X1LocalLogger()
        {
            InitLog();
        }

        public static void InitLog(string seriServer = "")
        {
            var logConfig = new LoggerConfiguration();
            logConfig = logConfig.MinimumLevel.Verbose()
                .WriteTo.Trace()
                .WriteTo.File(
                    Path.Combine(new DiskStorage().LogPath,
                        ZibaobaoLibContext.Instance.AppName + "-" + ZibaobaoLibContext.Instance.Platform + "-log-.txt"),
                    rollingInterval:RollingInterval.Day,
                    fileSizeLimitBytes:20*1024*1024);
#if !(WINDOWS_UWP || __IOS__ || __ANDROID__)
            logConfig = logConfig.WriteTo.Console();
#endif
            Serilog.Log.Logger = logConfig.CreateLogger();
        }
        public void WriteEntry(string message, X1EventLogEntryType type, int eventID)
        {
            switch (type)
            {
                case X1EventLogEntryType.Error:
                    Serilog.Log.Error(message);
                    break;
                case X1EventLogEntryType.Warning:
                    Serilog.Log.Warning(message);
                    break;
                case X1EventLogEntryType.Information:
                    Serilog.Log.Information(message);
                    break;
                case X1EventLogEntryType.SuccessAudit:
                    Serilog.Log.Debug(message);
                    break;
                case X1EventLogEntryType.Verbose:
                    Serilog.Log.Verbose(message);
                    break;
            }
        }

    }
}