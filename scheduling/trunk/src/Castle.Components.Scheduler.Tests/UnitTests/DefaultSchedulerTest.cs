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
using System.Diagnostics;
using System.Text;
using System.Threading;
using Castle.Components.Scheduler.JobStores;
using Castle.Components.Scheduler.Tests.Utilities;
using Castle.Core.Logging;
using MbUnit.Framework;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Castle.Components.Scheduler.Tests.UnitTests
{
    [TestFixture(TimeOut=1)]
    [TestsOn(typeof(DefaultScheduler))]
    [Author("Jeff Brown", "jeff@ingenio.com")]
    public class DefaultSchedulerTest : BaseUnitTest
    {
        private delegate void SaveJobDetailsDelegate(JobDetails jobDetails);
        private delegate bool DeleteJobDelegate(string jobName);
        private delegate JobDetails GetNextJobToProcessDelegate();
        private delegate void DisposeDelegate();
        private delegate bool ExecuteDelegate(JobExecutionContext context);
        private delegate IAsyncResult BeingExecuteDelegate(JobExecutionContext context,
            AsyncCallback asyncCallback, object asyncState);
        private delegate bool EndExecuteDelegate(IAsyncResult asyncResult);

        private DefaultScheduler scheduler;
        private IJobStore mockJobStore;
        private IJobRunner mockJobRunner;
        private ILogger mockLogger;
        private Trigger mockTrigger;

        private JobSpec dummyJobSpec;
        private JobDetails dummyJobDetails;
        private JobData dummyJobData;

        private bool isWoken;

        public override void SetUp()
        {
            base.SetUp();

            mockJobStore = Mocks.CreateMock<IJobStore>();
            mockJobRunner = Mocks.CreateMock<IJobRunner>();
            mockLogger = Mocks.CreateMock<ILogger>();
            mockTrigger = Mocks.PartialMock<Trigger>();
            scheduler = new DefaultScheduler(mockJobStore, mockJobRunner);

            dummyJobData = new JobData();
            dummyJobSpec = new JobSpec("foo", "bar", "key", mockTrigger);
            dummyJobDetails = new JobDetails(dummyJobSpec, DateTime.Now);

            isWoken = false;
        }

        public override void TearDown()
        {
            Wake();

            if (!scheduler.IsDisposed)
                scheduler.Dispose();

            base.TearDown();
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsWhenJobStoreIsNull()
        {
            new DefaultScheduler(null, mockJobRunner);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Constructor_ThrowsWhenJobRunnerIsNull()
        {
            new DefaultScheduler(mockJobStore, null);
        }

        [Test]
        public void Name_GetterAndSetter()
        {
            Assert.IsNotNull(scheduler.Name); // has a default name

            scheduler.Name = "Test";
            Assert.AreEqual("Test", scheduler.Name);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Name_ThrowsIfValueIsNull()
        {
            scheduler.Name = null;
        }

        [Test]
        public void Logger_GetterAndSetter()
        {
            Assert.AreSame(NullLogger.Instance, scheduler.Logger);

            scheduler.Logger = mockLogger;
            Assert.AreSame(mockLogger, scheduler.Logger);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Logger_ThrowsIfValueIsNull()
        {
            scheduler.Logger = null;
        }

        [Test]
        public void ErrorRecoveryDelayInSeconds_GetterAndSetter()
        {
            Assert.AreEqual(DefaultScheduler.DefaultErrorRecoveryDelayInSeconds, scheduler.ErrorRecoveryDelayInSeconds);

            scheduler.ErrorRecoveryDelayInSeconds = 25;
            Assert.AreEqual(25, scheduler.ErrorRecoveryDelayInSeconds);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void ErrorRecoveryDelayInSeconds_ThrowsIfValueIsNegative()
        {
            scheduler.ErrorRecoveryDelayInSeconds = -1;
        }

        [Test]
        public void DisposeDoesNotThrowWhenCalledMultipleTimes()
        {
            Assert.IsFalse(scheduler.IsDisposed);

            scheduler.Dispose();
            Assert.IsTrue(scheduler.IsDisposed);

            scheduler.Dispose();
            Assert.IsTrue(scheduler.IsDisposed);
        }

        [Test]
        public void GetJobDetails_DelegatesToJobStore()
        {
            Expect.Call(mockJobStore.GetJobDetails("testJob")).Return(dummyJobDetails);

            Mocks.ReplayAll();

            Assert.AreSame(dummyJobDetails, scheduler.GetJobDetails("testJob"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void GetJobDetails_ThrowsIfNameIsNull()
        {
            scheduler.GetJobDetails(null);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void GetJobDetails_ThrowsIfDisposed()
        {
            scheduler.Dispose();
            scheduler.GetJobDetails("test");
        }

        [Test]
        public void DeleteJob_DelegatesToJobStore()
        {
            Expect.Call(mockJobStore.DeleteJob("testJob")).Return(true);

            Mocks.ReplayAll();

            Assert.IsTrue(scheduler.DeleteJob("testJob"));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void DeleteJob_ThrowsIfNameIsNull()
        {
            scheduler.DeleteJob(null);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void DeleteJob_ThrowsIfDisposed()
        {
            scheduler.Dispose();
            scheduler.DeleteJob("test");
        }

        [Test]
        public void CreateJob_DelegatesToJobStore()
        {
            mockJobStore.CreateJob(dummyJobSpec, dummyJobData, DateTime.Now, CreateJobConflictAction.Update);
            LastCall.Constraints(Is.Same(dummyJobSpec), Is.Same(dummyJobData), Is.Anything(), Is.Equal(CreateJobConflictAction.Update)).Return(true);

            Mocks.ReplayAll();

            Assert.IsTrue(scheduler.CreateJob(dummyJobSpec, dummyJobData, CreateJobConflictAction.Update));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void CreateJob_ThrowsIfSpecIsNull()
        {
            scheduler.CreateJob(null, dummyJobData, CreateJobConflictAction.Update);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void CreateJob_ThrowsIfDisposed()
        {
            scheduler.Dispose();
            scheduler.CreateJob(dummyJobSpec, dummyJobData, CreateJobConflictAction.Update);
        }

        [Test]
        public void StartTwiceDoesNotCrash()
        {
            PrepareMockJobWatcher(null);
            Mocks.ReplayAll();

            scheduler.Start();
            Assert.IsTrue(scheduler.IsRunning);

            scheduler.Start();
            Assert.IsTrue(scheduler.IsRunning);
        }

        [Test]
        public void StopWithoutStartDoesNotCrash()
        {
            scheduler.Stop();
            Assert.IsFalse(scheduler.IsRunning);
        }

        [Test]
        public void StoppingCompletesSynchronously()
        {
            PrepareMockJobWatcher(null);
            Mocks.ReplayAll();

            scheduler.Start();
            Assert.IsTrue(scheduler.IsRunning);

            scheduler.Stop();
            Assert.IsFalse(scheduler.IsRunning); // synchronously stopped
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Start_ThrowsIfDisposed()
        {
            scheduler.Dispose();
            scheduler.Start();
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Stop_ThrowsIfDisposed()
        {
            scheduler.Dispose();
            scheduler.Stop();
        }

        [Test]
        public void SchedulePendingJob_WithSkipAction()
        {
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = JobState.Pending;

            PrepareMockJobWatcher(jobDetails);

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.FirstTime, DateTime.Now))
                .Constraints(Is.Equal(TriggerScheduleCondition.FirstTime), Is.Anything())
                .Return(TriggerScheduleAction.Skip);
            Expect.Call(mockTrigger.NextFireTime).Return(new DateTime(1970, 1, 5));
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 1, 0));

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Do((SaveJobDetailsDelegate)WakeOnSaveJobDetails);

            Mocks.ReplayAll();

            RunSchedulerUntilWake();

            Assert.AreEqual(JobState.Scheduled, jobDetails.JobState);
            Assert.AreEqual(new DateTime(1970, 1, 5), jobDetails.NextTriggerFireTime);
            Assert.AreEqual(new TimeSpan(0, 1, 0), jobDetails.NextTriggerMisfireThreshold);
            Assert.IsNull(jobDetails.JobData);
            Assert.IsNull(jobDetails.LastJobExecutionDetails);
        }

        [RowTest]
        public void ScheduleTriggeredJob_WithExecuteAction()
        {
        }

        [RowTest]
        [Row(false, false, false)]
        [Row(true, false, false)]
        [Row(true, true, true)]
        public void ScheduleOrphanJob_WithStopAction(bool lastExecutionDetailsNotNull,
            bool lastExecutionSucceeded, bool lastEndTimeNotNull)
        {
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = JobState.Orphaned;

            if (lastExecutionDetailsNotNull)
            {
                jobDetails.LastJobExecutionDetails = new JobExecutionDetails(scheduler.Name, DateTime.Now);
                jobDetails.LastJobExecutionDetails.Succeeded = lastExecutionSucceeded;

                if (lastEndTimeNotNull)
                    jobDetails.LastJobExecutionDetails.EndTime = DateTime.Now;
            }

            PrepareMockJobWatcher(jobDetails);

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.JobFailed, DateTime.Now))
                .Constraints(Is.Equal(TriggerScheduleCondition.JobFailed), Is.Anything())
                .Return(TriggerScheduleAction.DeleteJob);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

            mockJobStore.DeleteJob(jobDetails.JobSpec.Name);
            LastCall.Do((DeleteJobDelegate)WakeOnDeleteJob);

            Mocks.ReplayAll();

            RunSchedulerUntilWake();

            Assert.AreEqual(JobState.Orphaned, jobDetails.JobState);
            Assert.IsNull(jobDetails.NextTriggerFireTime);
            Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
            Assert.IsNull(jobDetails.JobData);
            Assert.IsNotNull(jobDetails.LastJobExecutionDetails);
            Assert.AreEqual(scheduler.Name, jobDetails.LastJobExecutionDetails.SchedulerName);
            Assert.AreEqual(false, jobDetails.LastJobExecutionDetails.Succeeded);
            Assert.IsNotNull(jobDetails.LastJobExecutionDetails.EndTime);
        }
        
        [RowTest]
        [Row(false, false, false, TriggerScheduleCondition.JobFailed)]
        [Row(true, false, false, TriggerScheduleCondition.JobFailed)]
        [Row(true, true, true, TriggerScheduleCondition.JobSucceeded)]
        public void ScheduleCompletedJob_WithStopAction(bool lastExecutionDetailsNotNull,
            bool lastExecutionSucceeded, bool lastEndTimeNotNull, TriggerScheduleCondition expectedCondition)
        {
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = JobState.Completed;

            if (lastExecutionDetailsNotNull)
            {
                jobDetails.LastJobExecutionDetails = new JobExecutionDetails(scheduler.Name, DateTime.Now);
                jobDetails.LastJobExecutionDetails.Succeeded = lastExecutionSucceeded;

                if (lastEndTimeNotNull)
                    jobDetails.LastJobExecutionDetails.EndTime = DateTime.Now;
            }

            PrepareMockJobWatcher(jobDetails);

            Expect.Call(mockTrigger.Schedule(expectedCondition, DateTime.Now))
                .Constraints(Is.Equal(expectedCondition), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Do((SaveJobDetailsDelegate)WakeOnSaveJobDetails);

            Mocks.ReplayAll();

            RunSchedulerUntilWake();

            Assert.AreEqual(JobState.Stopped, jobDetails.JobState);
            Assert.IsNull(jobDetails.NextTriggerFireTime);
            Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
            Assert.IsNull(jobDetails.JobData);
            Assert.IsNotNull(jobDetails.LastJobExecutionDetails);
            Assert.AreEqual(scheduler.Name, jobDetails.LastJobExecutionDetails.SchedulerName);
            Assert.AreEqual(lastExecutionSucceeded, jobDetails.LastJobExecutionDetails.Succeeded);
            Assert.IsNotNull(jobDetails.LastJobExecutionDetails.EndTime);
        }

        [RowTest]
        [Row(false, true, true, Description="Fire with trigger fire time & misfire threshold.")]
        [Row(false, true, false, Description="Fire with trigger fire time but not misfire threshold.")]
        [Row(true, true, true, Description="Misfire with trigger fire time & misfire threshold.")]
        [Row(true, false, true, Description="Misfire because of missing trigger fire time but have misfire threshold.")]
        [Row(true, false, false, Description="Misfire because of missing trigger fire time but also missing misfire threshold.")]
        [Row(true, true, true, Description="Misfire assumed because last execution details missing.")]
        public void SchedulerTriggeredJob_WithSkipAction(bool misfire,
            bool nextTriggerFireTimeNotNull, bool nextTriggerMisfireThresholdNotNull)
        {
            // Create a job scheduled to fire 3 minutes in the past.
            // We cause a misfire by setting a threshold for 2 seconds which clearly is
            // in the past.  Otherwise we set the threshold to 1 minute which clearly is satisfiable. 
            DateTime schedTime = DateTime.UtcNow.AddSeconds(-3);
            JobDetails jobDetails = new JobDetails(dummyJobSpec, schedTime.AddSeconds(-5));
            jobDetails.JobState = JobState.Triggered;

            if (nextTriggerFireTimeNotNull)
                jobDetails.NextTriggerFireTime = schedTime;
            if (nextTriggerMisfireThresholdNotNull)
                jobDetails.NextTriggerMisfireThreshold = misfire ? new TimeSpan(0, 0, 2) : new TimeSpan(0, 1, 0);

            PrepareMockJobWatcher(jobDetails);

            TriggerScheduleCondition expectedCondition = misfire ? TriggerScheduleCondition.Misfire : TriggerScheduleCondition.Fire;

            Expect.Call(mockTrigger.Schedule(expectedCondition, DateTime.MinValue))
                .Constraints(Is.Equal(expectedCondition), Is.Anything())
                .Return(TriggerScheduleAction.Skip);
            Expect.Call(mockTrigger.NextFireTime).Return(new DateTime(1970, 1, 5));
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 2, 0));

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Do((SaveJobDetailsDelegate)WakeOnSaveJobDetails);

            Mocks.ReplayAll();

            RunSchedulerUntilWake();

            Assert.AreEqual(JobState.Scheduled, jobDetails.JobState);
            Assert.AreEqual(new DateTime(1970, 1, 5), jobDetails.NextTriggerFireTime);
            Assert.AreEqual(new TimeSpan(0, 2, 0), jobDetails.NextTriggerMisfireThreshold);
            Assert.IsNull(jobDetails.JobData);
            Assert.IsNull(jobDetails.LastJobExecutionDetails);
        }

        [RowTest]
        [Row(true, false, false, Description="Successful job execution.")]
        [Row(false, false, false, Description="Failed job execution with no exception.")]
        [Row(false, true, false, Description="Failed job executed without an exception.")]
        public void SchedulerExecutesJobsAndHandlesSuccessFailureOrException(
            bool jobSucceeds, bool jobThrows, bool savingCompletedJobThrows)
        {
            JobData newJobData = new JobData();

            ExecuteDelegate execute = delegate(JobExecutionContext context)
            {
                Assert.IsNotNull(context);
                Assert.AreSame(scheduler, context.Scheduler);
                Assert.IsNotNull(context.Logger);
                Assert.IsNotNull(context.JobSpec);

                context.JobData = newJobData;

                if (jobThrows)
                    throw new Exception("Oh no!");

                return jobSucceeds;
            };

            PrepareJobForExecution(execute.BeginInvoke, execute.EndInvoke);

            TriggerScheduleCondition expectedCondition = jobSucceeds ? TriggerScheduleCondition.JobSucceeded : TriggerScheduleCondition.JobFailed;
            Expect.Call(mockTrigger.Schedule(expectedCondition, DateTime.Now))
                .Constraints(Is.Equal(expectedCondition), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

            mockJobStore.SaveJobDetails(null);
            LastCall.IgnoreArguments().Do((SaveJobDetailsDelegate) delegate(JobDetails completedJobDetails)
            {
                Assert.IsNotNull(completedJobDetails);
                Assert.AreEqual(dummyJobSpec.Name, completedJobDetails.JobSpec.Name);

                Assert.IsNotNull(completedJobDetails.LastJobExecutionDetails);
                Assert.AreEqual(scheduler.Name, completedJobDetails.LastJobExecutionDetails.SchedulerName);
                Assert.GreaterEqualThan(completedJobDetails.LastJobExecutionDetails.StartTime, completedJobDetails.CreationTime);
                Assert.IsNotNull(completedJobDetails.LastJobExecutionDetails.EndTime);
                Assert.GreaterEqualThan(completedJobDetails.LastJobExecutionDetails.EndTime, completedJobDetails.LastJobExecutionDetails.StartTime);
                Assert.AreEqual(jobSucceeds, completedJobDetails.LastJobExecutionDetails.Succeeded);

                if (!jobThrows)
                    JobAssert.AreEqual(newJobData, completedJobDetails.JobData);
                else
                    Assert.IsNull(completedJobDetails.JobData);

                Wake();
            });

            Mocks.ReplayAll();

            RunSchedulerUntilWake();
        }

        [Test]
        public void SchedulerExecutesJobsAndHandlesBeginJobFailure()
        {
            PrepareJobForExecution(delegate
            {
                throw new Exception("Eeep!");
            }, delegate
            {
                return true;
            });

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.JobFailed, DateTime.Now))
                .Constraints(Is.Equal(TriggerScheduleCondition.JobFailed), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

            mockJobStore.SaveJobDetails(null);
            LastCall.IgnoreArguments().Do((SaveJobDetailsDelegate)delegate(JobDetails completedJobDetails)
            {
                Assert.IsNotNull(completedJobDetails);
                Assert.AreEqual(dummyJobSpec.Name, completedJobDetails.JobSpec.Name);

                Assert.IsNotNull(completedJobDetails.LastJobExecutionDetails);
                Assert.AreEqual(scheduler.Name, completedJobDetails.LastJobExecutionDetails.SchedulerName);
                Assert.GreaterEqualThan(completedJobDetails.LastJobExecutionDetails.StartTime, completedJobDetails.CreationTime);
                Assert.IsNotNull(completedJobDetails.LastJobExecutionDetails.EndTime);
                Assert.GreaterEqualThan(completedJobDetails.LastJobExecutionDetails.EndTime, completedJobDetails.LastJobExecutionDetails.StartTime);
                Assert.AreEqual(false, completedJobDetails.LastJobExecutionDetails.Succeeded);
                Assert.IsNull(completedJobDetails.JobData);

                Wake();
            });

            Mocks.ReplayAll();

            RunSchedulerUntilWake();
        }

        /// <summary>
        /// This case is a bit hard to verify automatically because everything is asynchronous.
        /// TODO: Look for the message that was logged by the exception.
        /// </summary>
        [Test]
        public void SchedulerExecutesJobsAndToleratesExceptionDuringFinalSave()
        {
            ExecuteDelegate execute = delegate
            {
                return true;
            };

            PrepareJobForExecution(execute.BeginInvoke, execute.EndInvoke);

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.JobSucceeded, DateTime.Now))
                .Constraints(Is.Equal(TriggerScheduleCondition.JobSucceeded), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

            mockJobStore.SaveJobDetails(null);
            LastCall.IgnoreArguments().Do((SaveJobDetailsDelegate)delegate
            {
                Wake();
                throw new Exception("Oops!");
            });

            Mocks.ReplayAll();

            RunSchedulerUntilWake();
        }

        [RowTest]
        [Row(JobState.Scheduled)]
        [Row(JobState.Running)]
        [Row(JobState.Stopped)]
        [Row((JobState)9999)] // invalid job state
        public void SchedulerHandlesUnexpectedJobStateReceivedFromWatcherByStoppingTheTrigger(JobState jobState)
        {
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = jobState;

            PrepareMockJobWatcher(jobDetails);

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Do((SaveJobDetailsDelegate)WakeOnSaveJobDetails);

            Mocks.ReplayAll();

            RunSchedulerUntilWake();

            Assert.AreEqual(JobState.Stopped, jobDetails.JobState);
            Assert.IsNull(jobDetails.NextTriggerFireTime);
            Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
            Assert.IsNull(jobDetails.LastJobExecutionDetails);
            Assert.IsNull(jobDetails.JobData);
        }

        [Test]
        public void SchedulerHandlesJobWatcherExceptionByInsertingAnErrorRecoveryDelay()
        {
            // The mock job watcher will throw on the first GetNextJobToProcess.
            // On the second one it will cause us to wake up.
            // There should be a total delay at least as big as the error recovery delay.
            IJobWatcher mockJobWatcher = Mocks.CreateMock<IJobWatcher>();

            Expect.Call(mockJobWatcher.GetNextJobToProcess()).Throw(new Exception("Uh oh!"));
            Expect.Call(mockJobWatcher.GetNextJobToProcess()).Do((GetNextJobToProcessDelegate)delegate
            {
                Wake();
                return null;
            });

            mockJobWatcher.Dispose();
            LastCall.Repeat.AtLeastOnce();

            Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Name)).Return(mockJobWatcher);

            Mocks.ReplayAll();

            scheduler.ErrorRecoveryDelayInSeconds = 2;

            Stopwatch stopWatch = Stopwatch.StartNew();
            RunSchedulerUntilWake();
            Assert.Between(stopWatch.ElapsedMilliseconds, 2000, 10000);
        }

        [RowTest]
        [Row(false)]
        [Row(true)]
        public void SchedulerHandlesJobWatcherObjectDisposedExceptionByStopping(bool throwSecondExceptionInDispose)
        {
            // The mock job watcher will throw ObjectDisposedException on the first
            // call to GetNextJobToProcess.  This should cause the scheduler's job watching
            // thread to begin shutting down and eventually call the job watcher's Dispose.
            // That will wake us up from sleep so we can verify that the scheduler has stopped
            // on its own.
            // We also check what happens if a second exception occurs in Dispose during shutdown.
            IJobWatcher mockJobWatcher = Mocks.CreateMock<IJobWatcher>();

            Expect.Call(mockJobWatcher.GetNextJobToProcess()).Throw(new ObjectDisposedException("Uh oh!"));

            mockJobWatcher.Dispose();
            LastCall.Do((DisposeDelegate)delegate
            {
                Wake();

                if (throwSecondExceptionInDispose)
                    throw new Exception("Yikes!  We're trying to shut down here!");
            });

            Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Name)).Return(mockJobWatcher);

            Mocks.ReplayAll();

            scheduler.Start();
            WaitUntilWake();

            Assert.IsFalse(scheduler.IsRunning, "The scheduler should have stopped itself automatically.");
        }

        [Test]
        public void SchedulerHandlesJobStoreExceptionByInsertingAnErrorRecoveryDelay()
        {
            // The mock job watcher will return job detail on the first GetNextJobToProcess
            // but the mock job store will throw Expection during SaveJobDetails.
            // On the second call to GetNextJobToProcess the mock job watcher will cause us to wake up.
            // There should be a total delay at least as big as the error recovery delay.
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = JobState.Pending;

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.FirstTime, DateTime.Now))
                .Constraints(Is.Equal(TriggerScheduleCondition.FirstTime), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(new DateTime(1970, 1, 5));
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 1, 0));

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Throw(new Exception("Uh oh!"));

            IJobWatcher mockJobWatcher = Mocks.CreateMock<IJobWatcher>();

            Expect.Call(mockJobWatcher.GetNextJobToProcess()).Return(jobDetails);
            Expect.Call(mockJobWatcher.GetNextJobToProcess()).Do((GetNextJobToProcessDelegate)delegate
            {
                Wake();
                return null;
            });

            mockJobWatcher.Dispose();
            LastCall.Repeat.AtLeastOnce();

            Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Name)).Return(mockJobWatcher);

            Mocks.ReplayAll();

            scheduler.ErrorRecoveryDelayInSeconds = 2;

            Stopwatch stopWatch = Stopwatch.StartNew();
            RunSchedulerUntilWake();
            Assert.Between(stopWatch.ElapsedMilliseconds, 2000, 10000);
        }

        [Test]
        public void SchedulerHandlesJobStoreConcurrentModificationExceptionByIgnoringTheJob()
        {
            // The mock job watcher will return job detail on the first GetNextJobToProcess
            // but the mock job store will throw ConcurrentModificationException during SaveJobDetails.
            // On the second call to GetNextJobToProcess the mock job watcher will cause us to wake up.
            // There should be no noticeable delay, particularly not one as big as the error recovery delay.
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = JobState.Pending;

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.FirstTime, DateTime.Now))
                .Constraints(Is.Equal(TriggerScheduleCondition.FirstTime), Is.Anything())
                .Return(TriggerScheduleAction.Stop);
            Expect.Call(mockTrigger.NextFireTime).Return(new DateTime(1970, 1, 5));
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 1, 0));

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Throw(new ConcurrentModificationException("Another scheduler grabbed it."));

            IJobWatcher mockJobWatcher = Mocks.CreateMock<IJobWatcher>();

            Expect.Call(mockJobWatcher.GetNextJobToProcess()).Return(jobDetails);
            Expect.Call(mockJobWatcher.GetNextJobToProcess()).Do((GetNextJobToProcessDelegate)delegate
            {
                Wake();
                return null;
            });

            mockJobWatcher.Dispose();
            LastCall.Repeat.AtLeastOnce();

            Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Name)).Return(mockJobWatcher);

            Mocks.ReplayAll();

            scheduler.ErrorRecoveryDelayInSeconds = 2;

            Stopwatch stopWatch = Stopwatch.StartNew();
            RunSchedulerUntilWake();
            Assert.LowerThan(stopWatch.ElapsedMilliseconds, 2000);
        }

        [RowTest]
        [Row((TriggerScheduleAction) 9999)] // invalid trigger action
        public void ScheduleHandlesUnexpectedActionReceivedFromTriggerByStoppingTheTrigger(TriggerScheduleAction action)
        {
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = JobState.Pending;

            PrepareMockJobWatcher(jobDetails);

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.FirstTime, DateTime.Now))
                .Constraints(Is.Equal(TriggerScheduleCondition.FirstTime), Is.Anything())
                .Return(action);
            Expect.Call(mockTrigger.NextFireTime).Return(new DateTime(1970, 1, 5));
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(new TimeSpan(0, 1, 0));

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Do((SaveJobDetailsDelegate)WakeOnSaveJobDetails);

            Mocks.ReplayAll();

            RunSchedulerUntilWake();

            Assert.AreEqual(JobState.Stopped, jobDetails.JobState);
            Assert.IsNull(jobDetails.NextTriggerFireTime);
            Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
            Assert.IsNull(jobDetails.LastJobExecutionDetails);
            Assert.IsNull(jobDetails.JobData);
        }

        [Test]
        public void ScheduleHandlesTriggerExceptionByStoppingTheTrigger()
        {
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = JobState.Pending;

            PrepareMockJobWatcher(jobDetails);

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.FirstTime, DateTime.Now))
                .Constraints(Is.Equal(TriggerScheduleCondition.FirstTime), Is.Anything())
                .Throw(new Exception("Oh no!")); // throw an exception from the trigger

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Do((SaveJobDetailsDelegate)WakeOnSaveJobDetails);

            Mocks.ReplayAll();

            RunSchedulerUntilWake();

            Assert.AreEqual(JobState.Stopped, jobDetails.JobState);
            Assert.IsNull(jobDetails.NextTriggerFireTime);
            Assert.IsNull(jobDetails.NextTriggerMisfireThreshold);
            Assert.IsNull(jobDetails.LastJobExecutionDetails);
            Assert.IsNull(jobDetails.JobData);
        }

        /// <summary>
        /// Schedules a job that is guaranteed to be executed.
        /// </summary>
        private void PrepareJobForExecution(BeingExecuteDelegate beginExecute,
            EndExecuteDelegate endExecute)
        {
            JobDetails jobDetails = new JobDetails(dummyJobSpec, DateTime.Now);
            jobDetails.JobState = JobState.Pending;

            PrepareMockJobWatcher(jobDetails);

            Expect.Call(mockTrigger.Schedule(TriggerScheduleCondition.FirstTime, DateTime.MinValue))
                .Constraints(Is.Equal(TriggerScheduleCondition.FirstTime), Is.Anything())
                .Return(TriggerScheduleAction.ExecuteJob);
            Expect.Call(mockTrigger.NextFireTime).Return(null);
            Expect.Call(mockTrigger.NextMisfireThreshold).Return(null);

            mockJobStore.SaveJobDetails(jobDetails);
            LastCall.Do((SaveJobDetailsDelegate)delegate
            {
                Assert.AreEqual(JobState.Running, jobDetails.JobState);
                Assert.IsNotNull(jobDetails.LastJobExecutionDetails);
                Assert.AreEqual(scheduler.Name, jobDetails.LastJobExecutionDetails.SchedulerName);
                Assert.GreaterEqualThan(jobDetails.LastJobExecutionDetails.StartTime, jobDetails.CreationTime);
                Assert.IsNull(jobDetails.LastJobExecutionDetails.EndTime);
                Assert.IsFalse(jobDetails.LastJobExecutionDetails.Succeeded);
            });

            Expect.Call(mockJobRunner.BeginExecute(null, null, null))
                .IgnoreArguments().Repeat.Any().Do(beginExecute);
            Expect.Call(mockJobRunner.EndExecute(null))
                .IgnoreArguments().Repeat.Any().Do(endExecute);
        }

        /// <summary>
        /// Sets the mock job store to provide a watcher that yields the specified job details
        /// on its first access then waits to be disposed.
        /// </summary>
        /// <param name="jobDetails">The job details to yield</param>
        private void PrepareMockJobWatcher(JobDetails jobDetails)
        {
            Expect.Call(mockJobStore.CreateJobWatcher(scheduler.Name)).
                Return(new MockJobWatcher(jobDetails));
        }

        /// <summary>
        /// Runs the scheduler until <see cref="Wake" /> is called then Stops it.
        /// </summary>
        private void RunSchedulerUntilWake()
        {
            lock (scheduler)
            {
                scheduler.Logger = new ConsoleLogger();
                scheduler.Start();
            }

            WaitUntilWake();
            scheduler.Stop();
        }

        /// <summary>
        /// Waits for <see cref="Wake" /> to be called.
        /// </summary>
        private void WaitUntilWake()
        {
            lock (scheduler)
            {
                if (! isWoken)
                    Assert.IsTrue(Monitor.Wait(scheduler, 10000), "Wake signal not received before timeout elapsed.");
            }
        }

        /// <summary>
        /// Wakes the threads blocked in <see cref="WaitUntilWake" />.
        /// </summary>
        private void Wake()
        {
            lock (scheduler)
            {
                isWoken = true;
                Monitor.PulseAll(scheduler);
            }
        }

        private void WakeOnSaveJobDetails(JobDetails jobDetails)
        {
            Wake();
        }

        private bool WakeOnDeleteJob(string jobName)
        {
            Wake();
            return true;
        }

        private class MockJobWatcher : IJobWatcher
        {
            private JobDetails jobDetails;
            private bool firstTime;
            private bool isDisposed;

            public MockJobWatcher(JobDetails jobDetails)
            {
                this.jobDetails = jobDetails;

                firstTime = true;
            }

            public JobDetails GetNextJobToProcess()
            {
                lock (this)
                {
                    while (!isDisposed)
                    {
                        if (jobDetails != null && firstTime)
                        {
                            firstTime = false;
                            return jobDetails;
                        }

                        Monitor.Wait(this);
                    }
                }

                return null;
            }

            public void Dispose()
            {
                lock (this)
                {
                    isDisposed = true;
                    Monitor.PulseAll(this);
                }
            }
        }
    }
}
