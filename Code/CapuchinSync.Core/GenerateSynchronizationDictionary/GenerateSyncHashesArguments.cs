using System.Linq;

namespace CapuchinSync.Core.GenerateSynchronizationDictionary
{
    public class GenerateSyncHashesArguments
    {
        public string[] ExtensionsToExclude { get; set; }
        public string RootDirectory { get; set; }

        public override string ToString()
        {            
            if(ExtensionsToExclude == null || !ExtensionsToExclude.Any())
            return $"Directory:<{RootDirectory}>";
            return $"Directory:<{RootDirectory}>, Excludes:<{string.Join(",", ExtensionsToExclude)}>";
        }
    }
}
