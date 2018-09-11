using System;
using System.Text;

namespace ZibaobaoLib.Helpers
{
    public enum X1EventLogEntryType
    {
        Error = 1,
        Warning = 2,
        Information = 4,
        SuccessAudit = 8,
        Verbose = 16,
    }

    public interface IX1Log
    {
        void WriteEntry(string message, X1EventLogEntryType type, int eventId);
    }

    public delegate void Logging(int mode, string message);

    public class X1UnhandleException : Exception
    {
        public X1UnhandleException(Exception innerException) :
            base("unhandled exception", innerException)
        {
        }
    }

    public class X1LogHelper
    {
        static int _eventId;
        public static IX1Log X1ServiceEventLog { get; set; }
        public static int LogLevel = (int)X1EventLogEntryType.SuccessAudit;
        public static void Log(X1EventLogEntryType type, string info)
        {
            X1ServiceEventLog?.WriteEntry(info, type, _eventId++);
        }
        public static void Verbose(string info)
        {
            if (LogLevel >= (int)X1EventLogEntryType.Verbose)
            {
                Log(X1EventLogEntryType.Verbose, info);
            }
        }

        public static void Log(string info)
        {
            if (LogLevel >= (int)X1EventLogEntryType.SuccessAudit)
            {
                Log(X1EventLogEntryType.SuccessAudit, info);
            }
        }
        public static void Warning(string info)
        {
            if (LogLevel >= (int)X1EventLogEntryType.Warning)
            {
                Log(X1EventLogEntryType.Warning, info);
            }
        }
        public static void Info(string info)
        {
            if (LogLevel >= (int)X1EventLogEntryType.Information)
            {
                Log(X1EventLogEntryType.Information, info);
            }
        }

        public static string FlattenException(Exception exception)
        {
            var stringBuilder = new StringBuilder();

            while (exception != null)
            {
                stringBuilder.AppendLine(exception.Message);
                stringBuilder.AppendLine(exception.StackTrace);

                exception = exception.InnerException;
            }
            return stringBuilder.ToString();
        }
        public static void Error(string info)
        {
            Log(X1EventLogEntryType.Error, info);
        }
        public static void Exception(Exception ex)
        {
            Error(ex.Message + Environment.NewLine + FlattenException(ex));
        }

        public static void Log(int mode, string message)
        {
            if (mode <= (int)X1EventLogEntryType.Error)
            {
                Error(message);
            }
            else if (mode == (int)X1EventLogEntryType.Warning)
            {
                Warning(message);
            }
            else if (mode == (int)X1EventLogEntryType.Information)
            {
                Info(message);
            }
            else if (mode == (int)X1EventLogEntryType.SuccessAudit)
            {
                Log(message);
            }
            else
            {
                Verbose(message);
            }
        }

        public static Action<Exception> GlobalExceptionHandler { get; set; }

        //true, processed, false not processed
        public static bool HandlerGlobalException(Exception e, object info)
        {
            if (e is X1UnhandleException)
            {
                throw e.InnerException;
            }
            if (e != null)
            {
                if (info != null)
                {
                    Error(info.ToString());
                }
                Exception(e);
                if (GlobalExceptionHandler != null)
                {
                    GlobalExceptionHandler.Invoke(e);
                    return true;
                }
                return false;
            }
            string error = "Unknow error";
            if (info != null)
            {
                error = info.ToString();
            }
            Error(error);
            return false;
        }
    }
}