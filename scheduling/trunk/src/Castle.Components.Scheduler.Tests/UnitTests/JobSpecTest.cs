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
using MbUnit.Framework;

namespace Castle.Components.Scheduler.Tests.UnitTests
{
    [TestFixture]
    [TestsOn(typeof(JobSpec))]
    [Author("Jeff Brown", "jeff@ingenio.com")]
    public class JobSpecTest : BaseUnitTest
    {
        private Trigger trigger = PeriodicTrigger.CreateDailyTrigger(DateTime.UtcNow);

        [Test]
        public void ConstructorSetsProperties()
        {
            JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);
            Assert.AreEqual("abc", spec.Name);
            Assert.AreEqual("some job", spec.Description);
            Assert.AreEqual("with this key", spec.JobKey);
            Assert.AreSame(trigger, spec.Trigger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsWhenJobNameIsNull()
        {
            new JobSpec(null, "some job", "with this key", trigger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void ConstructorThrowsWhenJobNameIsEmpty()
        {
            new JobSpec("", "some job", "with this key", trigger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsWhenJobDescriptionIsNull()
        {
            new JobSpec("abc", null, "with this key", trigger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsWhenJobKeyIsNull()
        {
            new JobSpec("abc", "some job", null, trigger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsWhenTriggerIsNull()
        {
            new JobSpec("abc", "some job", "with this key", null);
        }

        [RowTest]
        [Row(false)]
        [Row(true)]
        public void ClonePerformsADeepCopy(bool useGenericClonable)
        {
            JobSpec spec = new JobSpec("abc", "some job", "with this key", trigger);
            JobSpec clone = useGenericClonable ? spec.Clone()
                : (JobSpec) ((ICloneable) spec).Clone();

            Assert.AreNotSame(spec, clone);

            Assert.AreEqual("abc", clone.Name);
            Assert.AreEqual("some job", clone.Description);
            Assert.AreEqual("with this key", clone.JobKey);
            Assert.IsNotNull(clone.Trigger);
            Assert.AreNotSame(trigger, clone.Trigger);
        }
    }
}