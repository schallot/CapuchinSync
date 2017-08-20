namespace CapuchinSync.Core.Interfaces
{
    public interface IFileCopier
    {
        bool SuccesfullyCopied { get; }

        void PerformCopy();
    }
}