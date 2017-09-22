using System;
using System.IO;

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

        /// <summary>
        /// Thanks to anjdreas at https://stackoverflow.com/questions/3525775/how-to-check-if-directory-1-is-a-subdirectory-of-dir2-and-vice-versa
        /// Returns true if <paramref name="path"/> starts with the path <paramref name="baseDirPath"/>.
        /// The comparison is case-insensitive, handles / and \ slashes as folder separators and
        /// only matches if the base dir folder name is matched exactly ("c:\foobar\file.txt" is not a sub path of "c:\foo").
        /// </summary>
        public static bool IsSubPathOf(this string path, string baseDirPath)
        {
            string normalizedPath = Path.GetFullPath(path.Replace('/', '\\')
                .WithEnding("\\"));

            string normalizedBaseDirPath = Path.GetFullPath(baseDirPath.Replace('/', '\\')
                .WithEnding("\\"));

            return normalizedPath.StartsWith(normalizedBaseDirPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Thanks to anjdreas at https://stackoverflow.com/questions/3525775/how-to-check-if-directory-1-is-a-subdirectory-of-dir2-and-vice-versa
        /// Returns <paramref name="str"/> with the minimal concatenation of <paramref name="ending"/> (starting from end) that
        /// results in satisfying .EndsWith(ending).
        /// </summary>
        /// <example>"hel".WithEnding("llo") returns "hello", which is the result of "hel" + "lo".</example>
        public static string WithEnding(this string str, string ending)
        {
            if (str == null)
                return ending;

            string result = str;

            // Right() is 1-indexed, so include these cases
            // * Append no characters
            // * Append up to N characters, where N is ending length
            for (int i = 0; i <= ending.Length; i++)
            {
                string tmp = result + ending.Right(i);
                if (tmp.EndsWith(ending))
                    return tmp;
            }

            return result;
        }

        /// <summary>Gets the rightmost <paramref name="length" /> characters from a string.</summary>
        /// Thanks to anjdreas at https://stackoverflow.com/questions/3525775/how-to-check-if-directory-1-is-a-subdirectory-of-dir2-and-vice-versa
        /// <param name="value">The string to retrieve the substring from.</param>
        /// <param name="length">The number of characters to retrieve.</param>
        /// <returns>The substring.</returns>
        public static string Right(this string value, int length)
        {
            if (value == null)
            {
                throw new ArgumentNullException("value");
            }
            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", length, "Length is less than zero");
            }

            return (length < value.Length) ? value.Substring(value.Length - length) : value;
        }

    }
}
