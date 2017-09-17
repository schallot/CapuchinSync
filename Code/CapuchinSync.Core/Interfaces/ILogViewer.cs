using System.Collections.Generic;

namespace CapuchinSync.Core.Interfaces
{
    public interface ILogViewer
    {
        void ViewLogs(IEnumerable<ILogEntry> logs);
    }
}
