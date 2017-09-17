namespace CapuchinSync.Core.Interfaces
{
    public interface IFileCopierFactory
    {
        IFileCopier CreateFileCopier(string source, string destination);
    }
}
