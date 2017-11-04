using NLog;
using System.Runtime.CompilerServices;

namespace ImageEvaluatorInterfaces.BaseClasses
{
    public static class LoggerExtension
    {
        public static void TraceLog(this ILogger logger, string message, string className, [CallerMemberName] string funcName = "")
        {
            logger.Trace($"{className}-{funcName} {message}");
        }

        public static void DebugLog(this ILogger logger, string message, string className, [CallerMemberName] string funcName = "")
        {
            logger.Debug($"{className}-{funcName} {message}");
        }

        public static void InfoLog(this ILogger logger, string message, string className, [CallerMemberName] string funcName = "")
        {
            logger.Info($"{className}-{funcName} {message}");
        }

        public static void WarnLog(this ILogger logger, string message, string className, [CallerMemberName] string funcName = "")
        {
            logger.Warn($"{className}-{funcName} {message}");
        }

        public static void ErrorLog(this ILogger logger, string message, string className, [CallerMemberName] string funcName = "")
        {
            logger.Error($" {className}-{funcName}: {message}");
        }

        public static void FatalLog(this ILogger logger, string message, string className, [CallerMemberName] string funcName = "")
        {
            logger.Fatal($"{className}-{funcName} {message}");
        }

    }
}
