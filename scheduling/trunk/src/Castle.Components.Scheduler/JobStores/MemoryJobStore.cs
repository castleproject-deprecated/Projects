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
using System.Globalization;
using System.Threading;
using Castle.Core;

namespace Castle.Components.Scheduler.JobStores
{
    /// <summary>
    /// The memory job store maintains all job state in-process in memory.
    /// It does not support persistence or clustering.
    /// </summary>
    [Singleton]
    public class MemoryJobStore : IJobStore
    {
        private Dictionary<string, VersionedJobDetails> jobStatusTable;
        private bool isDisposed;

        /// <summary>
        /// Creates an in-process memory job store initially without any jobs.
        /// </summary>
        public MemoryJobStore()
        {
            jobStatusTable = new Dictionary<string, VersionedJobDetails>();
        }

        public void Dispose()
        {
            lock (jobStatusTable)
            {
                isDisposed = true;
                jobStatusTable.Clear();

                Monitor.PulseAll(jobStatusTable);
            }
        }

        public IJobWatcher CreateJobWatcher(string schedulerName)
        {
            if (schedulerName == null)
                throw new ArgumentNullException("schedulerName");

            ThrowIfDisposed();

            return new JobWatcher(this);
        }

        public JobDetails GetJobDetails(string jobName)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            lock (jobStatusTable)
            {
                ThrowIfDisposed();

                VersionedJobDetails jobDetails;
                if (jobStatusTable.TryGetValue(jobName, out jobDetails))
                    return jobDetails.Clone();

                return null;
            }
        }

        public void SaveJobDetails(JobDetails jobDetails)
        {
            if (jobDetails == null)
                throw new ArgumentNullException("jobStatus");

            VersionedJobDetails versionedJobDetails = (VersionedJobDetails) jobDetails;

            lock (jobStatusTable)
            {
                ThrowIfDisposed();

                string jobName = jobDetails.JobSpec.Name;

                VersionedJobDetails existingJobDetails;
                if (! jobStatusTable.TryGetValue(jobName, out existingJobDetails))
                    throw new ConcurrentModificationException("The job details could not be saved because the job was concurrently deleted.");

                if (existingJobDetails.Version != versionedJobDetails.Version)
                    throw new ConcurrentModificationException("The job details could not be saved because the job was concurrently modified.");

                versionedJobDetails.Version += 1;
                versionedJobDetails.JobSpec.Trigger.IsDirty = false;
                jobStatusTable[jobName] = (VersionedJobDetails) versionedJobDetails.Clone();

                Monitor.PulseAll(jobStatusTable);
            }
        }

        public bool CreateJob(JobSpec jobSpec, JobData jobData, DateTime creationTime, CreateJobConflictAction conflictAction)
        {
            if (jobSpec == null)
                throw new ArgumentNullException("jobSpec");

            lock (jobStatusTable)
            {
                ThrowIfDisposed();

                if (jobStatusTable.ContainsKey(jobSpec.Name))
                {
                    if (conflictAction == CreateJobConflictAction.Ignore)
                        return false;
                    if (conflictAction != CreateJobConflictAction.Update)
                        throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
                            "There is already a job with name '{0}'.", jobSpec.Name));
                }

                VersionedJobDetails jobDetails = new VersionedJobDetails(jobSpec, creationTime, 0);
                jobDetails.JobData = jobData;

                jobStatusTable[jobSpec.Name] = jobDetails;
                Monitor.PulseAll(jobStatusTable);
                return true;
            }
        }

        public bool DeleteJob(string jobName)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            lock (jobStatusTable)
            {
                ThrowIfDisposed();

                if (!jobStatusTable.Remove(jobName))
                    return false;

                Monitor.PulseAll(jobStatusTable);
                return true;
            }
        }

        private void SignalBlockedThreads()
        {
            lock (jobStatusTable)
                Monitor.PulseAll(jobStatusTable);
        }

        private JobDetails GetNextJobToProcessOrWaitUntilSignaled()
        {
            lock (jobStatusTable)
            {
                ThrowIfDisposed();

                DateTime now = DateTime.UtcNow;
                DateTime? waitNextTriggerFireTime = null;

                foreach (VersionedJobDetails jobDetails in jobStatusTable.Values)
                {
                    switch (jobDetails.JobState)
                    {
                        case JobState.Scheduled:
                            if (jobDetails.NextTriggerFireTime.HasValue)
                            {
                                DateTime jobNextTriggerFireTime = jobDetails.NextTriggerFireTime.Value.ToUniversalTime();

                                if (jobNextTriggerFireTime > now)
                                {
                                    if (!waitNextTriggerFireTime.HasValue || jobNextTriggerFireTime < waitNextTriggerFireTime.Value)
                                        waitNextTriggerFireTime = jobNextTriggerFireTime;
                                    break;
                                }
                            }

                            jobDetails.JobState = JobState.Triggered;
                            return jobDetails.Clone();

                        case JobState.Pending:
                        case JobState.Triggered:
                        case JobState.Orphaned:
                        case JobState.Completed:
                            return jobDetails.Clone();
                    }
                }

                // Otherwise wait for a signal or the next fire time whichever comes first.
                if (waitNextTriggerFireTime.HasValue)
                {
                    // Need to ensure that wait time in millis will fit in a 32bit integer, otherwise the
                    // Monitor.Wait will throw an ArgumentException (even when using the TimeSpan based overload).
                    // This can happen when the next trigger fire time is very far out into the future like DateTime.MaxValue.
                    TimeSpan waitTimeSpan = waitNextTriggerFireTime.Value - now;
                    int waitMillis = (int) Math.Min(int.MaxValue, waitTimeSpan.TotalMilliseconds);
                    Monitor.Wait(jobStatusTable, waitMillis);
                }
                else
                {
                    Monitor.Wait(jobStatusTable);
                }
            }

            return null;
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        private class JobWatcher : IJobWatcher
        {
            private volatile MemoryJobStore jobStore;

            public JobWatcher(MemoryJobStore jobStore)
            {
                this.jobStore = jobStore;
            }

            public void Dispose()
            {
                MemoryJobStore cachedJobStore = jobStore;
                if (cachedJobStore != null)
                {
                    jobStore = null;
                    cachedJobStore.SignalBlockedThreads();
                }
            }

            public JobDetails GetNextJobToProcess()
            {
                for (;;)
                {
                    MemoryJobStore cachedJobStore = jobStore;
                    if (cachedJobStore == null)
                        return null;

                    JobDetails jobDetails = jobStore.GetNextJobToProcessOrWaitUntilSignaled();
                    if (jobDetails != null)
                        return jobDetails;
                }
            }
        }
    }
}
