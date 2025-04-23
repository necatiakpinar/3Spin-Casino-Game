namespace Abstractions
{
    public abstract class BaseLogger
    {
        public abstract void Log(object message);
        public abstract void LogError(object message);
        public abstract void LogWarning(object message);
    }
}