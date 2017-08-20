namespace CapuchinSync.Core.Interfaces
{
    public interface IHashUtility
    {
        /// <summary>
        /// Reads a file at a given location, and computes a hash for it.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        string GetHashFromFile(string path);
        /// <summary>
        /// Gets the length of all hashes generated with this utility.
        /// </summary>
        /// <value>
        /// The length of the hash.
        /// </value>
        int HashLength { get; }
        /// <summary>
        /// Gets the name of the hash. E.g., SHA1.
        /// </summary>
        /// <value>
        /// The name of the hash.
        /// </value>
        string HashName { get; }
    }
}
