﻿using System;
using CapuchinSync.Core;
using CapuchinSync.Core.GenerateSynchronizationDictionary;
using CapuchinSync.Core.Hashes;

namespace CapuchinSync.Hash
{
    public class Program : Loggable
    {
        public static int Main(string[] args)
        {
            var program  = new Program();
            return program.Run(args);
        }

        private int Run(string[] args)
        {
            var fileSystem = new FileSystem();
            var loggingCommandParser = new LoggingLevelCommandLineParser();
            args = loggingCommandParser.SetLoggingLevelAndReturnNonLoggingArgs(args).ToArray();
            var parser = new GenerateSyncHashesCommandLineArgumentParser(args, fileSystem);
            if (!parser.ErrorNumber.IsReturnCodeJustPeachy())
            {
                Error($"Command line parser returned error code {parser.ErrorNumber}.");
                return parser.ErrorNumber;
            }
            var hashUtility = new Sha1Hash();
            var pathUtility = new PathUtility();
            var dateTimeProvider = new DateTimeProvider();
            try
            {
                var hasherFactory = new FileHasherFactory(hashUtility, pathUtility);
                var hasherDictionaryGenerator =
                    new HashDictionaryGenerator(parser.Arguments, fileSystem, pathUtility, hasherFactory, dateTimeProvider);
                Info($"Finished generating {hasherDictionaryGenerator.HashDictionaryFilepath}");
            }
            catch (Exception e)
            {
                Error("Hash dictionary generation failed.", e);
                return 123;
            }
            return Constants.EverythingsJustPeachyReturnCode;
        }
    }
}
