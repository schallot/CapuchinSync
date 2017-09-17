namespace CapuchinSync.Core.Interfaces
{
    public interface IFileHasherFactory
    {
        IFileHasher CreateHasher(string filePath);
    }
}
