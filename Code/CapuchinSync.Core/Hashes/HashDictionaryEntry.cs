using System;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.Hashes
{
    public class HashDictionaryEntry : Loggable
    {
        public string ErrorMessage { get; }
        /// <summary>
        /// The delimiter that will be used to separate entires in a hash dictionary line.
        /// </summary>
        public const string Delimiter = "\t";

        private static string[] SplitDelimiters = {Delimiter};

        private const int HashIndex = 0;
        private const int FilePathIndex = 1;

        /// <summary>
        /// The string that will be used to indicate that a hash could not be computed for a file.
        /// </summary>
        private const string UnknownHash = "?";

        public HashDictionaryEntry(IHashUtility hashUtility, string rootDirectory, string hashFileLine)
        {
            if(hashUtility == null) throw new ArgumentNullException(nameof(hashUtility));
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

            var split = hashFileLine.Split(SplitDelimiters, StringSplitOptions.None);
            const int minimumEntryLength = 2;
            if (split.Length < minimumEntryLength)
            {
                ErrorMessage = $"Hash dictionary line <{hashFileLine}> has an insufficient number of entries (found {split.Length}, need {minimumEntryLength})";
                Warn(ErrorMessage);
                IsValid = false;
                return;
            }
            
            Hash = split[HashIndex].ToUpperInvariant();
            RelativePath = split[FilePathIndex].Trim();
            if (RelativePath.StartsWith("/") || RelativePath.StartsWith("\\"))
            {
                RelativePath = RelativePath.Substring(1);
            }

            if (UnknownHash.Equals(Hash, StringComparison.InvariantCultureIgnoreCase))
            {
                ErrorMessage = $"Hash dictionary line <{hashFileLine}> has a hash that is too short (is {Hash.Length} characters, expected {hashUtility.HashLength})";
                // We'll treat this as a warning, but we'll continue on and just accept that we'll have to copy this file without the benefit of a hash.
                Warn(ErrorMessage);
            }
            else
            {
                if (Hash.Length < hashUtility.HashLength)
                {
                    ErrorMessage = $"Hash dictionary line <{hashFileLine}> has an insufficient number of entries (found {split.Length}, need {minimumEntryLength})";
                    Warn(ErrorMessage);
                    IsValid = false;
                    return;
                }

                foreach (var c in Hash)
                {
                    if (!c.IsHex())
                    {
                        ErrorMessage =
                            $"Hash <{Hash}> in hash dictionary for folder {RootDirectory} contains non-hex character {c}.";
                        Warn(ErrorMessage);
                        IsValid = false;
                        return;
                    }
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
