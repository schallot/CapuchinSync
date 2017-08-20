using System.Collections.Generic;
using System.IO;
using System.Text;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileSystem : IFileSystem
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

        /// <summary>
        /// Copies a file from <see cref="sourcePath" /> to <see cref="targetPath" />.
        /// If a file already exists at <see cref="targetPath" />, it is overwritten.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        public void CopyFileAndOverwriteIfItExists(string sourcePath, string targetPath)
        {
            File.Copy(sourcePath, targetPath, true);
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
        public IEnumerable<string> EnumerateFilesInDirectory(string directoryPath)
        {
            return Directory.EnumerateFiles(directoryPath, "*", SearchOption.AllDirectories);
        }

        /// <summary>
        /// Moves a file from one location to another.
        /// </summary>
        /// <param name="originalLocation">The original location.</param>
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
    }
}
