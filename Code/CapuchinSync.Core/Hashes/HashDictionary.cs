using System.Collections.Generic;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.Hashes
{
    public class HashDictionary
    {
        public List<IHashDictionaryEntry> Entries { get; } = new List<IHashDictionaryEntry>();
        public string FilePath { get; set; }
    }
}
