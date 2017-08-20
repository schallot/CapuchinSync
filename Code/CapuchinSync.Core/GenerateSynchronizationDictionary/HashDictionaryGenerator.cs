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
        public HashDictionaryGenerator(GenerateSyncHashesArguments arguments, IFileSystem fileSystem, IHashUtility hashUtility, IPathUtility pathUtility)
        {
            if(arguments == null) throw new ArgumentNullException(nameof(arguments));

            var hashes = new List<FileHasher>();
            Stopwatch watch = new Stopwatch();
            watch.Restart();
            var rootDir = arguments.RootDirectory;
            var hashFile = Constants.HashFileName;

            Parallel.ForEach(fileSystem.EnumerateFilesInDirectory(rootDir), file =>
            {
                FileCount++;
                var hasher = new FileHasher(hashUtility, rootDir, file);
                // there's no point in recording the hash of the list of hashes.
                if (hasher.RelativePath.Equals(hashFile, StringComparison.InvariantCultureIgnoreCase)) return;
                hashes.Add(hasher);
            });

            HashDictionaryFilepath = pathUtility.Combine(rootDir, hashFile);
            var backupLocation = HashDictionaryFilepath + ".old";
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{hashes.Count} files found in {rootDir} on {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}");
            foreach (var hash in hashes)
            {
                sb.AppendLine(hash.ToString());
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
