using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace FlatMate.Module.Common.Extensions
{
    public static class LoggerExtensions
    {
        public static IDisposable LogInformationTimed<T>(this ILogger<T> logger, string message, params object[] args)
        {
            return new LogTimed<T>(logger, LogLevel.Information, message, args);
        }
    }

    public class LogTimed<T> : IDisposable
    {
        private readonly object[] _args;
        private readonly LogLevel _level;
        private readonly ILogger<T> _logger;
        private readonly string _message;
        private readonly Stopwatch _startNew;

        public LogTimed(ILogger<T> logger, LogLevel level, string message, params object[] args)
        {
            _logger = logger;
            _level = level;
            _message = message;
            _args = args;
            _startNew = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _startNew.Stop();
            _logger.Log(_level, _message + " in {duration}", _args.Append(_startNew.Elapsed).ToArray());
        }
    }
}