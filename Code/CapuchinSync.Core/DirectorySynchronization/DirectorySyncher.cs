using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.DirectorySynchronization
{
    public class DirectorySyncher : Loggable
    {
        private int _filesExamined;
        private readonly IFileCopierFactory _fileCopierFactory;
        private readonly ILogViewer _logViewer;
        private readonly int _maxParallelCopies;

        public DirectorySyncher(IFileSystem fileSystem, IPathUtility pathUtility, IFileCopierFactory fileCopierFactory, int maxParallelCopies, ILogViewer logViewer = null)
        {
            if(fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));
            if(pathUtility == null) throw new ArgumentNullException(nameof(pathUtility));
            if(fileCopierFactory == null) throw new ArgumentNullException(nameof(fileCopierFactory));
            _maxParallelCopies = maxParallelCopies;
            if (maxParallelCopies < 0)
            {
                _maxParallelCopies = 1;
            }
            if (maxParallelCopies > 128)
            {
                _maxParallelCopies = 128;
            }
            Trace($"Creating instance of {nameof(DirectorySyncher)} with filesystem of type {fileSystem.GetType()}");
            Info($"Setting maxParallelCopies to {_maxParallelCopies}.");
            _logViewer = logViewer;
            _fileCopierFactory = fileCopierFactory;
        }

        public int Synchronize(IEnumerable<IHashVerifier> hashesToVerify, Stopwatch stopwatch = null)
        {
            if (stopwatch == null)
            {
                stopwatch = new Stopwatch();
                stopwatch.Start();
            }
            var hashGroups = hashesToVerify
                .GroupBy(x => x.GetHashEntry().Hash).Select(x => x.ToArray()).ToArray();
            _filesExamined = hashGroups.Sum(x => x.Length);
            
            // Each group contains all files that match a particular hash.  This will let us limit the copying of a 
            // files with that hash to at most once over the network, with all subsequent copies happening locally
            Parallel.ForEach(hashGroups,
                new ParallelOptions { MaxDegreeOfParallelism = _maxParallelCopies },
                ProcessHashGrouping);

            stopwatch.Stop();
            if (FileSystem.FileCopyErrorCount == 0)
            {
                Info($"Successfully synchronized {_filesExamined} files after {FileSystem.FileCopyCount} file" +
                     $" copies in {(int) (stopwatch.ElapsedMilliseconds / 1000f)} seconds.");
            }
            else
            {
                Error($"Successfully synchronized {_filesExamined} files after {FileSystem.FileCopyCount} file" +
                     $" copies with {FileSystem.FileCopyErrorCount} failed copies" +
                     $" in {(int)(stopwatch.ElapsedMilliseconds / 1000f)} seconds.");
            }
            _logViewer?.ViewLogs(AllLogEntries);
            return 0;
        }

        private class HashVerifierComparer : IEqualityComparer<IHashVerifier>
        {
            public bool Equals(IHashVerifier x, IHashVerifier y)
            {
                if (x == null || y == null) return false;
                return x.FullTargetPath.Equals(y.FullTargetPath, StringComparison.InvariantCultureIgnoreCase)
                       && x.FullSourcePath.Equals(y.FullSourcePath, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode(IHashVerifier obj)
            {
                return $"{obj.FullSourcePath}=>{obj.FullTargetPath}".GetHashCode();
            }
        }

        private void ProcessHashGrouping(IHashVerifier[] group)
        {
            // If we're processing multiple hash dictionaries at once, we could potentially have multiple
            // instances of IHashVerifier pointing to identical files.  We don't want to end up copying from 
            // a file to itself, so we'll filter out all duplicates here.
            group = group.Distinct(new HashVerifierComparer()).ToArray();
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
                }
            }
        }
    }
}
