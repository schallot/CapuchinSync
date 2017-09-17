using System.Diagnostics;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class ProcessStarter : IProcessStarter
    {
        public Process Start(string pathToExecutable, string arguments)
        {
            return Process.Start(pathToExecutable, arguments);
        }
    }
}
