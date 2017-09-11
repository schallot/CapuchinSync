using System;

namespace CapuchinSync.Core.Interfaces
{
    public interface IDateTimeProvider
    {
        string GetTimeString(DateTime time);
        string GetDateString(DateTime time);
        DateTime Now { get; }
    }
}
