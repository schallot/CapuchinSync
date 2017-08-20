using System;
using System.IO;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.Hashes
{
    public class Sha1Hash : IHashUtility
    {
        public string GetHashFromFile(string path)
        {
            using (FileStream fop = File.OpenRead(path))
            {
                var hash = System.Security.Cryptography.SHA1.Create().ComputeHash(fop);
                string chksum = BitConverter.ToString(hash).Replace("-", "").ToUpperInvariant();
                return chksum;
            }
        }

        public int HashLength { get; } = 40;
        public string HashName { get; } = "SHA1";
    }
}
