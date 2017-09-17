using CapuchinSync.Core.DirectorySynchronization;

namespace CapuchinSync.Core.Interfaces
{
    public interface IHashVerifier
    {
        string CalculatedHash { get; }
        string FullSourcePath { get; }
        string FullTargetPath { get; }

        IHashDictionaryEntry GetHashEntry();

        string RootSourceDirectory { get; }
        string RootTargetDirectory { get; }
        HashVerifier.VerificationStatus Status { get; }
    }
}