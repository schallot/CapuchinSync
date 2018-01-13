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
            return time.ToString("dddd, MMMM d, yyyy");
        }

        public DateTime Now => DateTime.Now;
    }
}
