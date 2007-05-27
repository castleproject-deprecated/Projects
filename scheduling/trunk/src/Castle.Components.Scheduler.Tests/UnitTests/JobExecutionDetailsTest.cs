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
using System.Collections.Generic;
using MbUnit.Framework;

namespace Castle.Components.Scheduler.Tests.UnitTests
{
    [TestFixture]
    [TestsOn(typeof(JobExecutionDetails))]
    [Author("Jeff Brown", "jeff@ingenio.com")]
    public class JobExecutionDetailsTest : BaseUnitTest
    {
        [Test]
        public void ConstructorSetsProperties()
        {
            DateTime now = DateTime.Now;
            JobExecutionDetails details = new JobExecutionDetails("foo", now);

            Assert.AreEqual("foo", details.SchedulerName);
            Assert.AreEqual(now, details.StartTime);
            Assert.IsNull(details.EndTime);
            Assert.IsFalse(details.Succeeded);
            Assert.AreEqual("Unknown", details.StatusMessage);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsIfSchedulerNameIsNull()
        {
            new JobExecutionDetails(null, DateTime.Now);
        }

        [Test]
        public void EndTime_GetterAndSetter()
        {
            JobExecutionDetails details = new JobExecutionDetails("foo", DateTime.Now);

            details.EndTime = new DateTime(1970, 1, 2);
            Assert.AreEqual(new DateTime(1970, 1, 2), details.EndTime);
        }

        [Test]
        public void Succeeded_GetterAndSetter()
        {
            JobExecutionDetails details = new JobExecutionDetails("foo", DateTime.Now);

            details.Succeeded = true;
            Assert.IsTrue(details.Succeeded);
        }

        [Test]
        public void StatusMessage_GetterAndSetter()
        {
            JobExecutionDetails details = new JobExecutionDetails("foo", DateTime.Now);

            details.StatusMessage = "Test test test";
            Assert.AreEqual("Test test test", details.StatusMessage);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void StatusMessage_ThrowsIfValueIsNull()
        {
            JobExecutionDetails details = new JobExecutionDetails("foo", DateTime.Now);
            details.StatusMessage = null;
        }

        [RowTest]
        [Row(false)]
        [Row(true)]
        public void ClonePerformsADeepCopy(bool useGenericClonable)
        {
            DateTime now = DateTime.Now;
            JobExecutionDetails details = new JobExecutionDetails("foo", now);
            details.EndTime = DateTime.MaxValue;
            details.Succeeded = true;
            details.StatusMessage = "Blah";

            JobExecutionDetails clone = useGenericClonable ? details.Clone()
                : (JobExecutionDetails)((ICloneable)details).Clone();

            Assert.AreNotSame(details, clone);

            Assert.AreEqual("foo", clone.SchedulerName);
            Assert.AreEqual(now, clone.StartTime);
            Assert.AreEqual(DateTime.MaxValue, clone.EndTime);
            Assert.IsTrue(clone.Succeeded);
            Assert.AreEqual("Blah", clone.StatusMessage);
        }
    }
}