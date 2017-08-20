using System.Collections.Generic;

namespace CapuchinSync.Core.Hashes
{
    public class HashDictionary
    {
        public List<HashDictionaryEntry> Entries { get; } = new List<HashDictionaryEntry>();
        public string FilePath { get; set; }
    }
}
