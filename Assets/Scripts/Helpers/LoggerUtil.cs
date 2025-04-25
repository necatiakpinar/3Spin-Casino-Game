using System.Collections.Generic;
using Abstractions;
using Loggers;

namespace Helpers
{
    public static class LoggerUtil
    {
        private static readonly List<BaseLogger> _loggers = new()
        {
            new UnityLogger()
        };

        public static void Log(object message)
        {
            BaseLogger logger;
            for (var i = 0; i < _loggers.Count; i++)
            {
                logger = _loggers[i];
                logger.Log(message);
            }
        }

        public static void LogError(object message)
        {
            BaseLogger logger;
            for (var i = 0; i < _loggers.Count; i++)
            {
                logger = _loggers[i];
                logger.LogError(message);
            }
        }

        public static void LogWarning(object message)
        {
            BaseLogger logger;
            for (var i = 0; i < _loggers.Count; i++)
            {
                logger = _loggers[i];
                logger.LogWarning(message);
            }
        }
    }
}