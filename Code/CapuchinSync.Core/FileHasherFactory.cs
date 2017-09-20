using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileHasherFactory : IFileHasherFactory
    {
        private readonly IHashUtility _hashUtility;
        private readonly IPathUtility _pathUtility;

        public FileHasherFactory(IHashUtility hashUtility, IPathUtility pathUtility)
        {
            _hashUtility = hashUtility;
            _pathUtility = pathUtility;
        }

        public IFileHasher CreateHasher(string filePath)
        {
            return new FileHasher(_hashUtility, _pathUtility, filePath);
        }
    }
}
