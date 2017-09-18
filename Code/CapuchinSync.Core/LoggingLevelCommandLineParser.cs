using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace CapuchinSync.Core
{
    public class LoggingLevelCommandLineParser : Loggable
    {
        private const string VerbosityPrefix = "VERBOSITY:";
        public bool IsVerbosityArgument(string arg)
        {
            if (arg == null) return false;
            arg = arg.Trim().ToUpperInvariant();
            return arg.StartsWith(VerbosityPrefix);
        }

        public List<string> SetLoggingLevelAndReturnNonLoggingArgs(IEnumerable<string> args)
        {
            var argArray = args.ToList();
            if (!argArray.Any(IsVerbosityArgument))
            {
                LogThreshold = LogEntry.LogSeverity.Info;
                Info("Defaulted logging verbosity threshold to Info.");
                return argArray;
            }
            LogThreshold = GetVerbosityArgument(argArray.First(IsVerbosityArgument));
            Info($"Set logging verbosity threshold to {LogThreshold}");
            return argArray.Where(x => !IsVerbosityArgument(x)).ToList();
        }

        private LogEntry.LogSeverity GetVerbosityArgument(string arg)
        {
            arg = arg.Trim().ToUpperInvariant();
            if (arg.StartsWith(VerbosityPrefix))
            {
                arg = arg.Substring(VerbosityPrefix.Length);
            }
            switch (arg)
            {
                case "TRACE":
                    return LogEntry.LogSeverity.Trace;
                case "DEBUG":
                    return LogEntry.LogSeverity.Debug;
                case "INFO":
                    return LogEntry.LogSeverity.Info;
                case "INFORMATION":
                    return LogEntry.LogSeverity.Info;
                case "WARNING":
                    return LogEntry.LogSeverity.Warning;
                case "WARN":
                    return LogEntry.LogSeverity.Warning;
                case "ERROR":
                    return LogEntry.LogSeverity.Error;
                case "FATAL":
                    return LogEntry.LogSeverity.Fatal;
            }
            throw new InvalidEnumArgumentException(nameof(arg));
        }

    }
}
