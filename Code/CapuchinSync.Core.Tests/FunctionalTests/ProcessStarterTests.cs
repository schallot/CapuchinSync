using System.IO;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests.FunctionalTests
{
    [TestFixture]
    public class ProcessStarterTests : FileTestBase
    {
        [Test]
        public void StartTest()
        {
            var starter = new ProcessStarter();
            var exe = "cmd.exe";
            var args = "/C dir /a C:\\";
            var process  = starter.Start(exe, args);

            Assert.IsNotNull(process, "process != null");
            var info = process.StartInfo;
            Assert.AreEqual(exe, info.FileName, "Unexpected executable");
            Assert.AreEqual(args, info.Arguments, "Unexpected arguments");
        }
    }
}