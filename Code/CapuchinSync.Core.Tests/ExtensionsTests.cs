using NUnit.Framework;

namespace CapuchinSync.Core.Tests
{
    [TestFixture]
    public class ExtensionsTests
    {
        [Test]
        [TestCase('0',true)]
        [TestCase('1',true)]
        [TestCase('2',true)]
        [TestCase('3',true)]
        [TestCase('4',true)]
        [TestCase('5',true)]
        [TestCase('6',true)]
        [TestCase('7',true)]
        [TestCase('8',true)]
        [TestCase('9',true)]
        [TestCase('a',true)]
        [TestCase('b',true)]
        [TestCase('c',true)]
        [TestCase('d',true)]
        [TestCase('e',true)]
        [TestCase('f',true)]
        [TestCase('A',true)]
        [TestCase('B',true)]
        [TestCase('C',true)]
        [TestCase('D',true)]
        [TestCase('E',true)]
        [TestCase('F',true)]
        [TestCase('g',false)]
        [TestCase('j',false)]
        [TestCase('r',false)]
        [TestCase('w',false)]
        [TestCase('z',false)]
        [TestCase('G',false)]
        [TestCase('K',false)]
        [TestCase('M',false)]
        [TestCase('R',false)]
        [TestCase('V',false)]
        [TestCase('X',false)]
        [TestCase('Z',false)]
        [TestCase('$',false)]
        [TestCase('@',false)]
        [TestCase('*',false)]
        [TestCase('~',false)]
        public void IsHexTest(char value, bool expectedResult)
        {
            var result = value.IsHex();
            Assert.AreEqual(expectedResult, result, $"Unexpected result for {value}: Expected {expectedResult}, was {result}");
        }

        [Test]
        [TestCase(Constants.EverythingsJustPeachyReturnCode, true)]
        [TestCase(Constants.EverythingsJustPeachyReturnCode + 1, false)]
        [TestCase(Constants.EverythingsJustPeachyReturnCode + 10000, false)]
        [TestCase(Constants.EverythingsJustPeachyReturnCode - 10000, false)]
        public void NonPeachyReturnCode_ShouldNotBePeachy(int value, bool expectedResult)
        {
            var result = value.IsReturnCodeJustPeachy();
            Assert.AreEqual(expectedResult, result, $"Unexpected result for {value}: Expected {expectedResult}, was {result}");
        }
    }
}