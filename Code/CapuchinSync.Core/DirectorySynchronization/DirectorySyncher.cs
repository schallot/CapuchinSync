using System;
using System.Collections.Generic;
using System.Linq;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.DirectorySynchronization
{
    public class DirectorySyncher : Loggable
    {
        private int _filesExamined;
        private readonly IFileSystem _fileSystem;
        private readonly IPathUtility _pathUtility;
        private readonly IFileCopierFactory _fileCopierFactory;
        private readonly ILogViewer _logViewer;

        public DirectorySyncher(IFileSystem fileSystem, IPathUtility pathUtility, IFileCopierFactory fileCopierFactory, ILogViewer logViewer = null)
        {
            if(fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));
            if(pathUtility == null) throw new ArgumentNullException(nameof(pathUtility));
            if(fileCopierFactory == null) throw new ArgumentNullException(nameof(fileCopierFactory));
            Trace($"Creating instance of {nameof(DirectorySyncher)} with filesystem of type {fileSystem.GetType()}");
            _logViewer = logViewer;
            _fileSystem = fileSystem;
            _pathUtility = pathUtility;
            _fileCopierFactory = fileCopierFactory;
        }

        public int Synchronize(IEnumerable<IHashVerifier> hashesToVerify)
        {
            var hashGroups = hashesToVerify
                .GroupBy(x => x.GetHashEntry().Hash).Select(x => x.ToArray()).ToArray();
            _filesExamined = hashGroups.Sum(x => x.Length);

            int copies = 0;

            // Each group contains all files that match a particular hash.  This will let us limit the copying of a 
            // files with that hash to at most once over the network, with all subsequent copies happening locally
            foreach (var group in hashGroups)
            {
                var matchingHashes = group.Where(x => x.Status == HashVerifier.VerificationStatus.TargetFileMatchesHash).ToList();
                var misMatches = group.Where(x =>
                    x.Status == HashVerifier.VerificationStatus.TargetFileDoesNotMatchHash
                    || x.Status == HashVerifier.VerificationStatus.TargetFileDoesNotExist
                    || x.Status == HashVerifier.VerificationStatus.TargetFileNotRead).ToList();

                // We know that each group contains at least one file, so if there are no matching hashes, we can be sure that 
                // there is at least one mismatch that we can work with.  We'll copy it locally, and then perform all
                // subsequent copies using the newly copied file
                if (!matchingHashes.Any())
                {
                    var firstMisMatch = misMatches.First();
                    misMatches.RemoveAt(0);
                    Info($"Updating {firstMisMatch.FullTargetPath} from {firstMisMatch.FullSourcePath}.");
                    var copier = _fileCopierFactory.CreateFileCopier(firstMisMatch.FullSourcePath, firstMisMatch.FullTargetPath);
                    copier.PerformCopy();
                    matchingHashes.Add(firstMisMatch);
                    copies++;
                }

                // We know at this point that we have at least one file with our hash on our local folder, so we can use it for
                // future copies.
                var firstMatch = matchingHashes.First();
                if (firstMatch != null)
                {
                    foreach (var mismatch in misMatches)
                    {
                        Info($"Updating {mismatch.FullTargetPath} from local {firstMatch.FullTargetPath}.");
                        var copier =
                            _fileCopierFactory.CreateFileCopier(firstMatch.FullTargetPath, mismatch.FullTargetPath);
                        copier.PerformCopy();
                        copies++;
                    }
                }
            }

            Info($"Finished synchronization of {_filesExamined} files after {copies} file copies.");
            _logViewer?.ViewLogs(AllLogEntries);
            return 0;
        }
    }
}
