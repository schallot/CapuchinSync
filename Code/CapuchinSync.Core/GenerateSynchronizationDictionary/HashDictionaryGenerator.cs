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

            var hashedFiles = new List<IFileHasher>();
            Stopwatch watch = new Stopwatch();
            watch.Restart();
            var rootDirectories = arguments.RootDirectories;
            var extensionsToExclude = arguments.ExtensionsToExclude;
            // there's no point in recording the hash of the list of hashes, or the backup version of this file.
            var filesToExclude = new [] {Constants.HashFileName, Constants.BackupHashFileName};

            // We'll make sure that we only read in each file once, by not bothering to 
            // read in directories that are subdirectories of others.
            var combinedDirectories = new List<string>();
            foreach (var dir in rootDirectories)
            {
                if (!combinedDirectories.Any(x => pathUtility.IsSubPathOrEqualTo(x, dir)))
                {
                    combinedDirectories.Add(dir);
                }
            }

            // Because the actual enumeration of the filesystem happens at the evaluation of the
            // return of EnumerateFilesInDirectory, we need to look for file system errors at the 
            // enumeration, and not at the actual call of EnumerateFilesInDirectory
            try
            {
                var allFiles = combinedDirectories.SelectMany(y=>fileSystem.EnumerateFilesInDirectory(y,extensionsToExclude,filesToExclude));
                Parallel.ForEach(allFiles, file =>
                {
                    FileCount++;
                    var hasher = hasherFactory.CreateHasher(file);
                    hashedFiles.Add(hasher);
                });
            }
            catch (Exception e)
            {
                throw new Exception($"Enumeration of a directory failed: {e.InnerException?.Message}", e);
            }

            foreach (var rootDir in rootDirectories)
            {
                HashDictionaryFilepath = pathUtility.Combine(rootDir, Constants.HashFileName);
                var backupLocation = HashDictionaryFilepath + ".old";
                StringBuilder sb = new StringBuilder();
                var now = dateTimeProvider.Now;
                var hashedFilesInRootDir = hashedFiles.Where(x =>
                    {
                        var result = pathUtility.IsSubPathOrEqualTo(rootDir, x.FullPath);
                        return result;
                    })
                    .OrderBy(x => x.FullPath).ToArray();
                sb.AppendLine($"{hashedFilesInRootDir.Length} files found in {rootDir} on {dateTimeProvider.GetDateString(now)} at {dateTimeProvider.GetTimeString(now)}");
                foreach (var hashed in hashedFilesInRootDir)
                {
                    sb.AppendLine(hashed.GetDictionaryEntryString(rootDir));
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
            }

            watch.Stop();
            Info($"Finished processing {FileCount} files in {watch.ElapsedMilliseconds / 1000d} seconds.");
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
