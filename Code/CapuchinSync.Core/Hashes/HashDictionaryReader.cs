using System;
using System.Linq;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.Hashes
{
    public class HashDictionaryReader : Loggable
    {
        public static class ErrorCodes
        {
            public const int HashDictionaryNotFound = 766;
            public const int CouldNotReadFileCountFromFirstLine = 767;
            public const int UnexpectedNumberOfHashesInFile = 768;
            public const int InvalidHashDictionaryLine = 769;
        }

        private readonly string _dictionaryFile;
        private readonly string _rootDirectory;
        private readonly IFileSystem _fileSystem;
        private readonly IHashUtility _hashUtility;
        public int ErrorCode;

        public HashDictionaryReader(IHashUtility hash, string rootDirectory, IFileSystem fileSystem, IPathUtility pathUtility)
        {
            if(pathUtility == null) throw new ArgumentNullException(nameof(pathUtility));
            if(hash == null) throw new ArgumentNullException(nameof(hash));
            _hashUtility = hash;
            _rootDirectory = rootDirectory;
            _fileSystem = fileSystem;
            _dictionaryFile = pathUtility.Combine(rootDirectory, Constants.HashFileName);
            Info($"Creating hash dictionary reader for file {Constants.HashFileName} in directory {rootDirectory}");
        }

        public HashDictionary Read()
        {
            if (!_fileSystem.DoesFileExist(_dictionaryFile))
            {
                Error($"No hash file exists at {_dictionaryFile}.");
                ErrorCode = ErrorCodes.HashDictionaryNotFound;
                return null;
            }
            var hashFileContents = _fileSystem.ReadAllLines(_dictionaryFile).Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            var firstLine = hashFileContents.First();
            hashFileContents.RemoveAt(0);
            var countText = firstLine.Split(' ').FirstOrDefault(x => !string.IsNullOrWhiteSpace(x));
            int expectedCount;
            if (!int.TryParse(countText, out expectedCount))
            {
                Error($"Failed to parse the file count from the first line of {_dictionaryFile}: {firstLine}.  " +
                      "The file is not properly formatted.");
                ErrorCode = ErrorCodes.CouldNotReadFileCountFromFirstLine;
                return null;
            }
            if (hashFileContents.Count != expectedCount)
            {
                Error($"ExpectedHash file {_dictionaryFile} declares {expectedCount} files, but has {hashFileContents.Count} entries.");
                ErrorCode = ErrorCodes.UnexpectedNumberOfHashesInFile;
                return null;
            }

            var dictionary = new HashDictionary()
            {
                FilePath = _dictionaryFile
            };

            dictionary.Entries.AddRange(hashFileContents.Select(x => new HashDictionaryEntry(_hashUtility,_rootDirectory,x)));
            var firstInvalidLine = dictionary.Entries.FirstOrDefault(x => !x.IsValid);
            if (firstInvalidLine != null)
            {
                Error($"Hash dictionary file {_dictionaryFile} contains an invalid hash line: {firstInvalidLine}\r\n\t{firstInvalidLine.ErrorMessage}");
                ErrorCode = ErrorCodes.InvalidHashDictionaryLine;
                return null;
            }
            
            return dictionary;
        }
    }
}
