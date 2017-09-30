using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileSystem : Loggable, IFileSystem
    {
        /// <summary>
        /// Determines whether or not a file exists.
        /// </summary>
        /// <param name="path">The path to the file in question.</param>
        /// <returns></returns>
        public bool DoesFileExist(string path)
        {
            return File.Exists(path);
        }

        /// <summary>
        /// Determines whether or not a directory exists.
        /// </summary>
        /// <param name="path">The path to the directory in question.</param>
        /// <returns></returns>
        public bool DoesDirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="path">The path to the directory that is to be created.</param>
        public bool CreateDirectory(string path)
        {
            return Directory.CreateDirectory(path).Exists;
        }

        private static object FileCopyCountLock = new object();
        private static object FileCopyErrorCountLock = new object();
        public static int FileCopyCount = 0;
        public static int FileCopyErrorCount = 0;

        /// <summary>
        /// Copies a file from <see cref="sourcePath" /> to <see cref="targetPath" />.
        /// If a file already exists at <see cref="targetPath" />, it is overwritten.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        public void CopyFileAndOverwriteIfItExists(string sourcePath, string targetPath)
        {
            if (File.Exists(targetPath))
            {
                File.SetAttributes(targetPath, FileAttributes.Normal);
            }
            try
            {
                File.Copy(sourcePath, targetPath, true);
            }
            catch
            {
                lock (FileCopyErrorCountLock)
                {
                    FileCopyErrorCount++;
                }
                throw;
            }
            lock (FileCopyCountLock)
            {
                FileCopyCount++;
            }
        }

        /// <summary>
        /// Reads all lines from a file, and returns them as a collection of strings.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public IEnumerable<string> ReadAllLines(string path)
        {
            return File.ReadAllLines(path);
        }

        /// <summary>
        /// Enumerates the files in a directory, including all files in all subdirectories.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns></returns>
        public IEnumerable<string> EnumerateFilesInDirectory(string directoryPath, IEnumerable<string> extensionsToExclude, IEnumerable<string> filesToExclude)
        {
            List<string> extExcludes = new List<string>();
            List<string> fileExcludes = new List<string>();
            if (extensionsToExclude != null)
            {
                extExcludes.AddRange(extensionsToExclude.Select(x=>x.Trim('.').Trim()).Where(x=>!string.IsNullOrWhiteSpace(x)));
            }
            if (filesToExclude != null)
            {
                fileExcludes.AddRange(filesToExclude.Select(x=>x.Trim()).Where(x=>!string.IsNullOrWhiteSpace(x)));
            }

            return Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories)
                .Where(x => !extExcludes.Any(y=>HasExtension(x,y)))
                .Where(x=> !fileExcludes.Any(y=>IsFileNameMatchWithExtension(x,y)));
        }

        private static bool HasExtension(string filePath, string searchExtension)
        {
            var ext = Path.GetExtension(filePath);
            if (string.IsNullOrWhiteSpace(ext)) return false;
            if (ext.StartsWith(".")) ext = ext.TrimStart('.');
            return searchExtension.Equals(ext, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool IsFileNameMatchWithExtension(string filePath, string searchNameWithExtension)
        {
            var fileName = Path.GetFileName(filePath);
            if (string.IsNullOrWhiteSpace(fileName)) return false;
            return fileName.Equals(searchNameWithExtension, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Moves a file from one location to another.
        /// </summary>
        /// /<param name="originalLocation">The original location.</param>
        /// <param name="targetLocation">The target location.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void MoveFile(string originalLocation, string targetLocation)
        {
            File.Move(originalLocation, targetLocation);
        }

        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void DeleteFile(string filePath)
        {
            // https://stackoverflow.com/questions/1157246/unauthorizedaccessexception-trying-to-delete-a-file-in-a-folder-where-i-can-dele
            // Trying to get around baffling System.UnauthorizedAccessException errors that are showing up, even when this process created the file that we're deleting.
            File.SetAttributes(filePath, FileAttributes.Normal);
            File.Delete(filePath);
        }

        /// <summary>
        /// Writes as UTF8 text file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        /// <exception cref="System.NotImplementedException"></exception>
        public void WriteAsUtf8TextFile(string path, string contents)
        {
            File.WriteAllText(path, contents, Encoding.UTF8);
        }

        /// <summary>
        /// Writes lines as a UTF8 text file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="lines">The lines to be written to the file.</param>
        public void WriteAllLinesAsUtf8TextFile(string path, IEnumerable<string> lines)
        {
            File.WriteAllLines(path, lines, Encoding.UTF8);
        }
    }
}
