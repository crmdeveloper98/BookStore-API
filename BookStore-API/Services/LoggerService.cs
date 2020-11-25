using BookStore_API.Contracts;
using NLog;

namespace BookStore_API.Services
{
    public class LoggerService : ILoggerService
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        public void LogInfo(string message) => Logger.Info(message);

        public void LogWarn(string message) => Logger.Warn(message);

        public void LogDebug(string message) => Logger.Debug(message);

        public void LogError(string message) => Logger.Error(message);
    }
}
