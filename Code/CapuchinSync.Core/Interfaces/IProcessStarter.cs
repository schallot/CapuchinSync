using System.Diagnostics;

namespace CapuchinSync.Core.Interfaces
{
    public interface IProcessStarter
    {
        Process Start(string pathToExecutable, string arguments);
    }
}
