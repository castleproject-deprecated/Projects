using System;
using System.Globalization;
using System.Threading;
using Castle.Core;
using Castle.Core.Logging;

namespace Castle.Components.Scheduler.JobStores
{
    /// <summary>
    /// Abstract base implementation of a <see cref="IJobStore" />.
    /// Provides a common framework for implementing simple job stores.
    /// </summary>
    [Singleton]
    public abstract class BaseJobStore : IJobStore
    {
        private bool isDisposed;
        private ILogger logger;

        protected BaseJobStore()
        {
            logger = NullLogger.Instance;
        }

        /// <summary>
        /// Gets or sets whether the job store has been disposed.
        /// </summary>
        public bool IsDisposed
        {
            get { return isDisposed; }
            protected set { isDisposed = value; }
        }

        /// <summary>
        /// Gets or sets the logger used by the scheduler.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public ILogger Logger
        {
            get { return logger; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                logger = value;
            }
        }

        public abstract void Dispose();

        public abstract void RegisterScheduler(Guid schedulerGuid, string schedulerName);

        public abstract void UnregisterScheduler(Guid schedulerGuid);

        public abstract IJobWatcher CreateJobWatcher(Guid schedulerGuid);

        public abstract JobDetails GetJobDetails(string jobName);

        public abstract void SaveJobDetails(JobDetails jobDetails);

        public abstract bool CreateJob(JobSpec jobSpec, JobData jobData, DateTime creationTime, CreateJobConflictAction conflictAction);

        public abstract bool DeleteJob(string jobName);

        /// <summary>
        /// Signals all threads blocked on <see cref="GetNextJobToProcessOrWaitUntilSignaled" />.
        /// </summary>
        protected abstract void SignalBlockedThreads();

        /// <summary>
        /// Gets the next job to process.
        /// If none are available, waits until signaled by <see cref="SignalBlockedThreads" />.
        /// </summary>
        /// <param name="schedulerGuid">The GUID of the scheduler that is polling</param>
        /// <returns>The next job to process or null if there were none</returns>
        protected abstract JobDetails GetNextJobToProcessOrWaitUntilSignaled(Guid schedulerGuid);

        /// <summary>
        /// Throws <see cref="ObjectDisposedException" /> if the job store has been disposed.
        /// </summary>
        protected void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(GetType().Name);
        }

        /// <summary>
        /// A job watcher based on <see cref="BaseJobStore.GetNextJobToProcessOrWaitUntilSignaled" />
        /// and <see cref="BaseJobStore.SignalBlockedThreads" />.
        /// </summary>
        protected class JobWatcher : IJobWatcher
        {
            private volatile BaseJobStore jobStore;
            private Guid schedulerGuid;

            public JobWatcher(BaseJobStore jobStore, Guid schedulerGuid)
            {
                this.jobStore = jobStore;
                this.schedulerGuid = schedulerGuid;
            }

            public void Dispose()
            {
                BaseJobStore cachedJobStore = jobStore;
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
                    BaseJobStore cachedJobStore = jobStore;
                    if (cachedJobStore == null)
                        return null;

                    JobDetails jobDetails = jobStore.GetNextJobToProcessOrWaitUntilSignaled(schedulerGuid);
                    if (jobDetails != null)
                        return jobDetails;
                }
            }
        }
    }
}