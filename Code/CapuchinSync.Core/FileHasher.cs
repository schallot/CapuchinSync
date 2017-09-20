using System;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileHasher : Loggable, IFileHasher
    {
        public FileHasher(IHashUtility hashUtility, IPathUtility pathUtility, string path)
        {
            if(hashUtility == null) throw new ArgumentNullException(nameof(hashUtility));
            if(pathUtility == null) throw new ArgumentException(nameof(pathUtility));
            _pathUtility = pathUtility;
            FullPath = path;
            Trace($"Creating instance of {nameof(FileHasher)} with hash <{hashUtility.HashName}> for path {path}.");
            try
            {
                Hash = hashUtility.GetHashFromFile(path);
            }
            catch (Exception e)
            {
                Warn($"Failed to calculate hash for file {path} - Defaulting to unknown hash.", e);
                Hash = HashDictionaryEntry.UnknownHash;
            }
            Info($"Calculated hash for file {path} as {Hash}.");
        }

        private readonly IPathUtility _pathUtility;
        public string FullPath { get; }
        public string Hash { get; }



        public string GetDictionaryEntryString(string rootDirectory)
        {
            var relativePath = _pathUtility.CalculateRelativePath(rootDirectory, FullPath);
            return $"{Hash}{HashDictionaryEntry.Delimiter}{relativePath}";
        }
    }
}
