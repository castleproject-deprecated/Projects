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
using System.Text;
using System.Threading;
using Castle.Components.Scheduler.JobStores;
using Castle.Components.Scheduler.Tests.Utilities;
using Castle.Core.Logging;
using MbUnit.Framework;

namespace Castle.Components.Scheduler.Tests.UnitTests.JobStores
{
    /// <summary>
    /// Base tests for a job store.
    /// </summary>
    [TestFixture(TimeOut = 1)]
    [TestsOn(typeof(BaseJobStore))]
    [Author("Jeff Brown", "jeff@ingenio.com")]
    public abstract class BaseJobStoreTest : BaseUnitTest
    {
        protected static readonly Guid SchedulerGuid = Guid.NewGuid();
        protected const string SchedulerName = "test";

        private BaseJobStore jobStore;

        protected JobSpec dummyJobSpec;
        protected JobData dummyJobData;

        /// <summary>
        /// Gets the job store to be tested.
        /// </summary>
        protected BaseJobStore JobStore
        {
            get { return jobStore; }
            set { jobStore = value; }
        }

        public override void SetUp()
        {
            base.SetUp();

            dummyJobSpec = new JobSpec("job", "test", "dummy", PeriodicTrigger.CreateOneShotTrigger(DateTime.UtcNow));
            dummyJobData = new JobData();
            dummyJobData.State["key"] = "value";

            jobStore = CreateJobStore();
            jobStore.RegisterScheduler(SchedulerGuid, SchedulerName);
        }

        public override void TearDown()
        {
            if (! jobStore.IsDisposed)
                jobStore.Dispose();

            base.TearDown();
        }

        /// <summary>
        /// Creates the job store to be tested.
        /// </summary>
        /// <returns>The job store</returns>
        protected abstract BaseJobStore CreateJobStore();

        [Test]
        public void Logger_GetterAndSetter()
        {
            Mocks.ReplayAll();

            Assert.AreSame(NullLogger.Instance, jobStore.Logger);

            ILogger mockLogger = Mocks.CreateMock<ILogger>();
            jobStore.Logger = mockLogger;
            Assert.AreSame(mockLogger, jobStore.Logger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Logger_ThrowsIfValueIsNull()
        {
            Mocks.ReplayAll();

            jobStore.Logger = null;
        }

        [Test]
        public void IsDisposed_ChangesToTrueWhenDisposed()
        {
            Mocks.ReplayAll();

            Assert.IsFalse(jobStore.IsDisposed);

            jobStore.Dispose();
            Assert.IsTrue(jobStore.IsDisposed);
        }

        /// <summary>
        /// There are no strong contracts to be checked regarding scheduler registration
        /// with a job store, it is just supposed to happen.  So there isn't much that
        /// we can check generically across all job store implementations.
        /// </summary>
        [Test]
        public void RegisterScheduler_DoesNotCrashWhenCalledRedundantly()
        {
            Mocks.ReplayAll();

            jobStore.RegisterScheduler(SchedulerGuid, SchedulerName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RegisterScheduler_ThrowsIfSchedulerNameIsNull()
        {
            Mocks.ReplayAll();

            jobStore.RegisterScheduler(SchedulerGuid, null);
        }

        [Test]
        public void UnregisterScheduler_OrphansRunningJobs()
        {
            Mocks.ReplayAll();

            JobDetails jobDetails = CreatePendingJob("job", DateTime.UtcNow);
            jobDetails.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);
            jobDetails.JobState = JobState.Running;
            jobStore.SaveJobDetails(jobDetails);

            jobStore.UnregisterScheduler(SchedulerGuid);

            jobDetails = jobStore.GetJobDetails("job");
            Assert.AreEqual(JobState.Orphaned, jobDetails.JobState);
        }

        [RowTest]
        [Row(CreateJobConflictAction.Ignore)]
        [Row(CreateJobConflictAction.Update)]
        [Row(CreateJobConflictAction.Throw)]
        public void CreateJob_ReturnsTrueIfCreated(CreateJobConflictAction conflictAction)
        {
            Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, dummyJobData, DateTime.UtcNow, conflictAction));
        }

        [RowTest]
        [Row(CreateJobConflictAction.Ignore, false)]
        [Row(CreateJobConflictAction.Update, true)]
        [Row(CreateJobConflictAction.Throw, false, ExpectedException=typeof(SchedulerException))]
        public void CreateJob_HandlesDuplicatesAccordingToConflictAction(CreateJobConflictAction conflictAction,
            bool expectedResult)
        {
            Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, dummyJobData, DateTime.UtcNow, CreateJobConflictAction.Ignore));

            Assert.AreEqual(expectedResult, jobStore.CreateJob(dummyJobSpec, dummyJobData, DateTime.UtcNow, conflictAction));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateJob_ThrowsIfJobSpecIsNull()
        {
            jobStore.CreateJob(null, null, DateTime.UtcNow, CreateJobConflictAction.Ignore);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CreateJob_ThrowsIfDisposed()
        {
            jobStore.Dispose();
            jobStore.CreateJob(dummyJobSpec, dummyJobData, DateTime.UtcNow, CreateJobConflictAction.Ignore);
        }

        [Test]
        public void GetJobDetails_ReturnsCorrectDetailsAfterCreateJob()
        {
            DateTime creationTime = DateTime.UtcNow;
            Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, dummyJobData, creationTime, CreateJobConflictAction.Throw));

            JobDetails jobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);
            Assert.IsNotNull(jobDetails);

            JobAssert.AreEqual(dummyJobSpec, jobDetails.JobSpec);
            JobAssert.AreEqual(dummyJobData, jobDetails.JobData);

            JobAssert.AreEqualUpToErrorLimit(creationTime, jobDetails.CreationTime);
            Assert.AreEqual(JobState.Pending, jobDetails.JobState);
            Assert.IsNull(jobDetails.LastJobExecutionDetails);
            Assert.IsNull(jobDetails.NextTriggerFireTime);
            Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
        }

        [Test]
        public void GetJobDetails_ReturnsNullIfJobDoesNotExists()
        {
            Assert.IsNull(jobStore.GetJobDetails("non-existant"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetJobDetails_ThrowsIfJobNameIsNull()
        {
            jobStore.GetJobDetails(null);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetJobDetails_ThrowsIfDisposed()
        {
            jobStore.Dispose();
            jobStore.GetJobDetails("job");
        }

        [Test]
        public void SaveJobDetails_RoundTripWithGetJobDetails()
        {
            // Create job and set details.
            Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, dummyJobData, DateTime.UtcNow, CreateJobConflictAction.Throw));
            JobDetails savedJobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);

            savedJobDetails.JobState = JobState.Running;
            savedJobDetails.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, DateTime.UtcNow);
            savedJobDetails.LastJobExecutionDetails.StatusMessage = "Status";
            savedJobDetails.LastJobExecutionDetails.Succeeded = true;
            savedJobDetails.LastJobExecutionDetails.EndTime = new DateTime(1969, 12, 31);
            savedJobDetails.NextTriggerFireTime = new DateTime(1970, 1, 1);
            savedJobDetails.NextTriggerMisfireThreshold = new TimeSpan(0, 1, 0);
            savedJobDetails.JobData.State["key"] = "new value";
            savedJobDetails.JobSpec.Trigger.Schedule(TriggerScheduleCondition.FirstTime, DateTime.MaxValue);
            dummyJobSpec.Trigger.Schedule(TriggerScheduleCondition.FirstTime, DateTime.MaxValue);

            Assert.IsTrue(savedJobDetails.JobSpec.Trigger.IsDirty);
            jobStore.SaveJobDetails(savedJobDetails);
            Assert.IsFalse(savedJobDetails.JobSpec.Trigger.IsDirty);

            // Check job details.
            JobDetails loadedJobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);
            JobAssert.AreEqual(savedJobDetails, loadedJobDetails);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void SaveJobDetails_ThrowsIfJobStatusIsNull()
        {
            jobStore.SaveJobDetails(null);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void SaveJobDetails_ThrowsIfDisposed()
        {
            jobStore.CreateJob(dummyJobSpec, dummyJobData, DateTime.UtcNow, CreateJobConflictAction.Throw);
            JobDetails savedJobDetails = jobStore.GetJobDetails(dummyJobSpec.Name);

            jobStore.Dispose();
            jobStore.SaveJobDetails(savedJobDetails);
        }

        [Test]
        [ExpectedException(typeof(ConcurrentModificationException))]
        public void SaveJobDetails_ThrowsOnConcurrentDeletion()
        {
            JobDetails job = CreatePendingJob("test", DateTime.UtcNow);
            jobStore.DeleteJob("test");
            
            // Now try to save the job details back again.
            job.JobState = JobState.Stopped;
            jobStore.SaveJobDetails(job);
        }

        [Test]
        [ExpectedException(typeof(ConcurrentModificationException))]
        public void SaveJobDetails_ThrowsOnConcurrentSave()
        {
            JobDetails job = CreatePendingJob("test", DateTime.UtcNow);
            JobDetails modifiedJob = job.Clone();
            modifiedJob.JobState = JobState.Stopped;
            jobStore.SaveJobDetails(modifiedJob);

            // Now try to save the original job details back again.
            job.JobState = JobState.Orphaned;
            jobStore.SaveJobDetails(job);
        }

        [Test]
        public void DeleteJobMakesTheJobInaccessibleToSubsequentGetJobDetails()
        {
            Assert.IsTrue(jobStore.CreateJob(dummyJobSpec, dummyJobData, DateTime.UtcNow, CreateJobConflictAction.Throw));
            Assert.IsTrue(jobStore.DeleteJob(dummyJobSpec.Name));
            Assert.IsNull(jobStore.GetJobDetails(dummyJobSpec.Name));
        }

        [Test]
        public void DeleteJobReturnsFalseIfJobDoesNotExist()
        {
            Assert.IsFalse(jobStore.DeleteJob("does not exist"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteJob_ThrowsIfJobNameIsNull()
        {
            jobStore.DeleteJob(null);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void DeleteJob_ThrowsIfDisposed()
        {
            jobStore.Dispose();
            jobStore.DeleteJob("job");
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CreateJobWatcher_ThrowsIfDisposed()
        {
            jobStore.Dispose();
            jobStore.CreateJobWatcher(SchedulerGuid);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void JobWatcher_UnblocksThreadAndThrowsIfJobStoreIsDisposedAsynchronously()
        {
            IJobWatcher watcher = jobStore.CreateJobWatcher(SchedulerGuid);

            ThreadPool.QueueUserWorkItem(delegate
            {
                Thread.Sleep(2000);
                jobStore.Dispose();
            });
            
            // This call blocks until the dispose runs.
            watcher.GetNextJobToProcess();
        }

        [Test]
        public void JobWatcher_UnblocksWhenScheduledJobBecomesTriggered()
        {
            IJobWatcher watcher = jobStore.CreateJobWatcher(SchedulerGuid);

            DateTime fireTime = DateTime.UtcNow.AddSeconds(3);
            CreateScheduledJob("scheduled", fireTime);

            // Wait for job to become ready.
            Assert.LowerThan(DateTime.UtcNow, fireTime);
            JobDetails triggered = watcher.GetNextJobToProcess();
            Assert.GreaterEqualThan(DateTime.UtcNow, fireTime);

            // Job should come back triggered.
            Assert.AreEqual("scheduled", triggered.JobSpec.Name);
            Assert.AreEqual(JobState.Triggered, triggered.JobState);

            watcher.Dispose();
        }

        [Test]
        public void JobWatcher_UnblocksWhenPendingJobAdded()
        {
            IJobWatcher watcher = jobStore.CreateJobWatcher(SchedulerGuid);

            ThreadPool.QueueUserWorkItem(delegate
            {
                Thread.Sleep(2000);
                CreatePendingJob("pending", DateTime.UtcNow);
            });

            // Wait for job to become ready.
            JobDetails triggered = watcher.GetNextJobToProcess();

            // Job should come back pending.
            Assert.AreEqual("pending", triggered.JobSpec.Name);
            Assert.AreEqual(JobState.Pending, triggered.JobState);

            watcher.Dispose();
        }

        [Test]
        public void JobWatcher_YieldsJobsInExpectedSequence()
        {
            IJobWatcher watcher = jobStore.CreateJobWatcher(SchedulerGuid);

            JobDetails orphaned = CreateOrphanedJob("orphaned", new DateTime(1970, 1, 3));
            JobDetails pending = CreatePendingJob("pending", new DateTime(1970, 1, 2));
            JobDetails triggered = CreateTriggeredJob("triggered", new DateTime(1970, 1, 6));
            JobDetails completed = CreateCompletedJob("completed", new DateTime(1970, 1, 1));
            JobDetails scheduled = CreateScheduledJob("scheduled", new DateTime(1970, 1, 4));

            // Ensure we tolerate a few odd cases where data may not be available like it should.
            JobDetails scheduled2 = CreateScheduledJob("scheduled2", new DateTime(1970, 1, 2));
            scheduled2.NextTriggerFireTime = null;
            jobStore.SaveJobDetails(scheduled2);

            JobDetails completed2 = CreateCompletedJob("completed2", new DateTime(1970, 1, 1));
            completed2.LastJobExecutionDetails = null;
            jobStore.SaveJobDetails(completed2);

            JobDetails orphaned2 = CreateOrphanedJob("orphaned2", new DateTime(1970, 1, 3));
            orphaned2.LastJobExecutionDetails.EndTime = null;
            jobStore.SaveJobDetails(orphaned2);

            // Populate a table of expected jobs.
            List<JobDetails> expectedJobs = new List<JobDetails>(new JobDetails[]
            {
                orphaned, pending, triggered, completed, scheduled, scheduled2, completed2, orphaned2
            });

            // Add in some extra jobs in other states that will not be returned.
            CreateRunningJob("running1");
            CreateStoppedJob("stopped1");
            CreateScheduledJob("scheduled-in-the-future", DateTime.MaxValue);
            
            // Ensure expected jobs are retrieved.
            while (expectedJobs.Count != 0)
            {
                JobDetails actualJob = watcher.GetNextJobToProcess();
                JobDetails expectedJob = expectedJobs.Find(delegate(JobDetails candidate)
                {
                    return candidate.JobSpec.Name == actualJob.JobSpec.Name;
                });
                Assert.IsNotNull(expectedJob, "Did expect job {0}", actualJob.JobSpec.Name);

                // All expected scheduled jobs will have been triggered.
                if (expectedJob.JobState == JobState.Scheduled)
                    expectedJob.JobState = JobState.Triggered;

                JobAssert.AreEqual(expectedJob, actualJob);

                if (expectedJobs.Count == 1)
                {
                    // Ensure same job is returned a second time until its status is changed.
                    // We wait for Count == 1 because that's the easiest case for which to verify
                    // this behavior.
                    JobDetails actualJob2 = watcher.GetNextJobToProcess();
                    JobAssert.AreEqual(expectedJob, actualJob2);
                }

                // Change the status to progress.
                actualJob.JobState = JobState.Stopped;
                jobStore.SaveJobDetails(actualJob);

                expectedJobs.Remove(expectedJob);
            }

            // Ensure next request blocks but is released by the call to dispose.
            ThreadPool.QueueUserWorkItem(delegate
            {
                Thread.Sleep(2);
                watcher.Dispose();
            });

            // This call blocks until the dispose runs.
            Assert.IsNull(watcher.GetNextJobToProcess());
        }

        protected JobDetails CreatePendingJob(string jobName, DateTime creationTime)
        {
            jobStore.CreateJob(new JobSpec(jobName, "", "key", PeriodicTrigger.CreateOneShotTrigger(creationTime)),
                dummyJobData, creationTime, CreateJobConflictAction.Throw);
            return jobStore.GetJobDetails(jobName);
        }

        protected JobDetails CreateScheduledJob(string jobName, DateTime triggerFireTime)
        {
            JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
            job.JobState = JobState.Scheduled;
            job.NextTriggerFireTime = triggerFireTime;
            jobStore.SaveJobDetails(job);
            return job;
        }

        protected JobDetails CreateTriggeredJob(string jobName, DateTime triggerFireTime)
        {
            JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
            job.JobState = JobState.Triggered;
            job.NextTriggerFireTime = triggerFireTime;
            jobStore.SaveJobDetails(job);
            return job;
        }

        protected JobDetails CreateOrphanedJob(string jobName, DateTime executionEndTime)
        {
            JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
            job.JobState = JobState.Orphaned;
            job.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, new DateTime(1970, 1, 1));
            job.LastJobExecutionDetails.EndTime = executionEndTime;
            jobStore.SaveJobDetails(job);
            return job;
        }

        protected JobDetails CreateCompletedJob(string jobName, DateTime executionEndTime)
        {
            JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
            job.JobState = JobState.Completed;
            job.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, new DateTime(1970, 1, 1));
            job.LastJobExecutionDetails.EndTime = executionEndTime;
            jobStore.SaveJobDetails(job);
            return job;
        }

        protected JobDetails CreateRunningJob(string jobName)
        {
            JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
            job.JobState = JobState.Running;
            job.LastJobExecutionDetails = new JobExecutionDetails(SchedulerGuid, new DateTime(1970, 1, 1));
            jobStore.SaveJobDetails(job);
            return job;
        }

        protected JobDetails CreateStoppedJob(string jobName)
        {
            JobDetails job = CreatePendingJob(jobName, new DateTime(1970, 1, 1));
            job.JobState = JobState.Stopped;
            jobStore.SaveJobDetails(job);
            return job;
        }
    }
}
