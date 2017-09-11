using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileHasherFactory : IFileHasherFactory
    {
        private readonly IHashUtility _hashUtility;
        private readonly string _rootDir;

        public FileHasherFactory(IHashUtility hashUtility, string rootDir)
        {
            _hashUtility = hashUtility;
            _rootDir = rootDir;
        }

        public IFileHasher CreateHasher(string filePath)
        {
            return new FileHasher(_hashUtility, _rootDir, filePath);
        }
    }
}
