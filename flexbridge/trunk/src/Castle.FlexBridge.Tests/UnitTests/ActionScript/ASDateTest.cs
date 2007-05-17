// Copyright 2007 Castle Project - http://www.castleproject.org/
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Xml;
using Castle.FlexBridge.ActionScript;
using Castle.FlexBridge.Tests.UnitTests;
using MbUnit.Framework;

namespace Castle.FlexBridge.Tests.UnitTests.ActionScript
{
    [TestFixture]
    [TestsOn(typeof(ASDate))]
    [Author("Jeff", "jeff@ingenio.com")]
    public class ASDateTest : BaseUnitTest
    {
        [Test]
        public void ConstructorWithDateTime_Utc()
        {
            DateTime then = new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Utc);
            ASDate asDate = new ASDate(then);

            Assert.AreEqual(86400000.0, asDate.MillisecondsSinceEpoch);
            Assert.AreEqual(0, asDate.TimeZoneOffsetMinutes);
        }

        [Test]
        public void ConstructorWithDateTime_NonUtc()
        {
            DateTime then = new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Local);
            ASDate asDate = new ASDate(then);

            int tzOffset = (int) TimeZone.CurrentTimeZone.GetUtcOffset(then).TotalMinutes;
            Assert.AreEqual(86400000.0 + tzOffset * 60000, asDate.MillisecondsSinceEpoch);
            Assert.AreEqual(tzOffset, asDate.TimeZoneOffsetMinutes);
        }

        [Test]
        public void ConstructorWithDateTimeAndTimeZoneOffset()
        {
            DateTime then = new DateTime(1970, 1, 2, 0, 0, 0, DateTimeKind.Local);
            ASDate asDate = new ASDate(then, -600);

            Assert.AreEqual(86400000.0, asDate.MillisecondsSinceEpoch);
            Assert.AreEqual(-600, asDate.TimeZoneOffsetMinutes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorWithDateTimeAndTimeZoneOffset_ThrowsIfTZOutOfRange()
        {
            new ASDate(DateTime.Now, 2000);
        }

        [Test]
        public void ConstructorWithMillisAndTimeZoneOffset()
        {
            ASDate asDate = new ASDate(123.0, 456);

            Assert.AreEqual(123.0, asDate.MillisecondsSinceEpoch);
            Assert.AreEqual(456, asDate.TimeZoneOffsetMinutes);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ConstructorWithMillisAndTimeZoneOffset_ThrowsIfTZOutOfRange()
        {
            new ASDate(123.0, 2000);
        }

        [Test]
        public void ToDateTimeIsARoundTripOperationExceptForLossOfPrecision_Utc()
        {
            DateTime now = new DateTime(1999, 12, 31, 0, 0, 0, DateTimeKind.Utc);
            ASDate date = new ASDate(now);

            Assert.AreEqual(now, date.ToDateTime());
        }

        [Test]
        public void ToDateTimeIsARoundTripOperationExceptForLossOfPrecision_NonUtc()
        {
            DateTime now = new DateTime(1999, 12, 31, 0, 0, 0, DateTimeKind.Local);
            ASDate date = new ASDate(now);

            Assert.AreEqual(now, date.ToDateTime());
        }

        [Test]
        public void GetHashCodeIsSane()
        {
            ASDate date1 = new ASDate(12.0, 120);
            ASDate date2 = new ASDate(12.0, 120);

            Assert.AreEqual(date1.GetHashCode(), date2.GetHashCode());
        }

        [Test]
        public void EqualsWithNullIsFalse()
        {
            ASDate date = new ASDate(DateTime.Now);
            Assert.AreNotEqual(date, null);
        }

        [Test]
        public void EqualsWithNonDateIsFalse()
        {
            ASDate date = new ASDate(DateTime.Now);
            Assert.AreNotEqual(date, 42);
        }

        [RowTest]
        [Row(12.0, 120, 12.0, 120, true)]
        [Row(12.0, 120, 10.0, 120, false)]
        [Row(12.0, 120, 12.0, 100, false)]
        public void EqualsWithOtherDate(double millis1, int tzoffset1, double millis2, int tzoffset2, bool expectedResult)
        {
            ASDate date1 = new ASDate(millis1, tzoffset1);
            ASDate date2 = new ASDate(millis2, tzoffset2);
            Assert.AreEqual(expectedResult, date1.Equals(date2));
        }
    }
}