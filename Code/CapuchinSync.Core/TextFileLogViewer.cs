using System.Collections.Generic;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class TextFileLogViewer : Loggable, ILogViewer
    {
        private readonly IPathUtility _pathUtility;
        private readonly IFileSystem _fileSystem;
        private readonly string _pathToTextEditor;

        public TextFileLogViewer(IPathUtility pathUtility, IFileSystem fileSystem, string pathToTextEditor = "")
        {
            _pathUtility = pathUtility;
            _fileSystem = fileSystem;
            _pathToTextEditor = pathToTextEditor;
        }

        public void ViewLogs(IEnumerable<LogEntry> logs)
        {
            var tempFile = _pathUtility.GetTempFileName();
            Info($"Writing log file to {tempFile}");

            string textEditor = _pathToTextEditor;
            if (string.IsNullOrWhiteSpace(textEditor) || !_fileSystem.DoesFileExist(textEditor))
            {
                textEditor = @"C:\Program Files (x86)\Notepad++\notepad++.exe";
            }
            if (!_fileSystem.DoesFileExist(textEditor))
            {
                textEditor = "notepad";
            }
            WriteAllLogEntriesToFile(tempFile, _fileSystem);
            {
                System.Diagnostics.Process.Start(textEditor, tempFile);
            }
        }
    }
}