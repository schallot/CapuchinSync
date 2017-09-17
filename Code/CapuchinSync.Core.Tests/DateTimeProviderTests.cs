using System;
using NUnit.Framework;

namespace CapuchinSync.Core.Tests
{
    [TestFixture]
    public class DateTimeProviderTests
    {
        private DateTime _dateTime;
        private DateTimeProvider _provider;

        [SetUp]
        public void SetUp()
        {
            _dateTime = new DateTime(1999,1,2,3,4,5,6);
            _provider = new DateTimeProvider();
            ;
        }

        [Test]
        public void GetTimeStringTest()
        {
            Assert.AreEqual("3:04:05 AM", _provider.GetTimeString(_dateTime));
        }

        [Test]
        public void GetDateStringTest()
        {
            Assert.AreEqual("Saturday, January 2, 1999", _provider.GetDateString(_dateTime));
        }

        [Test]
        public void NowTest()
        {
            var expectedNow = DateTime.Now;
            var actual = _provider.Now;
            var difference = expectedNow - actual;
            int acceptableDifferenceInMilliseconds = 1000;
            Assert.IsTrue(difference.TotalMilliseconds <= acceptableDifferenceInMilliseconds && difference.TotalMilliseconds >= -acceptableDifferenceInMilliseconds, 
                $"Expected provider's Now property to match DateTime.Now's value within {acceptableDifferenceInMilliseconds} milliseconds, but was actually {difference.TotalMilliseconds}");
        }
    }
}