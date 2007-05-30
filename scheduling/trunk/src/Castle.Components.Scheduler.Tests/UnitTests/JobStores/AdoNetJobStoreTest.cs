using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Castle.Components.Scheduler.JobStores;
using MbUnit.Framework;

namespace Castle.Components.Scheduler.Tests.UnitTests.JobStores
{
    [TestsOn(typeof(AdoNetJobStore))]
    [Author("Jeff Brown", "jeff@ingenio.com")]
    public abstract class AdoNetJobStoreTest : BaseJobStoreTest
    {
        new public AdoNetJobStore JobStore
        {
            get { return (AdoNetJobStore)base.JobStore; }
        }

        protected override BaseJobStore CreateJobStore()
        {
            AdoNetJobStore jobStore = CreateAdoNetJobStore();
            Assert.AreEqual(15, jobStore.PollIntervalInSeconds);
            Assert.AreEqual(120, jobStore.SchedulerExpirationTimeInSeconds);

            jobStore.PollIntervalInSeconds = 1;
            jobStore.SchedulerExpirationTimeInSeconds = 5;
            return jobStore;
        }

        protected abstract AdoNetJobStore CreateAdoNetJobStore();

        /// <summary>
        /// Sets whether subsequent Db connection requests for the specified job store
        /// should be caused to fail.
        /// </summary>
        protected abstract void SetBrokenConnectionMocking(AdoNetJobStore jobStore, bool brokenConnections);

        [Test]
        public void ClusterName_GetterAndSetter()
        {
            Assert.AreEqual("Default", JobStore.ClusterName);

            JobStore.ClusterName = "Cluster";
            Assert.AreEqual("Cluster", JobStore.ClusterName);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ClusterName_ThrowsIfValueIsNull()
        {
            JobStore.ClusterName = null;
        }

        [Test]
        public void PollIntervalInSeconds_GetterAndSetter()
        {
            JobStore.PollIntervalInSeconds = 15;
            Assert.AreEqual(15, JobStore.PollIntervalInSeconds);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PollIntervalInSeconds_ThrowsIfValueIsZero()
        {
            JobStore.PollIntervalInSeconds = 0;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void PollIntervalInSeconds_ThrowsIfValueIsNegative()
        {
            JobStore.PollIntervalInSeconds = -1;
        }

        [Test]
        public void SchedulerExpirationTimeInSeconds_GetterAndSetter()
        {
            JobStore.SchedulerExpirationTimeInSeconds = 15;
            Assert.AreEqual(15, JobStore.SchedulerExpirationTimeInSeconds);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SchedulerExpirationTimeInSeconds_ThrowsIfValueIsZero()
        {
            JobStore.SchedulerExpirationTimeInSeconds = 0;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void SchedulerExpirationTimeInSeconds_ThrowsIfValueIsNegative()
        {
            JobStore.SchedulerExpirationTimeInSeconds = -1;
        }

        [Test]
        public void ConnectionString_LooksSane()
        {
            Assert.IsNotNull(JobStore.ConnectionString);
        }

        [Test]
        [ExpectedException(typeof(SchedulerException))]
        public void RegisterScheduler_WrapsExceptionIfDbConnectionFailureOccurs()
        {
            Mocks.ReplayAll();

            SetBrokenConnectionMocking(JobStore, true);
            JobStore.RegisterScheduler(SchedulerGuid, SchedulerName);
        }

        [Test]
        [ExpectedException(typeof(SchedulerException))]
        public void UnregisterScheduler_WrapsExceptionIfDbConnectionFailureOccurs()
        {
            Mocks.ReplayAll();

            SetBrokenConnectionMocking(JobStore, true);
            JobStore.UnregisterScheduler(SchedulerGuid);
        }

        [Test]
        [ExpectedException(typeof(SchedulerException))]
        public void CreateJob_WrapsExceptionIfDbConnectionFailureOccurs()
        {
            Mocks.ReplayAll();

            SetBrokenConnectionMocking(JobStore, true);
            JobStore.CreateJob(dummyJobSpec, null, DateTime.UtcNow, CreateJobConflictAction.Ignore);
        }

        [Test]
        [ExpectedException(typeof(SchedulerException))]
        public void DeleteJob_WrapsExceptionIfDbConnectionFailureOccurs()
        {
            Mocks.ReplayAll();

            SetBrokenConnectionMocking(JobStore, true);
            JobStore.DeleteJob("foo");
        }

        [Test]
        [ExpectedException(typeof(SchedulerException))]
        public void GetJobDetails_WrapsExceptionIfDbConnectionFailureOccurs()
        {
            Mocks.ReplayAll();

            SetBrokenConnectionMocking(JobStore, true);
            JobStore.GetJobDetails("foo");
        }

        [Test]
        [ExpectedException(typeof(SchedulerException))]
        public void SaveJobDetails_WrapsExceptionIfDbConnectionFailureOccurs()
        {
            Mocks.ReplayAll();

            JobDetails jobDetails = CreatePendingJob("job", DateTime.UtcNow);

            SetBrokenConnectionMocking(JobStore, true);
            JobStore.SaveJobDetails(jobDetails);
        }

        [Test]
        public void JobWatcher_IgnoresExceptionIfDbConnectionFailureOccurs()
        {
            Mocks.ReplayAll();

            SetBrokenConnectionMocking(JobStore, true);

            IJobWatcher jobWatcher = JobStore.CreateJobWatcher(SchedulerGuid);

            // This could throw an exception but instead we catch and log it then keep going
            // until we are disposed.
            Stopwatch stopwatch = Stopwatch.StartNew();

            ThreadPool.QueueUserWorkItem(delegate
            {
                Thread.Sleep(2000);
                jobWatcher.Dispose();
            });

            Assert.IsNull(jobWatcher.GetNextJobToProcess());
            Assert.Between(stopwatch.ElapsedMilliseconds, 2000, 4000, "Check that the thread was blocked the whole time.");
        }

        [Test]
        public void RunningJobsAreOrphanedWhenSchedulerRegistrationRefreshFails()
        {
            Mocks.ReplayAll();

            CreateRunningJob("running-job");

            SetBrokenConnectionMocking(JobStore, true);
            JobStore.SchedulerExpirationTimeInSeconds = 1;

            // Allow some time for the expiration time to expire.
            Thread.Sleep(3000);

            // Now get a new scheduler.
            // Its next job up for processing should be the one that we created earlier
            // but now it will be Orphaned.
            AdoNetJobStore newJobStore = CreateAdoNetJobStore();
            newJobStore.SchedulerExpirationTimeInSeconds = 1;
            newJobStore.PollIntervalInSeconds = 1;

            IJobWatcher jobWatcher = newJobStore.CreateJobWatcher(Guid.NewGuid());
            JobDetails orphanedJob = jobWatcher.GetNextJobToProcess();

            Assert.AreEqual("running-job", orphanedJob.JobSpec.Name);
            Assert.AreEqual(JobState.Orphaned, orphanedJob.JobState);
            Assert.AreEqual(false, orphanedJob.LastJobExecutionDetails.Succeeded);
            Assert.IsNotNull(orphanedJob.LastJobExecutionDetails.EndTime);
        }
    }
}
