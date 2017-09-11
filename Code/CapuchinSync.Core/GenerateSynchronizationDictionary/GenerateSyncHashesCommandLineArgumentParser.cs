using System;
using System.Collections.Generic;
using System.Linq;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.GenerateSynchronizationDictionary
{
    public class GenerateSyncHashesCommandLineArgumentParser : Loggable
    {
        public static class ErrorCodes
        {
            public const int NoArgumentsProvided = 888;
            public const int DirectoryDoesNotExist = 889;
            public const int UnrecognizedArgument = 890;
        }

        public int ErrorNumber { get; }
        public const string ExcludeFilePrefix = "XF:";

        public GenerateSyncHashesCommandLineArgumentParser(string[] commandLineArgs, IFileSystem fileSystem)
            : base(fileSystem)
        {
            string exampleArguments = "Arguments should be supplied in the form of" +
                                            $"\r\n\t<Directory> [{ExcludeFilePrefix}<FileExcludePattern> ...]" +
                                            $"\r\n\tFor example, <C:\\Directory1 {ExcludeFilePrefix}*.pdb {ExcludeFilePrefix}*.suo>.";           

            var args = new List<string>();
            if (commandLineArgs != null) {
                args.AddRange(
                    commandLineArgs.Where(x=>!string.IsNullOrWhiteSpace(x))
                        .Select(y=>y.Trim()));
            }
            if (!args.Any())
            {
                Error($"No arguments were supplied.  {exampleArguments}");
                ErrorNumber = ErrorCodes.NoArgumentsProvided;
                return;
            }
            Debug($"Received {args.Count} command line arguments: [<{string.Join(">,<", args)}>]");

            var directory = args.First();
            if (!fileSystem.DoesDirectoryExist(directory))
            {
                Error($"Directory <{directory}> does not exist.");
                ErrorNumber = ErrorCodes.DirectoryDoesNotExist;
                return;
            }
            
            args.RemoveAt(0);

            var excludeArgs = args.Where(x => x.StartsWith(ExcludeFilePrefix, StringComparison.InvariantCultureIgnoreCase))
                .Select(x=>x.Substring(ExcludeFilePrefix.Length)
                    .Trim()
                    .Trim('.','*') // We'll get rid of any *'s or .'s in the extensions to normalize things.
                    .Trim())
                .ToArray();
            args = args.Where(x => !x.StartsWith(ExcludeFilePrefix, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (args.Any())
            {
                Error($"Unrecognized argument(s) <{string.Join(",", args)}>.\r\n\t{exampleArguments}");
                ErrorNumber = ErrorCodes.UnrecognizedArgument;
                return;
            }            
            Arguments = new GenerateSyncHashesArguments {RootDirectory = directory, ExtensionsToExclude = excludeArgs};            
            Info($"Received arguments {Arguments}");
        }

        public GenerateSyncHashesArguments Arguments { get; }
    }
}
