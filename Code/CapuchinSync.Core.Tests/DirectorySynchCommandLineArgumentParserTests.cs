using System.Linq;
using CapuchinSync.Core.DirectorySynchronization;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests
{
    [TestFixture]
    public class DirectorySynchCommandLineArgumentParserTests
    {
        [Test]
        public void ShouldParseArguments()
        {
            var args = new []
            {
                "source:C:\\temp1;destination:C:\\temp2"
            };
            var parser = new DirectorySynchCommandLineArgumentParser(args, new LoggingLevelCommandLineParser());

            Assert.AreEqual(0, parser.ErrorNumber, "Unexpected error number");
            Assert.AreEqual(1, parser.DirectorySynchArguments.Count, "Expected 1 arguments in our result");
            var first = parser.DirectorySynchArguments.First();

            Assert.AreEqual("C:\\temp1", first.SourceDirectory, "Unexpected source directory");
            Assert.AreEqual("C:\\temp2", first.TargetDirectory, "Unexpected target directory");
        }

        [Test]
        public void ShouldParseMultipleArguments()
        {
            var args = new[]
            {
                "source:C:\\temp1;destination:C:\\temp2",
                "source:C:\\temp3;destination:C:\\temp4",
                "source:C:\\temp5;destination:C:\\temp6"
            };
            var parser = new DirectorySynchCommandLineArgumentParser(args, new LoggingLevelCommandLineParser());

            Assert.AreEqual(0, parser.ErrorNumber, "Unexpected error number");
            Assert.AreEqual(3, parser.DirectorySynchArguments.Count, "Expected 3 arguments in our result");

            var first = parser.DirectorySynchArguments.First();

            Assert.AreEqual("C:\\temp1", first.SourceDirectory, "Unexpected source directory");
            Assert.AreEqual("C:\\temp2", first.TargetDirectory, "Unexpected target directory");

            var third = parser.DirectorySynchArguments.Last();
            Assert.AreEqual("C:\\temp5", third.SourceDirectory, "Unexpected source directory");
            Assert.AreEqual("C:\\temp6", third.TargetDirectory, "Unexpected target directory");
        }

        [Test]
        public void ShouldComplainAboutNullArgument()
        {
            var parser = new DirectorySynchCommandLineArgumentParser(null, new LoggingLevelCommandLineParser());
            Assert.AreEqual(DirectorySynchCommandLineArgumentParser.ErrorCodes.NoArgumentsProvided, 
                parser.ErrorNumber, "Unexpected error number");
        }

        [Test]
        public void ShouldComplainAboutEmptyArgument()
        {
            var parser = new DirectorySynchCommandLineArgumentParser(new string[] {}, new LoggingLevelCommandLineParser());
            Assert.AreEqual(DirectorySynchCommandLineArgumentParser.ErrorCodes.NoArgumentsProvided,
                parser.ErrorNumber, "Unexpected error number");
        }

        [Test]
        public void ShouldParseWithInvalidNumberOfPartsError()
        {
            var args = new[]
            {
                "source:C:\\temp1;destination:C:\\temp2;whatever:blah"
            };
            var parser = new DirectorySynchCommandLineArgumentParser(args, new LoggingLevelCommandLineParser());

            Assert.AreEqual(DirectorySynchCommandLineArgumentParser.ErrorCodes.ArgumentSplitToInvalidNumberOfParts, 
                parser.ErrorNumber, "Unexpected error number");
        }

        [Test]
        public void ShouldParseWithNotStartingWithSourceError()
        {
            var args = new[]
            {
                "whatever:C:\\temp1;destination:C:\\temp2"
            };
            var parser = new DirectorySynchCommandLineArgumentParser(args, new LoggingLevelCommandLineParser());

            Assert.AreEqual(DirectorySynchCommandLineArgumentParser.ErrorCodes.ArgumentDoesNotStartWithSource,
                parser.ErrorNumber, "Unexpected error number");
        }

        [Test]
        public void ShouldParseWithNotHavingDestinationInSecondPositionError()
        {
            var args = new[]
            {
                "source:C:\\temp1;whatever:C:\\temp2"
            };
            var parser = new DirectorySynchCommandLineArgumentParser(args, new LoggingLevelCommandLineParser());

            Assert.AreEqual(DirectorySynchCommandLineArgumentParser.ErrorCodes.ArgumentDoesNotHaveDestinationComponentInSecondPosition,
                parser.ErrorNumber, "Unexpected error number");
        }


        [Test]
        public void ShouldParseArgumentsInSpiteOfExtraSemicolons()
        {
            var args = new[]
            {
                ";;source:C:\\temp1;;;;;destination:C:\\temp2;;;"
            };
            var parser = new DirectorySynchCommandLineArgumentParser(args, new LoggingLevelCommandLineParser() );

            Assert.AreEqual(0, parser.ErrorNumber, "Unexpected error number");
            Assert.AreEqual(1, parser.DirectorySynchArguments.Count, "Expected 1 arguments in our result");
            var first = parser.DirectorySynchArguments.First();

            Assert.AreEqual("C:\\temp1", first.SourceDirectory, "Unexpected source directory");
            Assert.AreEqual("C:\\temp2", first.TargetDirectory, "Unexpected target directory");
        }
    }
}
