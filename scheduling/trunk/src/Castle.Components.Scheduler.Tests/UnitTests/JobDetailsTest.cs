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
    [TestsOn(typeof(JobDetails))]
    [Author("Jeff Brown", "jeff@ingenio.com")]
    public class JobDetailsTest : BaseUnitTest
    {
        private JobSpec jobSpec = new JobSpec("abc", "some job", "with.this.key", PeriodicTrigger.CreateDailyTrigger(DateTime.Now));

        [Test]
        public void ConstructorSetsProperties()
        {
            DateTime now = DateTime.Now;
            JobDetails jobDetails = new JobDetails(jobSpec, now);
            Assert.AreSame(jobSpec, jobDetails.JobSpec);
            Assert.AreEqual(now, jobDetails.CreationTime);
            Assert.AreEqual(JobState.Pending, jobDetails.JobState);
            Assert.IsNull(jobDetails.JobData);
            Assert.IsNull(jobDetails.NextTriggerFireTime);
            Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
            Assert.IsNull(jobDetails.LastJobExecutionDetails);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ConstructorThrowsWhenJobSpecIsNull()
        {
            new JobDetails(null, DateTime.Now);
        }

        [Test]
        public void JobState_GetterAndSetter()
        {
            JobDetails jobDetails = new JobDetails(jobSpec, DateTime.Now);

            jobDetails.JobState = JobState.Scheduled;
            Assert.AreEqual(JobState.Scheduled, jobDetails.JobState);
        }

        [Test]
        public void NextTriggerFireTime_GetterAndSetter()
        {
            JobDetails jobDetails = new JobDetails(jobSpec, DateTime.Now);

            jobDetails.NextTriggerFireTime = new DateTime(1970, 1, 1);
            Assert.AreEqual(new DateTime(1970, 1, 1), jobDetails.NextTriggerFireTime);
        }

        [Test]
        public void NextTriggerMisfireThreshold_GetterAndSetter()
        {
            JobDetails jobDetails = new JobDetails(jobSpec, DateTime.Now);

            jobDetails.NextTriggerMisfireThreshold = new TimeSpan(0, 1, 0);
            Assert.AreEqual(new TimeSpan(0, 1, 0), jobDetails.NextTriggerMisfireThreshold);
        }

        [Test]
        public void LastJobExecutionDetails_GetterAndSetter()
        {
            JobDetails jobDetails = new JobDetails(jobSpec, DateTime.Now);

            JobExecutionDetails jobExecutionDetails = new JobExecutionDetails("foo", DateTime.Now);
            jobDetails.LastJobExecutionDetails = jobExecutionDetails;
            Assert.AreSame(jobExecutionDetails, jobDetails.LastJobExecutionDetails);
        }

        [Test]
        public void JobData_GetterAndSetter()
        {
            JobDetails jobDetails = new JobDetails(jobSpec, DateTime.Now);

            JobData jobData = new JobData();
            jobDetails.JobData = jobData;
            Assert.AreSame(jobData, jobDetails.JobData);
        }

        [RowTest]
        [Row(false)]
        [Row(true)]
        public void ClonePerformsADeepCopy(bool useGenericClonable)
        {
            JobDetails jobDetails = new JobDetails(jobSpec, DateTime.Now);
            jobDetails.JobData = new JobData();
            jobDetails.LastJobExecutionDetails = new JobExecutionDetails("foo", DateTime.Now);
            jobDetails.JobState = JobState.Scheduled;
            jobDetails.NextTriggerFireTime = DateTime.Now;
            jobDetails.NextTriggerMisfireThreshold = TimeSpan.MaxValue;

            JobDetails clone = useGenericClonable ? jobDetails.Clone()
                : (JobDetails)((ICloneable)jobDetails).Clone();

            Assert.AreNotSame(jobDetails, clone);

            Assert.IsNotNull(clone.JobSpec);
            Assert.AreNotSame(jobDetails.JobSpec, clone.JobSpec);
            Assert.IsNotNull(clone.JobData);
            Assert.AreNotSame(jobDetails.JobData, clone.JobData);
            Assert.IsNotNull(clone.LastJobExecutionDetails);
            Assert.AreNotSame(jobDetails.LastJobExecutionDetails, clone.LastJobExecutionDetails);
            Assert.AreEqual(jobDetails.JobState, clone.JobState);
            Assert.AreEqual(jobDetails.NextTriggerFireTime, clone.NextTriggerFireTime);
            Assert.AreEqual(jobDetails.NextTriggerMisfireThreshold, clone.NextTriggerMisfireThreshold);
        }

    }
}