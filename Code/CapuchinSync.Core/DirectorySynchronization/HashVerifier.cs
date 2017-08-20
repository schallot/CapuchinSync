using System;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.DirectorySynchronization
{
    public class HashVerifier : Loggable
    {
        public string ErrorMessage { get; } = "";
        private VerificationStatus _status = VerificationStatus.TargetFileNotRead;
        private string _calculatedHash;
        private readonly IFileSystem _fileSystem;
        private readonly IHashUtility _hashUtility;
        public HashDictionaryEntry HashEntry { get; }

        public enum VerificationStatus
        {
            TargetFileMatchesHash,
            TargetFileDoesNotMatchHash,
            TargetFileDoesNotExist,
            TargetFileNotRead
        }

        public HashVerifier(HashDictionaryEntry entry, string targetDirectory, 
            IFileSystem fileSystem, IPathUtility pathUtility,
            IHashUtility hashUtility)
        {
            if (entry == null)
            {
                throw new ArgumentNullException(nameof(entry));
            }
            if (pathUtility == null)
            {
                throw new ArgumentNullException(nameof(pathUtility));
            }
            if (hashUtility == null)
            {
                throw new ArgumentNullException(nameof(hashUtility));
            }
            if (string.IsNullOrWhiteSpace(targetDirectory))
            {
                throw new ArgumentNullException(nameof(targetDirectory));
            }
            _hashUtility = hashUtility;
            Debug($"Creating {hashUtility.HashName} {nameof(HashVerifier)} for hash dictionary entry:<{entry}> with root source directory <{entry.RootDirectory}> and root target directory <{targetDirectory}>");
            HashEntry = entry;
            RootSourceDirectory = entry.RootDirectory;
            RootTargetDirectory = targetDirectory;
            _fileSystem = fileSystem;
            FullSourcePath = pathUtility.Combine(RootSourceDirectory, HashEntry.RelativePath);
            FullTargetPath = pathUtility.Combine(RootTargetDirectory, HashEntry.RelativePath);
        }

        public void RecalculateFileMatch()
        {
            Debug($"Allowing for recalculation of local file based on hash file line <{HashEntry}>");
            _status = VerificationStatus.TargetFileNotRead;
        }

        public VerificationStatus Status 
        {
            get
            {
                if (_status == VerificationStatus.TargetFileNotRead)
                {
                    if (!_fileSystem.DoesFileExist(FullTargetPath))
                    {
                        _status = VerificationStatus.TargetFileDoesNotExist;
                    }
                    else
                    {
                        try
                        {                            
                            if (CalculatedHash == HashEntry.Hash)
                            {
                                _status = VerificationStatus.TargetFileMatchesHash;
                            }
                            else
                            {
                                _status = VerificationStatus.TargetFileDoesNotMatchHash;
                            }
                        }
                        catch (Exception e)
                        {
                            Error($"Failed to verify hash of file at {FullTargetPath} - expected hash {HashEntry.Hash}", e);
                            _status = VerificationStatus.TargetFileNotRead;
                        }
                    }
                }
                return _status;
            }
        }

        public string CalculatedHash
        {
            get
            {
                if (string.IsNullOrEmpty(_calculatedHash))
                {
                    _calculatedHash = _hashUtility.GetHashFromFile(FullTargetPath);
                }
                return _calculatedHash;
            }
        }

        public string FullSourcePath { get; }
        public string FullTargetPath { get; }
        public string RootSourceDirectory { get; }
        public string RootTargetDirectory { get; }

    }
}
