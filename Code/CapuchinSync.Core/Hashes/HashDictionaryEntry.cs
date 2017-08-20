using System;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.Hashes
{
    public class HashDictionaryEntry : Loggable
    {
        public string ErrorMessage { get; }
        public HashDictionaryEntry(IHashUtility hash, string rootDirectory, string hashFileLine)
        {
            if(hash == null) throw new ArgumentNullException(nameof(hash));
            _dictionaryLine = hashFileLine;
            RootDirectory = rootDirectory;
            Debug($"Parsing hash dictionary line <{hashFileLine}> with root directory <{RootDirectory}>.");

            IsValid = true;
            if (string.IsNullOrWhiteSpace(hashFileLine))
            {
                ErrorMessage = $"Cannot process blank hash file line in hash file in {RootDirectory}";
                Warn(ErrorMessage);
                IsValid = false;
                return;
            }
            hashFileLine = hashFileLine.Trim();

            // There's got to be enough length for a hash and at least a short file name
            if (hashFileLine.Length < hash.HashLength + 3)
            {
                ErrorMessage = $"Hash file line <{hashFileLine}> is not long enough to be valid.";
                Error(ErrorMessage);
                IsValid = false;
                return;
            }

            Hash = hashFileLine.Substring(0, hash.HashLength).ToUpperInvariant();
            RelativePath = hashFileLine.Substring(hash.HashLength + 1).Trim();
            if (RelativePath.StartsWith("/") || RelativePath.StartsWith("\\"))
            {
                RelativePath = RelativePath.Substring(1);
            }

            foreach (var c in Hash)
            {
                if (!c.IsHex())
                {
                    ErrorMessage = $"Hash <{Hash}> in hash dictionary for folder {RootDirectory} contains non-hex character {c}.";
                    Warn(ErrorMessage);
                    IsValid = false;
                    return;
                }
            }
            Debug($"Created Hash Dictionary Entry in root directory <{RootDirectory}> with relative path" +
                  $"<{RelativePath}> and hash <{Hash}>.");

        }

        private readonly string _dictionaryLine;
        public string RootDirectory { get; }
        public string RelativePath { get; }
        public string Hash { get; }

        public bool IsValid { get; }

        public override string ToString()
        {
            return _dictionaryLine;
        }
    }
}
