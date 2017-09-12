using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileCopierFactory : IFileCopierFactory
    {
        public IFileCopier CreateFileCopier(IFileSystem fileSystem, IPathUtility pathUtility, string source, string destination)
        {
            return new FileCopier(fileSystem, pathUtility, source, destination);
        }
    }
}
