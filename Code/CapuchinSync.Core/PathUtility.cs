using System;
using System.IO;
using CapuchinSync.Core.Interfaces;
using System.Runtime.InteropServices;
using System.Text;

namespace CapuchinSync.Core
{
    public class PathUtility : IPathUtility
    {
        public string GetParentDirectoryFromPath(string path)
        {
            return Path.GetDirectoryName(path);
        }

        public string Combine(string rootedPath, string pathFragment)
        {
            return Path.Combine(rootedPath, pathFragment);
        }

        public string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        // Based on https://stackoverflow.com/questions/3525775/how-to-check-if-directory-1-is-a-subdirectory-of-dir2-and-vice-versa
        // Is case-insensitive, so definitely is only fully useful on windows file systems.
        public bool IsSubPathOrEqualTo(string parentPath, string childPath)
        {
            return childPath.IsSubPathOf(parentPath);
            //var parentUri = new Uri(parentPath);
            //var childUri = new Uri(childPath);
            //var result = parentUri.IsBaseOf(childUri);
            //return result;
        }

        public string CalculateRelativePath(string rootDirectory, string fileFullPath)
        {
            // TODO: Make use of the Uri class instead?
            if (!rootDirectory.EndsWith("\\") && !rootDirectory.EndsWith("/"))
            {
                rootDirectory = rootDirectory + "\\";
            }
            StringBuilder builder = new StringBuilder(1024);
            bool success = PathRelativePathTo(builder, rootDirectory, 0, fileFullPath, 0);
            var result = builder.ToString();
            if (result.StartsWith(".\\"))
            {
                result = result.Substring(2);
            }

            if (result.StartsWith(".."))
            {
                int x = 22;

                var isSub = IsSubPathOrEqualTo(rootDirectory, fileFullPath);

            }

            return result;
        }

        [DllImport("shlwapi.dll", EntryPoint = "PathRelativePathTo")]
        protected static extern bool PathRelativePathTo(StringBuilder lpszDst,
            string from, UInt32 attrFrom,
            string to, UInt32 attrTo);
    }
}
