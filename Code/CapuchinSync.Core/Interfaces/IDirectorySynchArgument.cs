using CapuchinSync.Core.DirectorySynchronization;

namespace CapuchinSync.Core.Interfaces
{
    /// <summary>
    /// An argument to the <see cref="DirectorySyncher"/> class specifying the 
    /// source and target directories that will be synched.
    /// </summary>
    public interface IDirectorySynchArgument
    {
        /// <summary>
        /// Gets the full path of the source directory.  After synchronization,
        /// the files in <see cref="TargetDirectory"/> will match those in this directory, and 
        /// the contents of this directory will be unchanged.
        /// </summary>
        /// <value>
        /// The source directory.
        /// </value>
        string SourceDirectory { get; }
        /// <summary>
        /// Gets the target directory.  After synchronization,
        /// the files in this directory will be updated to match those in <see cref="SourceDirectory"/> 
        /// </summary>
        /// <value>
        /// The target directory.
        /// </value>
        string TargetDirectory { get; }
    }
}
