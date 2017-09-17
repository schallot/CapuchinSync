using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapuchinSync.Core.Interfaces
{
    public interface ILogViewer
    {
        void ViewLogs(IEnumerable<ILogEntry> logs);
    }
}
