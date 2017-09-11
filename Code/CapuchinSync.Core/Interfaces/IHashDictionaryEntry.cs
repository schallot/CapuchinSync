namespace CapuchinSync.Core.Interfaces
{
    public interface IHashDictionaryEntry
    {
        string ErrorMessage { get; }
        string Hash { get; }
        bool IsValid { get; }
        string RelativePath { get; }
        string RootDirectory { get; }

        string ToString();
    }
}