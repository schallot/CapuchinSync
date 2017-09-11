using System;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public string GetTimeString(DateTime time)
        {
            return time.ToLongTimeString();
        }

        public string GetDateString(DateTime time)
        {
            return time.ToLongDateString();
        }

        public DateTime Now => DateTime.Now;
    }
}
