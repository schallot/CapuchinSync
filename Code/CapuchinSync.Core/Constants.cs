namespace CapuchinSync.Core
{
    public static class Constants
    {
        /// <summary>
        /// The name of the hash file that we'll create in or look for in the source folder.
        /// </summary>
        public const string HashFileName = ".capuchinSync";

        /// <summary>
        /// The name of the hash file that will be copied to before an existing hash file is overwritten.
        /// </summary>
        public const string BackupHashFileName = HashFileName + ".old";

        /// <summary>
        /// A return code that indicates that everything worked as expected.
        /// </summary>
        public const int EverythingsJustPeachyReturnCode = 0;
    }
}
