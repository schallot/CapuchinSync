using System.Collections.Generic;

namespace CapuchinSync.Core.Interfaces
{
    /// <summary>
    /// An abstraction around the Windows filesystem 
    /// so that we can easily test things that depend on it.
    /// </summary>
    public interface IFileSystem
    {
        /// <summary>
        /// Determines whether or not a file exists.
        /// </summary>
        /// <param name="path">The path to the file in question.</param>
        /// <returns></returns>
        bool DoesFileExist(string path);
        /// <summary>
        /// Determines whether or not a directory exists.
        /// </summary>
        /// <param name="path">The path to the directory in question.</param>
        /// <returns></returns>
        bool DoesDirectoryExist(string path);
        /// <summary>
        /// Creates a directory.
        /// </summary>
        /// <param name="path">The path to the directory that is to be created.</param>
        bool CreateDirectory(string path);
        /// <summary>
        /// Copies a file from <see cref="sourcePath"/> to <see cref="targetPath"/>. 
        /// If a file already exists at <see cref="targetPath"/>, it is overwritten.
        /// </summary>
        /// <param name="sourcePath">The source path.</param>
        /// <param name="targetPath">The target path.</param>
        void CopyFileAndOverwriteIfItExists(string sourcePath, string targetPath);
        /// <summary>
        /// Reads all lines from a file, and returns them as a collection of strings.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        IEnumerable<string> ReadAllLines(string path);
        /// <summary>
        /// Enumerates the files in a directory, including all files in all subdirectories.
        /// </summary>
        /// <param name="directoryPath">The directory path.</param>
        /// <returns></returns>
        IEnumerable<string> EnumerateFilesInDirectory(string directoryPath);
        /// <summary>
        /// Moves a file from one location to another.
        /// </summary>
        /// <param name="originalLocation">The original location.</param>
        /// <param name="targetLocation">The target location.</param>
        void MoveFile(string originalLocation, string targetLocation);
        /// <summary>
        /// Deletes a file.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        void DeleteFile(string filePath);

        /// <summary>
        /// Writes as UTF8 text file.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="contents">The contents.</param>
        void WriteAsUtf8TextFile(string path, string contents);
    }
}
