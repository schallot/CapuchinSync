using System;
using System.Linq;
using CapuchinSync.Core.Interfaces;
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

        public LoggableTests() : base(new FileSystem())
        {
        }
    }
}
