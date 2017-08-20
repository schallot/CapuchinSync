using System;
using System.Collections.Generic;
using System.Linq;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.DirectorySynchronization
{
    public class DirectorySyncher : Loggable
    {
        private int _filesExamined = 0;
        private int _failedReads;
        public bool OpenLogInNotepad = false;
        private readonly IFileSystem _fileSystem;
        private readonly IPathUtility _pathUtility;
        private readonly IFileCopierFactory _fileCopierFactory;

        public DirectorySyncher(IFileSystem fileSystem, IPathUtility pathUtility, IFileCopierFactory fileCopierFactory)
        {
            if(fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));
            if(pathUtility == null) throw new ArgumentNullException(nameof(pathUtility));
            if(fileCopierFactory == null) throw new ArgumentNullException(nameof(fileCopierFactory));
            Debug($"Creating instance of {nameof(DirectorySyncher)} with filesystem of type {fileSystem.GetType()}");
            _fileSystem = fileSystem;
            _pathUtility = pathUtility;
            _fileCopierFactory = fileCopierFactory;
        }

        public int Synchronize(List<HashVerifier> hashesToVerify)
        {
            var groups = hashesToVerify
                .GroupBy(x => x.HashEntry.Hash).Select(x => x.ToArray()).ToArray();

            int copies = 0;

            foreach (var group in groups)
            {
                var unknownStatus = group.Where(x => x.Status == HashVerifier.VerificationStatus.TargetFileNotRead).ToList();
                var matchingHashes = group.Where(x => x.Status == HashVerifier.VerificationStatus.TargetFileMatchesHash).ToList();
                var misMatches = group.Where(x =>
                    x.Status == HashVerifier.VerificationStatus.TargetFileDoesNotMatchHash
                    || x.Status == HashVerifier.VerificationStatus.TargetFileDoesNotExist).ToList();

                if (!matchingHashes.Any())
                {
                    var firstMisMatch = misMatches.First();
                    misMatches.RemoveAt(0);
                    Info($"Updating {firstMisMatch.FullTargetPath} from {firstMisMatch.FullSourcePath}.");
                    var copier = _fileCopierFactory.CreateFileCopier(_fileSystem, _pathUtility, 
                        firstMisMatch.FullSourcePath, firstMisMatch.FullTargetPath);
                    copier.PerformCopy();
                    matchingHashes.Add(firstMisMatch);
                    copies++;
                }

                var firstMatch = matchingHashes.First();
                foreach (var mismatch in misMatches)
                {
                    Info($"Updating {mismatch.FullTargetPath} from local {firstMatch.FullTargetPath}.");
                    var copier = _fileCopierFactory.CreateFileCopier(_fileSystem, _pathUtility, 
                        firstMatch.FullTargetPath, mismatch.FullTargetPath);
                    copier.PerformCopy();
                    copies++;
                }
                foreach (var unknown in unknownStatus)
                {
                    Error($"Failed to validate file at {unknown.FullTargetPath}: {unknown.ErrorMessage}");
                    _failedReads++;
                }
            }

            var tempFile = _pathUtility.GetTempFileName();

            Info($"Writing log file to {tempFile}");
            Info($"Finished synchronization of {_filesExamined} files after {copies} file copies, with {_failedReads} target file validation errors.");

            WriteAllLogEntriesToFile(tempFile);

            if (OpenLogInNotepad)
            {
                System.Diagnostics.Process.Start(@"C:\Program Files (x86)\Notepad++\notepad++.exe", tempFile);
            }
            return 0;
        }
    }
}
