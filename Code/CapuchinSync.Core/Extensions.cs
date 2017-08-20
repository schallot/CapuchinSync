namespace CapuchinSync.Core
{
    public static class Extensions
    {
        /// <summary>
        /// Determines whether this instance is a valid hexadecimal character (0-9, A-F, a-f).
        /// </summary>
        /// <param name="c">The c.</param>
        /// <returns>
        ///   <c>true</c> if the specified c is hexadecimal; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsHex(this char c)
        {
            return (c >= '0' && c <= '9') ||
                     (c >= 'a' && c <= 'f') ||
                     (c >= 'A' && c <= 'F');
        }


        /// <summary>
        /// Determines whether or not a return code indicates that everything completed successfully.
        /// </summary>
        /// <param name="returnCode">The return code.</param>
        /// <returns>
        ///   <c>true</c> if [is return code just peachy] [the specified return code]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsReturnCodeJustPeachy(this int returnCode)
        {
            return returnCode == Constants.EverythingsJustPeachyReturnCode;
        }
    }
}
