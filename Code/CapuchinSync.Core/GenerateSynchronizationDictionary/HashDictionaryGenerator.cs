using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var hashFile = Constants.HashFileName;

            // Because the actual enumeration of the filesystem happens at the evaluation of the
            // return of EnumerateFilesInDirectory, we need to look for file system errors at the 
            // enumeration, and not at the actual call of EnumerateFilesInDirectory
            try
            {
                var allFiles = fileSystem.EnumerateFilesInDirectory(rootDir);
                Parallel.ForEach(allFiles, file =>
                {
                    FileCount++;
                    //var hasher = new FileHasher(hashUtility, rootDir, file);
                    var hasher = hasherFactory.CreateHasher(file);
                    // there's no point in recording the hash of the list of hashes.
                    if (hasher.RelativePath.Equals(hashFile, StringComparison.InvariantCultureIgnoreCase)) return;
                    hashes.Add(hasher);
                });
            }
            catch (Exception e)
            {
                throw new Exception($"Enumeration of directory {rootDir} failed: {e.Message}", e);
            }

            HashDictionaryFilepath = pathUtility.Combine(rootDir, hashFile);
            var backupLocation = HashDictionaryFilepath + ".old";
            StringBuilder sb = new StringBuilder();
            var now = dateTimeProvider.Now;
            sb.AppendLine($"{hashes.Count} files found in {rootDir} on {dateTimeProvider.GetDateString(now)} at {dateTimeProvider.GetTimeString(now)}");
            foreach (var hash in hashes)
            {
                sb.AppendLine(hash.DictionaryEntryString);
            }
            if (fileSystem.DoesFileExist(HashDictionaryFilepath))
            {
                fileSystem.MoveFile(HashDictionaryFilepath, backupLocation);
            }
            fileSystem.WriteAsUtf8TextFile(HashDictionaryFilepath, sb.ToString());
            if (fileSystem.DoesFileExist(backupLocation))
            {
                fileSystem.DeleteFile(backupLocation);
            }

            Info($"Hashes written to {HashDictionaryFilepath}");

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
