using System;
using System.Collections.Generic;
using System.Linq;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core.DirectorySynchronization
{
    public class DirectorySynchCommandLineArgumentParser : Loggable
    {
        public static class ErrorCodes
        {
            public const int InvalidArgumentsProvided = 665;
            public const int NoArgumentsProvided = 666;
            public const int ArgumentSplitToInvalidNumberOfParts = 667;
            public const int ArgumentDoesNotStartWithSource = 668;
            public const int ArgumentDoesNotHaveDestinationComponentInSecondPosition = 669;
            public const int InvalidLoggingThreshold = 670;
        }

        public int ErrorNumber { get; private set; }

        public DirectorySynchCommandLineArgumentParser(IEnumerable<string> commandLineArgs, LoggingLevelCommandLineParser loggingLevelParser)
        {
            var args = new List<string>();
            if(commandLineArgs != null) args.AddRange(commandLineArgs);

            if (!args.Any())
            {
                Error("No synchronization arguments were supplied.  Arguments should be supplied in the form of" +
                      $"\r\nsource:PATHTOSOURCEDIR;destination:PATHTODESTINATIONDIR [...] [verbosity:<LoggingVerbosityLevel>].  Exiting with code {ErrorCodes.NoArgumentsProvided}.");
                ErrorNumber = ErrorCodes.NoArgumentsProvided;
                return;
            }

            args = loggingLevelParser.SetLoggingLevelAndReturnNonLoggingArgs(args);

            Info($"Received {args.Count} command line arguments: [<{string.Join(">,<",args)}>]");
            
            DirectorySynchArguments = new List<IDirectorySynchArgument>();
            foreach (var commandLineArg in args)
            {
                var arg = ParseCommandlineArg(commandLineArg);
                if (arg == null)
                {
                    Error($"Invalid command line arguments were provided.  Exiting with code {ErrorNumber}.");
                    if (ErrorNumber.IsReturnCodeJustPeachy())
                    {
                        // Set a generic error code if a more specific one wasn't set earlier.
                        ErrorNumber = ErrorCodes.InvalidArgumentsProvided;
                    }
                    return;
                }
                DirectorySynchArguments.Add(arg);
            }
        }

        private DirectorySynchArgument ParseCommandlineArg(string arg)
        {
            Debug($"Parsing command line arg <{arg}>");
            // Args are of format source:PATHTOSOURCEDIR;destination:PATHTODESTINATIONDIR
            var split = arg.Split(new [] { ';'}, StringSplitOptions.RemoveEmptyEntries);
            if (split.Length != 2)
            {
                Error($"Argument \r\n\t<{arg}>\r\nis not of format source:PATHTOSOURCEDIR;destination:PATHTODESTINATIONDIR." +
                      $"\r\nExiting with error code {ErrorCodes.ArgumentSplitToInvalidNumberOfParts}.");
                ErrorNumber = ErrorCodes.ArgumentSplitToInvalidNumberOfParts;
                return null;
            }
            var source = split.First();
            var dest = split.Last();
            if (!source.ToUpperInvariant().StartsWith("SOURCE:"))
            {
                Error($"Argument \r\n\t<{arg}>\r\nis not of format source:PATHTOSOURCEDIR;destination:PATHTODESTINATIONDIR" +
                      $"\r\nExiting with error code {ErrorCodes.ArgumentDoesNotStartWithSource}.");
                ErrorNumber = ErrorCodes.ArgumentDoesNotStartWithSource;
                return null;
            }
            if (!dest.ToUpperInvariant().StartsWith("DESTINATION:"))
            {
                Error($"Argument \r\n\t<{arg}>\r\nis not of format source:PATHTOSOURCEDIR;destination:PATHTODESTINATIONDIR" +
                      $"\r\nExiting with error code {ErrorCodes.ArgumentDoesNotHaveDestinationComponentInSecondPosition}.");
                ErrorNumber = ErrorCodes.ArgumentDoesNotHaveDestinationComponentInSecondPosition;
                return null;
            }

            source = source.Substring("source:".Length);
            dest = dest.Substring("destination:".Length);

            source = source.Trim('"');
            dest = dest.Trim('"');
            Info($"Parsed command line argument <{arg}> as\r\n" +
                 $"\tSource = {source}\r\n" +
                 $"\tDestination = {dest}");
            return new DirectorySynchArgument
            {
                SourceDirectory = source,
                TargetDirectory = dest
            };
        }

        public List<IDirectorySynchArgument> DirectorySynchArguments { get; }

        private class DirectorySynchArgument : IDirectorySynchArgument
        {
            public string SourceDirectory { get; set; }
            public string TargetDirectory { get; set; }
        }
    }
}
