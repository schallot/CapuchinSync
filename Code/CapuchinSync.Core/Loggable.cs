using System;
using System.Collections.Generic;
using System.Linq;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public abstract class Loggable : ILoggable
    {
        public static List<ILogEntry> AllLogEntries { get; } = new List<ILogEntry>();

        public static void WriteAllLogEntriesToFile(string path, IFileSystem fileSystem)
        {
            fileSystem.WriteAllLinesAsUtf8TextFile(path, AllLogEntries.OrderBy(x => x.EntryDate).Select(x => x.FormattedLogLine));
        }
        
        public List<LogEntry> LogEntries { get; } = new List<LogEntry>();

        public void Trace(string message, Exception e = null)
        {
            CreateEntry(message, LogEntry.LogSeverity.Trace, e);
        }

        public void Debug(string message, Exception e = null)
        {
            CreateEntry(message, LogEntry.LogSeverity.Debug, e);
        }

        public void Info(string message, Exception e = null)
        {
            CreateEntry(message, LogEntry.LogSeverity.Info, e);
        }

        public void Warn(string message, Exception e = null)
        {
            CreateEntry(message, LogEntry.LogSeverity.Warning, e);
        }

        public void Error(string message, Exception e = null)
        {
            CreateEntry(message, LogEntry.LogSeverity.Error, e);
        }

        public void Fatal(string message, Exception e = null)
        {
            CreateEntry(message, LogEntry.LogSeverity.Fatal);
        }

        private void CreateEntry(string message, LogEntry.LogSeverity severity, Exception e = null)
        {
            var entry = new LogEntry(GetType(),severity, message, e);
            LogEntries.Add(entry);
            AllLogEntries.Add(entry);
            WriteToConsole(entry);
        }

        private void WriteToConsole(LogEntry entry)
        {
            switch (entry.Severity)
            {
                case LogEntry.LogSeverity.Trace:
                    WriteToConsole($"{entry.FormattedLogLine}", ConsoleColor.DarkGray, DefaultBackground);
                    break;
                case LogEntry.LogSeverity.Debug:
                    WriteToConsole($"{entry.FormattedLogLine}", ConsoleColor.Gray, DefaultBackground);
                    break;
                case LogEntry.LogSeverity.Info:
                    WriteToConsole($"{entry.FormattedLogLine}", ConsoleColor.White, DefaultBackground);
                    break;
                case LogEntry.LogSeverity.Warning:
                    WriteToConsole($"{entry.FormattedLogLine}", ConsoleColor.DarkYellow, DefaultBackground);
                    break;
                case LogEntry.LogSeverity.Error:
                    WriteToConsole($"{entry.FormattedLogLine}", ConsoleColor.Red, DefaultBackground);
                    break;
                case LogEntry.LogSeverity.Fatal:
                    WriteToConsole($"{entry.FormattedLogLine}", ConsoleColor.Black, ConsoleColor.Red);
                    break;
            }
        }

        private static ConsoleColor DefaultForeground { get; } = Console.ForegroundColor;

        private static ConsoleColor DefaultBackground { get; } = Console.BackgroundColor;

        /// <summary>
        /// An object that we'll use to indicate when a thread has exclusive access to the console.
        /// Otherwise, two asynchronous processes can clobber each other's colorization of text.
        /// </summary>
        private static readonly object ConsoleWriterLock = new object();

        private void WriteToConsole(string message, ConsoleColor foregroundColor, ConsoleColor backgroundColor)
        {
            // https://stackoverflow.com/questions/1522936/how-do-i-lock-the-console-across-threads-in-c-net
            lock (ConsoleWriterLock)
            {
                Console.ForegroundColor = foregroundColor;
                Console.BackgroundColor = backgroundColor;
                Console.WriteLine(message);
                Console.ForegroundColor = DefaultForeground;
                Console.BackgroundColor = DefaultBackground;
            }
        }
    }
}
