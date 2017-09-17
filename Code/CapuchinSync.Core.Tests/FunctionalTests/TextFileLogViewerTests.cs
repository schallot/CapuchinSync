using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CapuchinSync.Core.Interfaces;
using NSubstitute;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    [TestFixture]
    public class TextFileLogViewerTests : FileTestBase
    {
        [Test]
        public void ViewLogs_ShouldWriteAndViewLogsAsTextFileUsingSuppliedExe()
        {
            var logEntry1 = Substitute.For<ILogEntry>();
            logEntry1.EntryDate.Returns(new DateTime(1999, 1, 1));
            logEntry1.FormattedLogLine.Returns("entry1");
            var textViewerPath = "C:\\temp\\blahViewer.exe";
            var logFilePath = "C:\\temp\\" + Guid.NewGuid();

            var pathUtility = Substitute.For<IPathUtility>();
            pathUtility.GetTempFileName().Returns(logFilePath);
            
            string pathWrittenTo = "NOT YET SET";
            string[] textWritten = null;

            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.When(x => x.WriteAllLinesAsUtf8TextFile(logFilePath, Arg.Any<IEnumerable<string>>())).Do(
                y =>
                {
                    pathWrittenTo = y[0].ToString();
                    textWritten = ((IEnumerable<string>)y[1]).ToArray();
                });

            fileSystem.DoesFileExist(textViewerPath).Returns(true);
            fileSystem.DoesFileExist(@"C:\Program Files (x86)\Notepad++\notepad++.exe").Returns(false);

            string exeCalled = "Not Yet Set";
            string arguments = "Not Yet Set";
            var processStarter = Substitute.For<IProcessStarter>();
            var process = new Process();
            processStarter.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(process).AndDoes(y =>
            {
                exeCalled = y[0].ToString();
                arguments = y[1].ToString();
            });


            var viewer = new TextFileLogViewer(pathUtility, fileSystem, processStarter, textViewerPath);
            
            Loggable.AllLogEntries.Clear();
            Loggable.AllLogEntries.Add(logEntry1);

            viewer.ViewLogs(new []{logEntry1});

            Assert.AreEqual(logFilePath, pathWrittenTo, "Logs were written to an unexpected path.");
            Assert.IsNotNull(textWritten, "Could not determine text written to log file.");
            Assert.IsTrue(textWritten.Contains("entry1"), $"Expected entry \"entry1\" to be written to the text file.  Instead, recieved [{string.Join(",", textWritten)}]");

            Assert.AreEqual(textViewerPath, exeCalled, "Unexpected exe called");
            Assert.AreEqual(logFilePath, arguments, "Unexpected arguments");
        }

        [Test]
        public void ViewLogs_ShouldWriteAndViewLogsAsTextFileUsingNotepadPlusPlus()
        {
            var logEntry1 = Substitute.For<ILogEntry>();
            logEntry1.EntryDate.Returns(new DateTime(1999, 1, 1));
            logEntry1.FormattedLogLine.Returns("entry1");
            var logFilePath = "C:\\temp\\" + Guid.NewGuid();

            var pathUtility = Substitute.For<IPathUtility>();
            pathUtility.GetTempFileName().Returns(logFilePath);

            string pathWrittenTo = "NOT YET SET";
            string[] textWritten = null;

            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.When(x => x.WriteAllLinesAsUtf8TextFile(logFilePath, Arg.Any<IEnumerable<string>>())).Do(
                y =>
                {
                    pathWrittenTo = y[0].ToString();
                    textWritten = ((IEnumerable<string>)y[1]).ToArray();
                });

            string notepadPlusPlusPath = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
            fileSystem.DoesFileExist(notepadPlusPlusPath).Returns(true);

            string exeCalled = "Not Yet Set";
            string arguments = "Not Yet Set";
            var processStarter = Substitute.For<IProcessStarter>();
            var process = new Process();
            processStarter.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(process).AndDoes(y =>
            {
                exeCalled = y[0].ToString();
                arguments = y[1].ToString();
            });

            var viewer = new TextFileLogViewer(pathUtility, fileSystem, processStarter);

            Loggable.AllLogEntries.Clear();
            Loggable.AllLogEntries.Add(logEntry1);

            viewer.ViewLogs(new[] { logEntry1 });

            Assert.AreEqual(logFilePath, pathWrittenTo, "Logs were written to an unexpected path.");
            Assert.IsNotNull(textWritten, "Could not determine text written to log file.");
            Assert.IsTrue(textWritten.Contains("entry1"), $"Expected entry \"entry1\" to be written to the text file.  Instead, recieved [{string.Join(",", textWritten)}]");

            Assert.AreEqual(notepadPlusPlusPath, exeCalled, "Unexpected exe called");
            Assert.AreEqual(logFilePath, arguments, "Unexpected arguments");
        }

        [Test]
        public void ViewLogs_ShouldWriteAndViewLogsAsTextFileUsingNotepad()
        {
            var logEntry1 = Substitute.For<ILogEntry>();
            logEntry1.EntryDate.Returns(new DateTime(1999, 1, 1));
            logEntry1.FormattedLogLine.Returns("entry1");
            var logFilePath = "C:\\temp\\" + Guid.NewGuid();

            var pathUtility = Substitute.For<IPathUtility>();
            pathUtility.GetTempFileName().Returns(logFilePath);

            string pathWrittenTo = "NOT YET SET";
            string[] textWritten = null;

            var fileSystem = Substitute.For<IFileSystem>();
            fileSystem.When(x => x.WriteAllLinesAsUtf8TextFile(logFilePath, Arg.Any<IEnumerable<string>>())).Do(
                y =>
                {
                    pathWrittenTo = y[0].ToString();
                    textWritten = ((IEnumerable<string>)y[1]).ToArray();
                });

            string notepadPlusPlusPath = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
            fileSystem.DoesFileExist(notepadPlusPlusPath).Returns(false);

            string exeCalled = "Not Yet Set";
            string arguments = "Not Yet Set";
            var processStarter = Substitute.For<IProcessStarter>();
            var process = new Process();
            processStarter.Start(Arg.Any<string>(), Arg.Any<string>()).Returns(process).AndDoes(y =>
            {
                exeCalled = y[0].ToString();
                arguments = y[1].ToString();
            });


            var viewer = new TextFileLogViewer(pathUtility, fileSystem, processStarter);

            Loggable.AllLogEntries.Clear();
            Loggable.AllLogEntries.Add(logEntry1);

            viewer.ViewLogs(new[] { logEntry1 });

            Assert.AreEqual(logFilePath, pathWrittenTo, "Logs were written to an unexpected path.");
            Assert.IsNotNull(textWritten, "Could not determine text written to log file.");
            Assert.IsTrue(textWritten.Contains("entry1"), $"Expected entry \"entry1\" to be written to the text file.  Instead, recieved [{string.Join(",", textWritten)}]");

            Assert.AreEqual("NOTEPAD", exeCalled.ToUpperInvariant(), "Unexpected exe called");
            Assert.AreEqual(logFilePath, arguments, "Unexpected arguments");
        }
    }
}
