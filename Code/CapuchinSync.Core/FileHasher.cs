using System;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileHasher : Loggable, IFileHasher
    {
        public FileHasher(IHashUtility hashUtility, string rootDirectory, string path)
        {
            if(hashUtility == null) throw new ArgumentNullException(nameof(hashUtility));
            Trace($"Creating instance of {nameof(FileHasher)} with hash <{hashUtility.HashName}> for path {path} in directory {rootDirectory}.");
            RelativePath = path.Substring(rootDirectory.Length);
            if (RelativePath.StartsWith("/") || RelativePath.StartsWith("\\"))
            {
                RelativePath = RelativePath.Substring(1);
            }
            Debug($"Relative path for file {path} calculated as {RelativePath} in directory {rootDirectory}.");
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

        public string RelativePath { get; }

        public string Hash { get; }

        public override string ToString()
        {
            return $"{Hash}{HashDictionaryEntry.Delimiter}{RelativePath}";
        }
    }
}
