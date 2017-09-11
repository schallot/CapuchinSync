namespace CapuchinSync.Core.Interfaces
{
    public interface IHashDictionaryFactory
    {
        IHashDictionaryEntry CreateHashEntry(string filePath);
    }
}
