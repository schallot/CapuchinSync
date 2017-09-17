using System;
using System.Collections.Generic;
using System.Linq;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests
{
    [TestFixture]
    public class LoggableTests : Loggable
    {
        private string _message;
        private Exception _exception;

        [SetUp]
        public void SetUp()
        {
            _message = $"Hello {Guid.NewGuid():D}";
            _exception = new Exception($"This is an exception. {Guid.NewGuid()}");
        }

        [Test]
        public void DebugTest()
        {
            Debug(_message);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.AreEqual(LogEntry.LogSeverity.Debug, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }


        [Test]
        public void InfoTest()
        {
            Info(_message);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.AreEqual(LogEntry.LogSeverity.Info, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }


        [Test]
        public void WarnTest()
        {
            Warn(_message);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.AreEqual(LogEntry.LogSeverity.Warning, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }


        [Test]
        public void ErrorTest()
        {
            Error(_message);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.AreEqual(LogEntry.LogSeverity.Error, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }


        [Test]
        public void FatalTest()
        {
            Fatal(_message);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.AreEqual(LogEntry.LogSeverity.Fatal, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }

        [Test]
        public void DebugWithExceptionTest()
        {
            Debug(_message, _exception);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.IsTrue(match.Message.Contains(_exception.Message), $"Expected log message to contain exception message {_exception.Message}");
            Assert.AreEqual(LogEntry.LogSeverity.Debug, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }



        [Test]
        public void InfoWithExceptionTest()
        {
            Info(_message, _exception);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.IsTrue(match.Message.Contains(_exception.Message), $"Expected log message to contain exception message {_exception.Message}");
            Assert.AreEqual(LogEntry.LogSeverity.Info, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }


        [Test]
        public void WarnWithExceptionTest()
        {
            Warn(_message, _exception);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.IsTrue(match.Message.Contains(_exception.Message), $"Expected log message to contain exception message {_exception.Message}");
            Assert.AreEqual(LogEntry.LogSeverity.Warning, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }


        [Test]
        public void ErrorWithExceptionTest()
        {
            Error(_message, _exception);
            var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
            Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
            Assert.IsTrue(match.Message.Contains(_exception.Message), $"Expected log message to contain exception message {_exception.Message}");
            Assert.AreEqual(LogEntry.LogSeverity.Error, match.Severity, "Log entry was created with incorrect log severity.");
            Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
            var now = DateTime.Now;
            var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
            Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        }

        // TODO: Figure out why this test tricks Nunit/Resharper into thinking that a test has failed.
        //[Test]
        //public void FatalWithException()
        //{
        //    Fatal(_message, _exception);
        //    var match = LogEntries.SingleOrDefault(x => x.Message.Contains(_message));
        //    Assert.IsNotNull(match, $"Could not find log entry with message containing {_message}.");
        //    Assert.IsTrue(match.Message.Contains(_exception.Message), $"Expected log message to contain exception message {_exception.Message}");
        //    Assert.AreEqual(LogEntry.LogSeverity.Fatal, match.Severity, "Log entry was created with incorrect log severity.");
        //    Assert.AreEqual(GetType(), match.LogSourceType, "Log entry was created with incorrect source type");
        //    var now = DateTime.Now;
        //    var differenceInTimeInSeconds = (now - match.EntryDate).TotalSeconds;
        //    Assert.IsTrue(Math.Abs(differenceInTimeInSeconds) < 5, $"Expected datetime of {now}, but found {match.EntryDate}, wich differs by {differenceInTimeInSeconds} seconds.");
        //}


        [Test]
        public void WriteAllLogEntriesToFile_ShouldWriteAllEntriesToATextFileOrderedByDate()
        {
            var logEntry1 = Substitute.For<ILogEntry>();
            logEntry1.EntryDate.Returns(new DateTime(1999, 1, 1));
            logEntry1.FormattedLogLine.Returns("entry1");
            var logEntry2 = Substitute.For<ILogEntry>();
            logEntry2.EntryDate.Returns(new DateTime(2000, 2, 2));
            logEntry2.FormattedLogLine.Returns("entry2");
            var logEntry3 = Substitute.For<ILogEntry>();
            logEntry3.EntryDate.Returns(new DateTime(2001, 3, 3));
            logEntry3.FormattedLogLine.Returns("entry3");
            var testFile = "C:\\temp\\" + Guid.NewGuid();

            string pathWrittenTo = "NOT YET SET";
            string[] textWritten = null;

            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.When(x=>x.WriteAllLinesAsUtf8TextFile(testFile, Arg.Any<IEnumerable<string>>())).Do(
                y =>
                {
                    pathWrittenTo = y[0].ToString();
                    textWritten = ((IEnumerable<string>) y[1]).ToArray();
                });

            AllLogEntries.Clear();
            AllLogEntries.AddRange(new []{logEntry2, logEntry1, logEntry3});

            WriteAllLogEntriesToFile(testFile, fileSystem);

            Assert.AreEqual(testFile, pathWrittenTo, "Logs were written to an unexpected path.");
            Assert.IsNotNull(textWritten,"Could not determine text written to log file.");
            Assert.AreEqual(3, textWritten.Length, $"Expected three entries to be written to the text file.  Instead, recieved [{string.Join(",", textWritten)}]");
            Assert.AreEqual("entry1",textWritten[0], "Unexpected first log line");
            Assert.AreEqual("entry2",textWritten[1], "Unexpected second log line");
            Assert.AreEqual("entry3",textWritten[2], "Unexpected third log line");
        }
    }
}
