using System;
using System.Collections.Generic;

namespace CapuchinSync.Core.Interfaces
{
    public interface ILoggable
    {
        List<LogEntry> LogEntries {get;}

        void Trace(string message, Exception e = null);
        void Debug(string message, Exception e = null);
        void Info(string message, Exception e = null);
        void Warn(string message, Exception e = null);
        void Error(string message, Exception e = null);
        void Fatal(string message, Exception e = null);
    }
}
