using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileHasherFactory : IFileHasherFactory
    {
        private readonly IHashUtility _hashUtility;
        private readonly string _rootDir;
        private readonly IFileSystem _fileSystem;

        public FileHasherFactory(IFileSystem fileSystem, IHashUtility hashUtility, string rootDir)
        {
            _hashUtility = hashUtility;
            _rootDir = rootDir;
            _fileSystem = fileSystem;
        }

        public IFileHasher CreateHasher(string filePath)
        {
            return new FileHasher(_fileSystem, _hashUtility, _rootDir, filePath);
        }
    }
}
