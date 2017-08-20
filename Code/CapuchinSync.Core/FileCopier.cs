using System;
using CapuchinSync.Core.Interfaces;

namespace CapuchinSync.Core
{
    public class FileCopier : Loggable, IFileCopier
    {
        private readonly string _source;
        private readonly string _destination;
        private readonly IFileSystem _fileSystem;
        private readonly IPathUtility _pathUtility;

        public bool SuccesfullyCopied { get; private set; }

        public FileCopier(IFileSystem fileSystem, IPathUtility pathUtility, string source, string destination)
        {
            if(fileSystem == null) throw new ArgumentNullException(nameof(fileSystem));
            if(pathUtility == null) throw new ArgumentNullException(nameof(pathUtility));
            Debug($"Creating instance of {typeof(FileCopier)} - source:{source}, destination:{destination}");
            _fileSystem = fileSystem;
            _source = source;
            _destination = destination;
            _pathUtility = pathUtility;
            SuccesfullyCopied = false;
        }

        public void PerformCopy()
        {
            try
            {
                TryCopy();
            }
            catch (Exception e)
            {
                Error($"Failed to copy {_source} to {_destination}.", e);
            }
        }

        private void TryCopy()
        {     
            Info($"Copying file {_source} to {_destination}");       
            var destDir = _pathUtility.GetParentDirectoryFromPath(_destination);
            if (string.IsNullOrEmpty(destDir))
            {
                Error($"Failed to copy {_source} to {_destination}: Could not determine destination directory.");
                return;
            }
            if (!_fileSystem.DoesDirectoryExist(destDir))
            {
                Debug($"Creating directory {destDir} so that we can copy {_source} to {_destination}");
                try
                {
                    _fileSystem.CreateDirectory(destDir);
                }
                catch (Exception e)
                {
                    Error($"Failed to copy {_source} to {_destination}: Could not create destination directory at {destDir}.", e);
                    return;
                }
            }
            try
            {
                _fileSystem.CopyFileAndOverwriteIfItExists(_source, _destination);
            }
            catch (Exception e)
            {
                Error($"Failed to copy {_source} to {_destination}", e);
                return;
            }
            Debug($"Successfully copied file from {_source} to {_destination}");
            SuccesfullyCopied = true;
        }
    }
}
