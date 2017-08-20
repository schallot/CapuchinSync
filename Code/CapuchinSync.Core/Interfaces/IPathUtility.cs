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
    }
}
