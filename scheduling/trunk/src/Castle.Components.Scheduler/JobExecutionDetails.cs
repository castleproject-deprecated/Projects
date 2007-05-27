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
    /// Provides details about the execution of a job including its current status.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An implementation of <see cref="IScheduler" /> may provide specialized
    /// subclasses of <see cref="JobExecutionDetails" /> to clients that can use additional
    /// scheduler-specific job execution details.  However, clients should
    /// be prepared to handle the <see cref="JobExecutionDetails" /> base class as a least common
    /// denominator albeit some of the scheduler's advanced capabilities may thus be unavailable.
    /// </para>
    /// <para>
    /// The <see cref="JobExecutionDetails" /> object returned to client code should
    /// always be a clone of the master copy, if applicable.
    /// </para>
    /// </remarks>
    [Serializable]
    public class JobExecutionDetails : ICloneable<JobExecutionDetails>
    {
        private string schedulerName;
        private DateTime startTime;
        private DateTime? endTime;
        private bool succeeded;
        private string statusMessage;

        /// <summary>
        /// Creates job execution details.
        /// </summary>
        /// <param name="schedulerName">The name of the scheduler that is running the job</param>
        /// <param name="startTime">The start time</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="schedulerName"/> is null</exception>
        public JobExecutionDetails(string schedulerName, DateTime startTime)
        {
            if (schedulerName == null)
                throw new ArgumentNullException("schedulerName");

            this.schedulerName = schedulerName;
            this.startTime = startTime;

            statusMessage = "Unknown";
        }

        /// <summary>
        /// Gets the name of the scheduler that is running the job, never null.
        /// </summary>
        public string SchedulerName
        {
            get { return schedulerName; }
        }

        /// <summary>
        /// Gets the time when the job started.
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
        }

        /// <summary>
        /// Gets or sets the time when the job ended or null if it is still running.
        /// </summary>
        /// <remarks>
        /// The default value is null.
        /// </remarks>
        public DateTime? EndTime
        {
            get { return endTime; }
            set { endTime = value; }
        }

        /// <summary>
        /// Gets or sets whether the job succeeded.
        /// </summary>
        /// <remarks>
        /// The default value is false.
        /// </remarks>
        public bool Succeeded
        {
            get { return succeeded; }
            set { succeeded = value; }
        }

        /// <summary>
        /// Gets or sets the job's status message, never null.
        /// When the job fails, this property will contain error information.
        /// </summary>
        /// <remarks>
        /// The default value is "Unknown".
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string StatusMessage
        {
            get { return statusMessage; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                statusMessage = value;
            }
        }

        /// <summary>
        /// Clones the job execution details including a deep copy of all properties.
        /// </summary>
        /// <returns>The cloned job execution details</returns>
        public virtual JobExecutionDetails Clone()
        {
            JobExecutionDetails clone = new JobExecutionDetails(schedulerName, startTime);

            clone.endTime = endTime;
            clone.succeeded = succeeded;
            clone.statusMessage = statusMessage;

            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
