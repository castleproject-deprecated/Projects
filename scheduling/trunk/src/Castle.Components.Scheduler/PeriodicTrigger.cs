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
using System.Globalization;

namespace Castle.Components.Scheduler
{
    /// <summary>
    /// Encapsulates an algorithm for generating regular periodic triggers
    /// relative to some fixed start time.  The trigger will fire repeatedly
    /// every recurrence period until either the remainind number of recurrences
    /// drops to zero, the end time is reached, or the associated job is deleted.
    /// </summary>
    [Serializable]
    public class PeriodicTrigger : Trigger
    {
        /// <summary>
        /// The default misfire action.
        /// </summary>
        public const TriggerScheduleAction DefaultMisfireAction = TriggerScheduleAction.Skip;

        private DateTime startTime;
        private DateTime? endTime;
        private TimeSpan? period;
        private int? jobExecutionCountRemaining;

        private TimeSpan? misfireThreshold;
        private TriggerScheduleAction misfireAction;

        private bool isJobExecutionPending;
        private DateTime? nextFireTime;

        /// <summary>
        /// Creates a periodic trigger.
        /// </summary>
        /// <param name="startTime">The date and time when the trigger will first fire</param>
        /// <param name="endTime">The date and time when the trigger must stop firing.
        /// If the time is set to null, the trigger may continue firing indefinitely.</param>
        /// <param name="period">The recurrence period of the trigger.
        /// If the period is set to null, the trigger will fire exactly once
        /// and never recur.</param>
        /// <param name="jobExecutionCount">The number of job executions remaining before the trigger
        /// stops firing.  This number is decremented each time the job executes
        /// until it reaches zero.  If the count is set to null, the number of times the job
        /// may execute is unlimited.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="period"/> is negative or zero</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="jobExecutionCount"/> is negative</exception>
        public PeriodicTrigger(DateTime startTime, DateTime? endTime, TimeSpan? period, int? jobExecutionCount)
        {
            if (period.HasValue && period.Value.Ticks <= 0)
                throw new ArgumentOutOfRangeException("value", "The recurrence period must not be negative or zero.");
            if (jobExecutionCount.HasValue && jobExecutionCount.Value < 0)
                throw new ArgumentOutOfRangeException("value", "The job execution count remaining must not be negative.");

            this.startTime = startTime;
            this.endTime = endTime;
            this.period = period;
            this.jobExecutionCountRemaining = jobExecutionCount;

            misfireAction = DefaultMisfireAction;
        }

        /// <summary>
        /// Creates a trigger that fires exactly once at the specified time.
        /// </summary>
        /// <param name="time">The time at which the trigger should fire</param>
        /// <returns>The one-shot trigger</returns>
        public static PeriodicTrigger CreateOneShotTrigger(DateTime time)
        {
            return new PeriodicTrigger(time, null, null, 1);
        }


        /// <summary>
        /// Creates a trigger that fires every day beginning at the specified start time.
        /// </summary>
        /// <param name="startTime">The date and time when the trigger will first fire</param>
        public static PeriodicTrigger CreateDailyTrigger(DateTime startTime)
        {
            return new PeriodicTrigger(startTime, null, new TimeSpan(24, 0, 0), null);
        }

        /// <summary>
        /// Gets or sets the date and time when the trigger will first fire.
        /// </summary>
        public DateTime StartTime
        {
            get { return startTime; }
            set
            {
                if (startTime != value)
                {
                    IsDirty = true;
                    startTime = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the date and time when the trigger must stop firing.
        /// If the time is set to null, the trigger may continue firing indefinitely.
        /// </summary>
        public DateTime? EndTime
        {
            get { return endTime; }
            set
            {
                if (endTime != value)
                {
                    IsDirty = true;
                    endTime = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the recurrence period of the trigger.
        /// If the period is set to null, the trigger will fire exactly once
        /// and never recur.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative or zero</exception>
        public TimeSpan? Period
        {
            get { return period; }
            set
            {
                if (value.HasValue && value.Value.Ticks <= 0)
                    throw new ArgumentOutOfRangeException("value", "The recurrence period must not be negative or zero.");

                if (period != value)
                {
                    IsDirty = true;
                    period = value;
                }
            }
        }


        /// <summary>
        /// Gets or sets the number of job executions remaining before the trigger
        /// stops firing.  This number is decremented each time the job executes
        /// until it reaches zero.  If the count is set to null, the number of times
        /// the job may execute is unlimited.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative</exception>
        public int? JobExecutionCountRemaining
        {
            get { return jobExecutionCountRemaining; }
            set
            {
                if (value.HasValue && value.Value < 0)
                    throw new ArgumentOutOfRangeException("value", "The job execution count remaining must not be negative.");

                if (jobExecutionCountRemaining != value)
                {
                    IsDirty = true;
                    jobExecutionCountRemaining = value;
                }
            }
        }
        
        /// <summary>
        /// Gets or sets the amount of time by which the scheduler is permitted to miss
        /// the next scheduled time before a misfire occurs or null if the trigger never misfires.
        /// </summary>
        /// <remarks>
        /// The default is null.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is negative</exception>
        public TimeSpan? MisfireThreshold
        {
            get { return misfireThreshold; }
            set
            {
                if (value.HasValue && value.Value.Ticks < 0)
                    throw new ArgumentOutOfRangeException("value", "The misfire threshold must not be negative.");

                if (misfireThreshold != value)
                {
                    IsDirty = true;
                    misfireThreshold = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the action to perform when the trigger misses a scheduled recurrence.
        /// </summary>
        /// <remarks>
        /// The default is <see cref="TriggerScheduleAction.Skip"/>.
        /// </remarks>
        public TriggerScheduleAction MisfireAction
        {
            get { return misfireAction; }
            set
            {
                if (misfireAction != value)
                {
                    IsDirty = true;
                    misfireAction = value;
                }
            }
        }

        public override DateTime?  NextFireTime
        {
	        get { return nextFireTime; }
        }

        public override TimeSpan? NextMisfireThreshold
        {
            get { return misfireThreshold; }
        }

        public override bool IsActive
        {
            get { return isJobExecutionPending || nextFireTime.HasValue; }
        }

        public override Trigger Clone()
        {
            PeriodicTrigger clone = new PeriodicTrigger(startTime, endTime, period, jobExecutionCountRemaining);
            clone.isJobExecutionPending = isJobExecutionPending;
            clone.nextFireTime = nextFireTime;
            clone.misfireThreshold = misfireThreshold;
            clone.misfireAction = misfireAction;
            clone.IsDirty = IsDirty;

            return clone;
        }

        public override TriggerScheduleAction Schedule(TriggerScheduleCondition condition, DateTime timeBasis)
        {
            IsDirty = true;

            switch (condition)
            {
                case TriggerScheduleCondition.FirstTime:
                    // If this is the first time, then set the schedule time to the start time then
                    // return.  The scheduler should come back soon to check whether the trigger has (mis)fired
                    // so we'll execute the job later on when we get back here.
                    return ScheduleSuggestedAction(TriggerScheduleAction.Skip, timeBasis, true);

                case TriggerScheduleCondition.Misfire:
                    return ScheduleSuggestedAction(misfireAction, timeBasis, false);

                case TriggerScheduleCondition.Fire:
                    return ScheduleSuggestedAction(TriggerScheduleAction.ExecuteJob, timeBasis, false);

                case TriggerScheduleCondition.JobSucceeded:
                case TriggerScheduleCondition.JobFailed:
                    isJobExecutionPending = false;
                    return ScheduleSuggestedAction(TriggerScheduleAction.Skip, timeBasis, false);

                default:
                    throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
                        "Unrecognized trigger schedule condition '{0}'.", condition));
            }
        }

        private TriggerScheduleAction ScheduleSuggestedAction(TriggerScheduleAction action, DateTime timeBasis, bool isFirstTime)
        {
            switch (action)
            {
                case TriggerScheduleAction.Stop:
                    break;

                case TriggerScheduleAction.ExecuteJob:
                    // If the job cannot execute again then stop.
                    if (jobExecutionCountRemaining.HasValue && jobExecutionCountRemaining.Value <= 0)
                        break;

                    // If the end time has passed then stop.
                    if (endTime.HasValue && timeBasis > endTime.Value)
                        break;

                    // If the start time is still in the future then hold off until then.
                    if (timeBasis < startTime)
                    {
                        nextFireTime = startTime;
                        return TriggerScheduleAction.Skip;
                    }

                    // Otherwise execute the job.
                    nextFireTime = null;
                    jobExecutionCountRemaining -= 1;
                    isJobExecutionPending = true;
                    return TriggerScheduleAction.ExecuteJob;

                case TriggerScheduleAction.DeleteJob:
                    nextFireTime = null;
                    jobExecutionCountRemaining = 0;
                    return TriggerScheduleAction.DeleteJob;

                case TriggerScheduleAction.Skip:
                    // If the job cannot execute again then stop.
                    if (jobExecutionCountRemaining.HasValue && jobExecutionCountRemaining.Value <= 0)
                        break;

                    // If the start time is still in the future then hold off until then.
                    if (isFirstTime || timeBasis < startTime)
                    {
                        nextFireTime = startTime;
                        return TriggerScheduleAction.Skip;
                    }

                    // If the trigger is not periodic then we must be skipping the only chance the
                    // job had to run so stop the trigger.
                    if (! period.HasValue)
                        break;

                    // Compute when the next occurrence should be.
                    TimeSpan timeSinceStart = timeBasis - startTime;
                    TimeSpan timeSinceLastPeriod = new TimeSpan(timeSinceStart.Ticks % period.Value.Ticks);
                    nextFireTime = timeBasis + period - timeSinceLastPeriod;
                    
                    // If the next occurrence is past the end time then stop.
                    if (nextFireTime > endTime)
                        break;

                    // Otherwise we're good.
                    return TriggerScheduleAction.Skip;
            }

            // Stop the trigger.
            nextFireTime = null;
            jobExecutionCountRemaining = 0;
            return TriggerScheduleAction.Stop;
        }
    }
}
