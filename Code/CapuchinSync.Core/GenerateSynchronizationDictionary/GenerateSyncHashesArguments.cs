using System.Linq;

namespace CapuchinSync.Core.GenerateSynchronizationDictionary
{
    public class GenerateSyncHashesArguments
    {
        public string[] ExtensionsToExclude { get; set; }
        public string[] RootDirectories { get; set; }

        public override string ToString()
        {            
            if(ExtensionsToExclude == null || !ExtensionsToExclude.Any())
            return $"Directories:<{string.Join(", ", RootDirectories)}>";
            return $"Directories:<{string.Join(", ", RootDirectories)}>, Excludes:<{string.Join(",", ExtensionsToExclude)}>";
        }
    }
}
