namespace CapuchinSync.Core.Interfaces
{
    public interface IPathUtility
    {
        /// <summary>
        /// Gets the parent directory from a path.
        /// For example, for the path "C:\Temp\blah", we'll return "C:\Temp"
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetParentDirectoryFromPath(string path);
        /// <summary>
        /// Combines the specified rooted path with a path fragment.
        /// For example, we'll combine "C:\Temp\" and "blah" as "C:\Temp\Blah"
        /// </summary>
        /// <param name="rootedPath">The rooted path.</param>
        /// <param name="pathFragment">The path fragment.</param>
        /// <returns></returns>
        string Combine(string rootedPath, string pathFragment);

        /// <summary>
        /// Gets the name of the temporary file where data can safely be written.
        /// </summary>
        /// <returns></returns>
        string GetTempFileName();

        /// <summary>
        /// Returns the file name and extension of the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetFileName(string path);

        /// <summary>
        /// Given two directories, returns true if the second is a subdirectory of the first, or if the two are the same directory.
        /// </summary>
        /// <param name="directoryFullPath">The directory full path.</param>
        /// <param name="possibleSubPathFullPath">The possible sub directory full path.</param>
        /// <returns>
        ///   <c>true</c> if [is sub directory of] [the specified directory full path]; otherwise, <c>false</c>.
        /// </returns>
        bool IsSubPathOrEqualTo(string directoryFullPath, string possibleSubPathFullPath);

        /// <summary>
        /// Calculates the relative path between a file and a directory.
        /// </summary>
        /// <param name="rootDirectory">The root directory.</param>
        /// <param name="fileFullPath">The file full path.</param>
        /// <returns></returns>
        string CalculateRelativePath(string rootDirectory, string fileFullPath);
    }
}
