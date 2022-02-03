namespace NetProc
{
    public interface ILogger
    {
        void Log(string text);
        void Log(string text, LogLevel logLevel = LogLevel.Info);
    }
}
