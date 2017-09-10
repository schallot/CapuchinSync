using System;
using System.Collections.Generic;
using System.IO;
using CapuchinSync.Core.Hashes;
using CapuchinSync.Core.Interfaces;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    [TestFixture]
    public abstract class FileTestBase
    {
        protected string TestSourceFolder { get; private set; }
        protected string TestDestinationFolder { get; private set; }
        protected IFileSystem FileSystem { get; set; }
        protected IPathUtility PathUtility { get; set; }
        protected IHashUtility HashUtility { get; set; }
        protected IFileCopierFactory FileCopierFactory { get; set; }
        protected IHashDictionaryFactory HashDictionaryFactory { get; set; }

        [SetUp]
        public void BaseSetup()
        {
            FileSystem = new FileSystem();
            PathUtility = new PathUtility();
            HashUtility = new Sha1Hash();
            FileCopierFactory = new FileCopierFactory();
            HashDictionaryFactory = new HashDictionaryFactory(HashUtility, TestSourceFolder);
            var tempDir = Path.GetTempPath();
            var rootTestDir = Path.Combine(tempDir, "DirectoryHashSyncTests");
            if (!Directory.Exists(rootTestDir))
            {
                Directory.CreateDirectory(rootTestDir);
            }
            var guid = Guid.NewGuid().ToString();
            TestSourceFolder = Path.Combine(rootTestDir, guid + "_source");
            TestDestinationFolder = Path.Combine(rootTestDir, guid + "_destination");
            EnsureFolderExists(TestSourceFolder);
            EnsureFolderExists(TestDestinationFolder);
        }

        [TearDown]
        public void TearDown()
        {
            try
            {
                if(Directory.Exists(TestSourceFolder))
                File.Delete(TestSourceFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to delete test folder at {TestSourceFolder}.\r\n\t{e}");
            }
            try
            {
                if (Directory.Exists(TestDestinationFolder))
                    File.Delete(TestDestinationFolder);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to delete test folder at {TestDestinationFolder}.\r\n\t{e}");
            }
        }


        public static string CreateFileWithFullPath(string fullPath, string contents)
        {
            var containingDir = Path.GetDirectoryName(fullPath);
            EnsureFolderExists(containingDir);
            File.WriteAllText(fullPath, contents);
            return fullPath;
        }

        public string CreateFile(string fileName, string contents)
        {
            var fullPath = Path.Combine(TestSourceFolder, fileName);
            return CreateFileWithFullPath(fullPath, contents);
        }

        protected string CreateFile(TestFile file)
        {
            return CreateFileWithFullPath(file.FilePath, file.Contents);
        }

        private static void EnsureFolderExists(string folderPath)
        {
            if (Directory.Exists(folderPath)) return;
            var parent = Path.GetDirectoryName(folderPath);
            EnsureFolderExists(parent);
            Directory.CreateDirectory(folderPath);
        }

        protected void WriteTestSet(IEnumerable<TestFile> testFiles)
        {
            foreach (var testFile in testFiles)
            {
                CreateFile(testFile.FilePath, testFile.Contents);
            }
        }

        protected TestFile[] TestSet1 => new[]
        {
            new TestFile
            {
                Contents = "Test file 1",
                RelativePath = "File1.txt",            
                RootDirectory = TestSourceFolder,
                Hash = "6E1BC9BFB99D6270DFD57B5CF24569D06E68A6FB"
            },
            new TestFile
            {
                Contents = "Another test file.  This is number two, located in a subdirectory.",
                RelativePath = "ASubDir\\Another sub directory\\file3",
                RootDirectory = TestSourceFolder,
                Hash = "BFFC96489DE2145A1B19BEDAF385CB584933E481"
            },
            new TestFile
            {
                Contents = "Yet another test file (This is the third one).  It'll be named weirdly.",
                RelativePath =  ".blah",
                RootDirectory = TestSourceFolder,
                Hash = "440093005F093B313DC443D8826FF735FA679EB0"
            },
            new TestFile
            {
                Contents = "You guessed it, another test file (This is the fourth one).  It'll be just another file.",
                RelativePath = "test4.log",
                RootDirectory = TestSourceFolder,
                Hash = "B336E05047990BB76E0EC59E6C86AE3BACE4B440"
            },
            new TestFile
            {
                Contents = "Yet another test file (This is the fifth one).",
                RelativePath = "test5.bat",
                RootDirectory = TestSourceFolder,
                Hash = "120BC5768FFFB1C27CA5C03A3D413863186D6A0A"
            }
        };

        protected class TestFile
        {
            public string RelativePath { get; set; }
            public string FilePath => Path.Combine(RootDirectory, RelativePath);
            public string Contents { get; set; }
            public string Hash { get; set; }
            public string RootDirectory { get; set; }

            public string HashFileLine
            {
                get
                {
                    return $"{Hash}{HashDictionaryEntry.Delimiter}{RelativePath}";
                }
            }
        }

    }
}
