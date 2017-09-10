namespace CapuchinSync.Core.Interfaces
{
    public interface IFileHasher
    {
        string RelativePath { get; }
        string DictionaryEntryString { get; }
    }
}
