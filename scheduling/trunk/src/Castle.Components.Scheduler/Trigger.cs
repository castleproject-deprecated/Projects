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
    /// A trigger encapsulates an algorithm for determining when an event should occur.
    /// A trigger must be serializable if it is to be used with a persistent store.
    /// </summary>
    /// <remarks>
    /// <para>
    /// An implementation of <see cref="IScheduler" /> may recognize and provide special
    /// support for certain subclasses of <see cref="Trigger" />.  However, the scheduler should
    /// be prepared to handle the <see cref="Trigger" /> base class as a least common
    /// denominator albeit some of its advanced capabilities may thus be unavailable.
    /// </para>
    /// </remarks>
    [Serializable]
    public abstract class Trigger : ICloneable<Trigger>
    {
        [NonSerialized]
        private bool isDirty;

        /// <summary>
        /// Creates a trigger initially in the clear state.
        /// </summary>
        protected Trigger()
        {
        }

        /// <summary>
        /// Gets or sets whether the trigger's internal state has been modified
        /// and needs to be saved back to the persistence store.
        /// </summary>
        public virtual bool IsDirty
        {
            get { return isDirty; }
            set { isDirty = value; }
        }

        /// <summary>
        /// Returns true if the trigger is active and may fire again sometime.
        /// If this method returns false, the scheduler may choose to remove the trigger
        /// or delete the job.
        /// </summary>
        public abstract bool IsActive { get; }

        /// <summary>
        /// Gets the time when the trigger is next scheduled to fire or null if the
        /// trigger is not scheduled to fire again based on a time signal.
        /// </summary>
        public abstract DateTime? NextFireTime { get; }

        /// <summary>
        /// Gets the amount of time by which the trigger is permitted to miss the next
        /// scheduled time before a misfire occurs or null to consider the schedule on
        /// time no matter how late it fires.
        /// </summary>
        public abstract TimeSpan? NextMisfireThreshold { get; }

        /// <summary>
        /// Updates the trigger's scheduling state machine in response to a condition
        /// and informs the scheduler as to what action should be taken.
        /// </summary>
        /// <remarks>
        /// The implementation should use the value of the <paramref name="timeBasis"/>
        /// parameter to evaluate its scheduling rules rather than calling <see cref="DateTime.Now" />.
        /// </remarks>
        /// <param name="condition">The reason the trigger is being scheduled</param>
        /// <param name="timeBasis">The time to use as a basis for evaluating scheduling rules and
        /// that should be considered as referring to "Now."</param>
        /// <returns>The action that the scheduler should perform in response</returns>
        public abstract TriggerScheduleAction Schedule(TriggerScheduleCondition condition, DateTime timeBasis);

        /// <summary>
        /// Clones the trigger including a deep copy of all properties.
        /// </summary>
        /// <returns>The cloned trigger</returns>
        public abstract Trigger Clone();

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
