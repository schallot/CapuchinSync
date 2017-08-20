using System;

namespace CapuchinSync.Core.Interfaces
{
    public interface ILogEntry
    {
        DateTime EntryDate { get; }
        Type LogSourceType { get; }
        string Message { get; }
        LogEntry.LogSeverity Severity { get; }
    }
}