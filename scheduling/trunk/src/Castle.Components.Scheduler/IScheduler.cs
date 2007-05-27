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

namespace Castle.Components.Scheduler
{
    /// <summary>
    /// A job scheduler schedules jobs for execution.
    /// It may optionally provide job persistence, clustering and other features.
    /// </summary>
    /// <todo>
    /// Optionally track job history.
    /// Provide events for instrumentation.
    /// Support job scheduling rules for mutually exclusive access to contended resources.
    /// Support job behaviors (persistent listeners).
    /// Support job queueing.
    /// </todo>
    public interface IScheduler : IDisposable
    {
        /// <summary>
        /// Returns true if the scheduler has been disposed.
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// Returns true if the scheduler has been started and is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets the name of the scheduler, never null.
        /// The name might not be unique.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Starts scheduling jobs for execution.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
        void Start();

        /// <summary>
        /// Stops scheduling jobs for execution.
        /// </summary>
        /// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
        void Stop();

        /// <summary>
        /// Gets the details of the job with the specified name.
        /// </summary>
        /// <param name="jobName">The name of the job</param>
        /// <returns>The job details, or null if not found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="jobName"/> is null</exception>
        /// <exception cref="SchedulerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
        JobDetails GetJobDetails(string jobName);

        /// <summary>
        /// Creates a job.
        /// </summary>
        /// <param name="jobSpec">The job specification</param>
        /// <param name="jobData">The initial job data, or null if none</param>
        /// <param name="conflictAction">The action to take if a job with the
        /// same name already exists</param>
        /// <returns>True if the job was created or updated, false if a conflict occurred
        /// and no changes were made</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="jobSpec"/> is null</exception>
        /// <exception cref="SchedulerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
        bool CreateJob(JobSpec jobSpec, JobData jobData, CreateJobConflictAction conflictAction);

        /// <summary>
        /// Deletes the job with the specified name.
        /// </summary>
        /// <param name="jobName">The name of the job to delete</param>
        /// <returns>True if a job was actually deleted, false if no such job was found</returns>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="jobName"/> is null</exception>
        /// <exception cref="SchedulerException">Thrown if an error occurs</exception>
        /// <exception cref="ObjectDisposedException">Thrown if the scheduler has been disposed</exception>
        bool DeleteJob(string jobName);
    }
}
