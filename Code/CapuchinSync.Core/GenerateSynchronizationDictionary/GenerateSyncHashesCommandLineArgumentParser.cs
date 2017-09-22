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
        public const string DirectoryPrefix = "DIR:";

        public GenerateSyncHashesCommandLineArgumentParser(string[] commandLineArgs, IFileSystem fileSystem)
        {
            string exampleArguments = "Arguments should be supplied in the form of" +
                                            $"\r\n\t{DirectoryPrefix}<Directory> [{DirectoryPrefix}AdditionalDirectory1 ...] [{ExcludeFilePrefix}<FileExcludePattern> ...]" +
                                            $"\r\n\tFor example, <C:\\Directory1 C:\\Directory2 {ExcludeFilePrefix}*.pdb {ExcludeFilePrefix}*.suo>.";           

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
            
            var directories = args.Where(x=> x.StartsWith(DirectoryPrefix, StringComparison.InvariantCultureIgnoreCase)).ToArray();
            args = args.Where(x => !directories.Contains(x)).ToList();

            var nonExistentDirectories = directories.Where(x => !fileSystem.DoesDirectoryExist(x)).ToArray();
            if (nonExistentDirectories.Any())
            {
                Error($"The following ({nonExistentDirectories.Length}) Directories do not exist: {string.Join(", ", nonExistentDirectories)}");
                ErrorNumber = ErrorCodes.DirectoryDoesNotExist;
                return;
            }
            args = args.Where(x => !x.StartsWith(DirectoryPrefix, StringComparison.InvariantCultureIgnoreCase))
                .ToList();

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
            Arguments = new GenerateSyncHashesArguments {RootDirectories = directories, ExtensionsToExclude = excludeArgs};            
            Info($"Received arguments {Arguments}");
        }

        public GenerateSyncHashesArguments Arguments { get; }
    }
}
