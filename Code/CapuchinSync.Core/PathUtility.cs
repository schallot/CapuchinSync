using System.IO;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class PathUtility : IPathUtility
    {
        public string GetParentDirectoryFromPath(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string Combine(string rootedPath, string pathFragment)
        {
            return Path.Combine(rootedPath, pathFragment);
        }

        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }
    }
}
