namespace CapuchinSync.Core.Interfaces
{
    public interface IFileCopierFactory
    {
        IFileCopier CreateFileCopier(IFileSystem fileSystem, IPathUtility pathUtility, string source,
            string destination);
    }
}
