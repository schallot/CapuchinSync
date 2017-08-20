﻿using System;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{

    public class LogEntry : ILogEntry
    {
        public Type LogSourceType { get; }
        public enum LogSeverity
        {
            Debug = 0,
            Info,
            Warning,
            Error,
            Fatal
        }

        public LogEntry(Type logSourceType, LogSeverity severity, string message, Exception e = null)
        {
            LogSourceType = logSourceType;
            Severity = severity;
            Message = message;
            if (e != null)
            {
                Message = $"{Message}: {e}";
            }
        }

        public DateTime EntryDate { get; } = DateTime.Now;
        public string Message { get; }
        public LogSeverity Severity { get; }

        public override string ToString()
        {
            return $"{Severity.ToString().ToUpperInvariant()}: {EntryDate.ToShortDateString()}@{EntryDate.ToShortTimeString()}, {LogSourceType.Name}: {Message}";
        }
    }
}
