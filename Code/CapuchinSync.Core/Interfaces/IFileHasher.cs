namespace CapuchinSync.Core.Interfaces
{
    public interface IFileHasher
    {
        string FullPath { get; }
        string GetDictionaryEntryString(string rootDirectory);
    }
}
