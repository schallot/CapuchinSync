using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileCopierFactory : IFileCopierFactory
    {
        private readonly IFileSystem _fileSystem;
        private readonly IPathUtility _pathUtility;

        public FileCopierFactory(IFileSystem fileSystem, IPathUtility pathUtility)
        {
            _fileSystem = fileSystem;
            _pathUtility = pathUtility;
        }

        public IFileCopier CreateFileCopier(string source, string destination)
        {
            return new FileCopier(_fileSystem, _pathUtility, source, destination);
        }
    }
}
