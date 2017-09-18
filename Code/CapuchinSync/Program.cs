﻿using System;
using System.Collections.Generic;
using System.Linq;
using CapuchinSync.Core;
using CapuchinSync.Core.DirectorySynchronization;
using CapuchinSync.Core.Hashes;

namespace CapuchinSync
{
    public class Program : Loggable
    {
        public static int Main(string[] args)
        {
            var fileSystem = new FileSystem();
            var pathUtility = new PathUtility();
            var hashUtility = new Sha1Hash();
            var fileCopierFactory = new FileCopierFactory(fileSystem, pathUtility);
            var loggingCommandParser = new LoggingLevelCommandLineParser();
            args = loggingCommandParser.SetLoggingLevelAndReturnNonLoggingArgs(args).ToArray();
            var parser = new DirectorySynchCommandLineArgumentParser(args);
            if (parser.ErrorNumber != 0)
            {
                Console.WriteLine($"Failed to parse command line arguments.  Returning error code {parser.ErrorNumber}.");
                return parser.ErrorNumber;
            }
            List<HashVerifier> hashesToVerify = new List<HashVerifier>();
            foreach (var argument in parser.DirectorySynchArguments)
            {
                HashDictionaryFactory dictionaryFactory = new HashDictionaryFactory(hashUtility, argument.SourceDirectory);
                var reader = new HashDictionaryReader(argument.SourceDirectory, new FileSystem(), pathUtility, dictionaryFactory);
                if (reader.ErrorCode != 0)
                {
                    Console.WriteLine($"Failed to create hash dictionary reader for directory {argument.SourceDirectory}.  Returning error code {reader.ErrorCode}.");
                    return reader.ErrorCode;
                } 
                var dictionary = reader.Read();
                if (reader.ErrorCode != 0)
                {
                    Console.WriteLine($"Failed to read hash dictionary for directory {argument.SourceDirectory}.  Returning error code {reader.ErrorCode}.");
                }
                hashesToVerify.AddRange(dictionary.Entries.Select(y => new HashVerifier(y, argument.TargetDirectory, fileSystem, pathUtility, hashUtility)));
            }
            var processStarter = new ProcessStarter();
            var logViewer = new TextFileLogViewer(pathUtility, fileSystem, processStarter);
            var syncher = new DirectorySyncher(new FileSystem(), pathUtility, fileCopierFactory, logViewer);
            return syncher.Synchronize(hashesToVerify);
        }


    }
}
