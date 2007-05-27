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
    /// A <see cref="JobSpec" /> is a specification of the properties of a
    /// scheduled job.  It provides the name of the scheduled job instance
    /// and a job key used to create suitable <see cref="IJob" /> instances for
    /// execution.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An implementation of <see cref="IScheduler" /> may accept specialized
    /// subclasses of <see cref="JobSpec" /> from clients that can provide additional
    /// scheduler-specific job specification information.  However, the scheduler should
    /// be prepared to handle the <see cref="JobSpec" /> base class as a least common
    /// denominator albeit some of its advanced capabilities may thus be unavailable.
    /// </para>
    /// <para>
    /// A <see cref="JobSpec" /> is always immutable once created.
    /// </para>
    /// </remarks>
    [Serializable]
    public class JobSpec : ICloneable<JobSpec>
    {
        private string name;
        private string description;
        private string jobKey;
        private Trigger trigger;

        /// <summary>
        /// Creates a job specification.
        /// </summary>
        /// <param name="name">The unique name of the job</param>
        /// <param name="description">The description of the job</param>
        /// <param name="jobKey">The key that is used by a <see cref="IJobFactory" />
        /// to construct an <see cref="IJob" /> instance when the job is to be executed</param>
        /// <param name="trigger">The trigger that determines when the job is scheduled to run</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="name"/>, <paramref name="description"/>,
        /// <paramref name="jobKey"/> or <paramref name="trigger"/> is null</exception>
        /// <exception cref="ArgumentException">Thrown if <paramref name="name"/> is an empty string</exception>
        public JobSpec(string name, string description, string jobKey, Trigger trigger)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (name.Length == 0)
                throw new ArgumentException("name");
            if (description == null)
                throw new ArgumentNullException("description");
            if (jobKey == null)
                throw new ArgumentNullException("jobKey");
            if (trigger == null)
                throw new ArgumentNullException("trigger");

            this.name = name;
            this.description = description;
            this.jobKey = jobKey;
            this.trigger = trigger;
        }

        /// <summary>
        /// Gets the unique name of this scheduled job.
        /// </summary>
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets the description of the scheduled job.
        /// </summary>
        public string Description
        {
            get { return description; }
        }

        /// <summary>
        /// Gets the key that is used by a <see cref="IJobFactory" />
        /// to construct an <see cref="IJob" /> instance when the job is to be executed.
        /// </summary>
        public string JobKey
        {
            get { return jobKey; }
        }

        /// <summary>
        /// Gets the trigger that determines when the job is scheduled to run.
        /// </summary>
        public Trigger Trigger
        {
            get { return trigger; }
        }

        /// <summary>
        /// Clones the job specification including a deep copy of the all properties.
        /// </summary>
        /// <returns>The cloned job specification</returns>
        public virtual JobSpec Clone()
        {
            JobSpec clone = new JobSpec(name, description, jobKey, trigger.Clone());

            return clone;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
