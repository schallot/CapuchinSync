using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class HashDictionaryFactory : IHashDictionaryFactory
    {
        private readonly IHashUtility _hashUtility;
        private readonly string _rootDirectory;
        private readonly IFileSystem _fileSystem;

        public HashDictionaryFactory(IFileSystem fileSystem, IHashUtility hashUtility, string rootDirectory)
        {
            _hashUtility = hashUtility;
            _rootDirectory = rootDirectory;
            _fileSystem = fileSystem;
        }

        public IHashDictionaryEntry CreateHashEntry(string filePath)
        {
            return new HashDictionaryEntry(_fileSystem, _hashUtility, _rootDirectory, filePath);
        }
    }
}
