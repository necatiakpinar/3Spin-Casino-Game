using Abstractions;
using UnityEngine;

namespace Loggers
{
    public class UnityLogger : BaseLogger
    {
        public override void Log(object message)
        {
            Debug.Log(message);
        }

        public override void LogError(object message)
        {
            Debug.LogError(message);
        }

        public override void LogWarning(object message)
        {
            Debug.LogWarning(message);
        }
    }
}