using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class HashDictionaryFactory : IHashDictionaryFactory
    {
        private readonly IHashUtility _hashUtility;
        private readonly string _rootDirectory;

        public HashDictionaryFactory(IHashUtility hashUtility, string rootDirectory)
        {
            _hashUtility = hashUtility;
            _rootDirectory = rootDirectory;
        }

        public IHashDictionaryEntry CreateHashEntry(string filePath)
        {
            return new HashDictionaryEntry(_hashUtility, _rootDirectory, filePath);
        }
    }
}
