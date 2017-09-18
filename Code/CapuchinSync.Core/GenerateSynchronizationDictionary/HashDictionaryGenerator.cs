using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.GenerateSynchronizationDictionary
{
    public class HashDictionaryGenerator : Loggable
    {
        public HashDictionaryGenerator(GenerateSyncHashesArguments arguments, IFileSystem fileSystem, IPathUtility pathUtility, IFileHasherFactory hasherFactory, IDateTimeProvider dateTimeProvider)
        {
            if(arguments == null) throw new ArgumentNullException(nameof(arguments));

            var hashes = new List<IFileHasher>();
            Stopwatch watch = new Stopwatch();
            watch.Restart();
            var rootDir = arguments.RootDirectory;

            // Because the actual enumeration of the filesystem happens at the evaluation of the
            // return of EnumerateFilesInDirectory, we need to look for file system errors at the 
            // enumeration, and not at the actual call of EnumerateFilesInDirectory
            try
            {
                var allFiles = fileSystem.EnumerateFilesInDirectory(rootDir);
                Parallel.ForEach(allFiles, file =>
                {
                    FileCount++;
                    // there's no point in recording the hash of the list of hashes, or the backup version of this file.
                    if(IsFileNameAMatch(file, Constants.HashFileName, pathUtility)
                        || IsFileNameAMatch(file, Constants.BackupHashFileName, pathUtility)) return;
                    var hasher = hasherFactory.CreateHasher(file);
                    hashes.Add(hasher);
                });
            }
            catch (Exception e)
            {
                throw new Exception($"Enumeration of directory {rootDir} failed: {e.InnerException?.Message}", e);
            }

            HashDictionaryFilepath = pathUtility.Combine(rootDir, Constants.HashFileName);
            var backupLocation = HashDictionaryFilepath + ".old";
            StringBuilder sb = new StringBuilder();
            var now = dateTimeProvider.Now;
            sb.AppendLine($"{hashes.Count} files found in {rootDir} on {dateTimeProvider.GetDateString(now)} at {dateTimeProvider.GetTimeString(now)}");
            foreach (var hash in hashes.OrderBy(x=>x.RelativePath))
            {
                sb.AppendLine(hash.DictionaryEntryString);
            }
            // If an old backup file exists, go ahead and delete it.
            if (fileSystem.DoesFileExist(backupLocation))
            {
                fileSystem.DeleteFile(backupLocation);
            }
            // Now, if an old dictionary exists, move it to the backup location
            if (fileSystem.DoesFileExist(HashDictionaryFilepath))
            {
                fileSystem.MoveFile(HashDictionaryFilepath, backupLocation);
            }
            fileSystem.WriteAsUtf8TextFile(HashDictionaryFilepath, sb.ToString());
            
            Info($"Hashes written to {HashDictionaryFilepath}");

            watch.Stop();
            Info($"Finished processing {FileCount} files in {watch.ElapsedMilliseconds / 1000d} seconds.");
        }

        private bool IsFileNameAMatch(string fullPath, string fileName, IPathUtility pathUtility)
        {
            return (fullPath.EndsWith(fileName, StringComparison.CurrentCultureIgnoreCase) && fileName.Equals(pathUtility.GetFileName(fullPath),
                    StringComparison.InvariantCultureIgnoreCase));
        }

        /// <summary>
        /// Gets the full path to the generated hash dictionary file.
        /// </summary>
        /// <value>
        /// The hash file.
        /// </value>
        public string HashDictionaryFilepath { get; }

        /// <summary>
        /// Gets the number of files included in the hash dictionary.
        /// </summary>
        /// <value>
        /// The file count.
        /// </value>
        public int FileCount { get; private set; }
    }
}
