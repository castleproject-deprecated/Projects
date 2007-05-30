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
using MbUnit.Framework;

namespace Castle.Components.Scheduler.Tests.UnitTests
{
    [TestFixture]
    [TestsOn(typeof(PeriodicTrigger))]
    [Author("Jeff Brown", "jeff@ingenio.com")]
    public class PeriodicTriggerTest : BaseUnitTest
    {
        [Test]
        public void ConstructorSetsProperties()
        {
            DateTime now = DateTime.UtcNow;
            TimeSpan interval = new TimeSpan(0, 1, 30);
            PeriodicTrigger trigger = new PeriodicTrigger(now, DateTime.MaxValue, interval, 33);

            Assert.AreEqual(now, trigger.StartTime);
            Assert.AreEqual(interval, trigger.Period);
            Assert.IsFalse(trigger.IsDirty);
            Assert.AreEqual(PeriodicTrigger.DefaultMisfireAction, trigger.MisfireAction);
            Assert.AreEqual(null, trigger.MisfireThreshold);
            Assert.AreEqual(DateTime.MaxValue, trigger.EndTime);
            Assert.AreEqual(33, trigger.JobExecutionCountRemaining);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ThrowsIfPeriodIsZero()
        {
            new PeriodicTrigger(DateTime.UtcNow, null, TimeSpan.Zero, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ThrowsIfPeriodIsNegative()
        {
            new PeriodicTrigger(DateTime.UtcNow, null, TimeSpan.MinValue, null);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Constructor_ThrowsIfJobExecutionCountIsNegative()
        {
            new PeriodicTrigger(DateTime.UtcNow, null, null, -1);
        }

        [Test]
        public void StartTime_SettingANewValueMakesTriggerDirty()
        {
            DateTime value = DateTime.MinValue;
            PeriodicTrigger trigger = new PeriodicTrigger(value, null, null, null);

            trigger.StartTime = value;
            Assert.IsFalse(trigger.IsDirty);

            value = DateTime.UtcNow;
            trigger.StartTime = value;
            Assert.AreEqual(value, trigger.StartTime);
            Assert.IsTrue(trigger.IsDirty);
        }

        [Test]
        public void EndTime_SettingANewValueMakesTriggerDirty()
        {
            DateTime value = DateTime.MinValue;
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, value, null, null);

            trigger.EndTime = value;
            Assert.IsFalse(trigger.IsDirty);

            value = DateTime.UtcNow;
            trigger.EndTime = value;
            Assert.AreEqual(value, trigger.EndTime);
            Assert.IsTrue(trigger.IsDirty);
        }

        [Test]
        public void Period_SettingANewValueMakesTriggerDirty()
        {
            TimeSpan value = TimeSpan.MaxValue;
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, value, null);

            trigger.Period = value;
            Assert.IsFalse(trigger.IsDirty);

            value = new TimeSpan(0, 1, 0);
            trigger.Period = value;
            Assert.AreEqual(value, trigger.Period);
            Assert.IsTrue(trigger.IsDirty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Period_ThrowsIfValueIsZero()
        {
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);
            trigger.Period = TimeSpan.Zero;
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Period_ThrowsIfValueIsNegative()
        {
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);
            trigger.Period = TimeSpan.MinValue;
        }

        [Test]
        public void JobExecutionCountRemaining_SettingANewValueMakesTriggerDirty()
        {
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, 33);

            trigger.JobExecutionCountRemaining = 33;
            Assert.IsFalse(trigger.IsDirty);

            trigger.JobExecutionCountRemaining = 42;
            Assert.AreEqual(42, trigger.JobExecutionCountRemaining);
            Assert.IsTrue(trigger.IsDirty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void JobExecutionCountRemaining_ThrowsIfValueIsNegative()
        {
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);
            trigger.JobExecutionCountRemaining = -1;
        }

        [Test]
        public void MisfireAction_SettingANewValueMakesTriggerDirty()
        {
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);

            trigger.MisfireAction = PeriodicTrigger.DefaultMisfireAction;
            Assert.IsFalse(trigger.IsDirty);

            trigger.MisfireAction = TriggerScheduleAction.ExecuteJob;
            Assert.AreEqual(TriggerScheduleAction.ExecuteJob, trigger.MisfireAction);
            Assert.IsTrue(trigger.IsDirty);
        }

        [Test]
        public void MisfireThreshold_SettingANewValueMakesTriggerDirty()
        {
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);

            trigger.MisfireThreshold = null;
            Assert.IsFalse(trigger.IsDirty);

            trigger.MisfireThreshold = new TimeSpan(1, 0, 0);
            Assert.AreEqual(new TimeSpan(1, 0, 0), trigger.MisfireThreshold);
            Assert.IsTrue(trigger.IsDirty);
        }

        [Test]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void MisfireThreshold_ThrowsIfValueIsNegative()
        {
            PeriodicTrigger trigger = new PeriodicTrigger(DateTime.UtcNow, null, null, null);
            trigger.MisfireThreshold = TimeSpan.MinValue;
        }

        [Test]
        public void CreateDailyTrigger()
        {
            DateTime now = DateTime.UtcNow;
            PeriodicTrigger trigger = PeriodicTrigger.CreateDailyTrigger(DateTime.UtcNow);

            Assert.AreEqual(now, trigger.StartTime);
            Assert.AreEqual(new TimeSpan(24, 0, 0), trigger.Period);
            Assert.IsNull(trigger.EndTime);
            Assert.IsNull(trigger.JobExecutionCountRemaining);
        }

        [Test]
        public void CreateOneShotTrigger()
        {
            DateTime now = DateTime.UtcNow;
            PeriodicTrigger trigger = PeriodicTrigger.CreateOneShotTrigger(DateTime.UtcNow);

            Assert.AreEqual(now, trigger.StartTime);
            Assert.IsNull(trigger.Period);
            Assert.IsNull(trigger.EndTime);
            Assert.AreEqual(1, trigger.JobExecutionCountRemaining);
        }

        [RowTest]
        [Row(false)]
        [Row(true)]
        public void ClonePerformsADeepCopy(bool useGenericClonable)
        {
            DateTime now = DateTime.UtcNow;
            PeriodicTrigger trigger = new PeriodicTrigger(now, DateTime.MaxValue, TimeSpan.MaxValue, 42);
            trigger.MisfireAction = TriggerScheduleAction.DeleteJob;
            trigger.MisfireThreshold = TimeSpan.MaxValue;
            trigger.Schedule(TriggerScheduleCondition.FirstTime, now);

            PeriodicTrigger clone = useGenericClonable ? (PeriodicTrigger)trigger.Clone()
                : (PeriodicTrigger)((ICloneable)trigger).Clone();

            Assert.AreNotSame(trigger, clone);

            Assert.AreEqual(trigger.StartTime, clone.StartTime);
            Assert.AreEqual(trigger.EndTime, clone.EndTime);
            Assert.AreEqual(trigger.Period, clone.Period);
            Assert.AreEqual(trigger.JobExecutionCountRemaining, clone.JobExecutionCountRemaining);
            Assert.AreEqual(trigger.MisfireAction, clone.MisfireAction);
            Assert.AreEqual(trigger.MisfireThreshold, clone.MisfireThreshold);
            Assert.AreEqual(trigger.IsDirty, clone.IsDirty);

            Assert.AreEqual(trigger.IsActive, clone.IsActive);
            Assert.AreEqual(trigger.NextFireTime, clone.NextFireTime);
            Assert.AreEqual(trigger.NextMisfireThreshold, clone.NextMisfireThreshold);
        }

        [RowTest]
        // One-shot triggers
        [Row(1, 0, 0, 1, TriggerScheduleAction.Skip, TriggerScheduleCondition.FirstTime, 1,
            TriggerScheduleAction.Skip, 1, 1, true, Description="One-shot first time.")]
        [Row(1, 0, 0, 1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1,
            TriggerScheduleAction.ExecuteJob, 0, 0, true, Description = "One-shot fire.")]
        [Row(1, 0, 0, 1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "One-shot misfire skip yields stop.")]
        [Row(1, 0, 0, 1, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.ExecuteJob, 0, 0, true, Description = "One-shot misfire execute yields execute.")]
        [Row(1, 0, 0, 1, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.DeleteJob, 0, 0, false, Description = "One-shot misfire deletejob yields deletejob.")]
        [Row(1, 0, 0, 1, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "One-shot misfire stop yields stop.")]
        [Row(1, 0, 0, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobSucceeded, 1,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "One-shot job succeeded yields stop.")]
        [Row(1, 0, 0, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobFailed, 1,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "One-shot job failed yields stop.")]
        // One-shot triggers, fired before start time
        [Row(3, 0, 0, 1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1,
            TriggerScheduleAction.Skip, 3, 1, true, Description = "One-shot fire before start time.")]
        // Periodic unlimited triggers
        [Row(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.FirstTime, 1,
            TriggerScheduleAction.Skip, 1, -1, true, Description = "Periodic first time.")]
        [Row(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1,
            TriggerScheduleAction.ExecuteJob, 0, -1, true, Description = "Periodic fire.")]
        [Row(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Skip, 3, -1, true, Description = "Periodic misfire skip yields skip.")]
        [Row(1, 0, 1, -1, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.ExecuteJob, 0, -1, true, Description = "Periodic misfire execute yields execute.")]
        [Row(1, 0, 1, -1, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.DeleteJob, 0, 0, false, Description = "Periodic misfire deletejob yields deletejob.")]
        [Row(1, 0, 1, -1, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic misfire stop yields stop.")]
        [Row(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobSucceeded, 1,
            TriggerScheduleAction.Skip, 2, -1, true, Description = "Periodic job succeeded yields skip.")]
        [Row(1, 0, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobFailed, 1,
            TriggerScheduleAction.Skip, 2, -1, true, Description = "Periodic job failed yields skip.")]
        // Periodic execution count limited triggers, non-zero count
        [Row(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.FirstTime, 1,
            TriggerScheduleAction.Skip, 1, 10, true, Description = "Periodic non-zero execution count first time.")]
        [Row(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1,
            TriggerScheduleAction.ExecuteJob, 0, 9, true, Description = "Periodic non-zero execution count fire.")]
        [Row(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Skip, 3, 10, true, Description = "Periodic non-zero execution count misfire skip yields skip.")]
        [Row(1, 0, 1, 10, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.ExecuteJob, 0, 9, true, Description = "Periodic non-zero execution count misfire execute yields execute.")]
        [Row(1, 0, 1, 10, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.DeleteJob, 0, 0, false, Description = "Periodic non-zero execution count misfire deletejob yields deletejob.")]
        [Row(1, 0, 1, 10, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic non-zero execution count misfire stop yields stop.")]
        [Row(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobSucceeded, 1,
            TriggerScheduleAction.Skip, 2, 10, true, Description = "Periodic non-zero execution count job succeeded yields skip.")]
        [Row(1, 0, 1, 10, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobFailed, 1,
            TriggerScheduleAction.Skip, 2, 10, true, Description = "Periodic non-zero execution count job failed yields skip.")]
        // Periodic execution count limited triggers, zero count
        [Row(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.FirstTime, 1,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic zero execution count first time yields stop.")]
        [Row(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 1,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic zero execution count fire yields stop.")]
        [Row(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic zero execution count misfire skip yields stop.")]
        [Row(1, 0, 1, 0, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic zero execution count misfire execute yields stop.")]
        [Row(1, 0, 1, 0, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.DeleteJob, 0, 0, false, Description = "Periodic zero execution count misfire deletejob yields deletejob.")]
        [Row(1, 0, 1, 0, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 2,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic zero execution count misfire stop yields stop.")]
        [Row(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobSucceeded, 1,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic zero execution count job succeeded yields stop.")]
        [Row(1, 0, 1, 0, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobFailed, 1,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic zero execution count job failed yields stop.")]
        // Periodic time limited triggers, over time
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.FirstTime, 4,
            TriggerScheduleAction.Skip, 1, -1, true, Description = "Periodic over-time first time yields skip (because we want to detect the misfire).")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 4,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic over-time fire yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 4,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic over-time misfire skip yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 4,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic over-time misfire execute yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 4,
            TriggerScheduleAction.DeleteJob, 0, 0, false, Description = "Periodic over-time misfire deletejob yields deletejob.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 4,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic over-time misfire stop yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobSucceeded, 4,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic over-time job succeeded yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobFailed, 4,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic over-time job failed yields stop.")]
        // Periodic time limited triggers, exactly at end time
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.FirstTime, 3,
            TriggerScheduleAction.Skip, 1, -1, true, Description = "Periodic at end-time first time yields skip (because we want to detect the misfire).")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Fire, 3,
            TriggerScheduleAction.ExecuteJob, 0, -1, true, Description = "Periodic at end-time fire yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.Misfire, 3,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic at end-time misfire skip yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.ExecuteJob, TriggerScheduleCondition.Misfire, 3,
            TriggerScheduleAction.ExecuteJob, 0, -1, true, Description = "Periodic at end-time misfire execute yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.DeleteJob, TriggerScheduleCondition.Misfire, 3,
            TriggerScheduleAction.DeleteJob, 0, 0, false, Description = "Periodic at end-time misfire deletejob yields deletejob.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Stop, TriggerScheduleCondition.Misfire, 3,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic at end-time misfire stop yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobSucceeded, 3,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic at end-time job succeeded yields stop.")]
        [Row(1, 3, 1, -1, TriggerScheduleAction.Skip, TriggerScheduleCondition.JobFailed, 3,
            TriggerScheduleAction.Stop, 0, 0, false, Description = "Periodic at end-time job failed yields stop.")]
        public void Schedule(int startDay, int endDay, int periodDays, int jobExecutionCount, TriggerScheduleAction misfireAction,
            TriggerScheduleCondition condition, int timeBasisDay,
            TriggerScheduleAction expectedAction, int expectedFireDay,
            int expectedJobExecutionCountRemaining, bool expectedIsActive)
        {
            DateTime startTime = new DateTime(1970, 1, startDay);
            DateTime? endTime = endDay != 0 ? new DateTime(1970, 1, endDay) : (DateTime?) null;
            TimeSpan? period = periodDays != 0 ? new TimeSpan(periodDays, 0, 0, 0) : (TimeSpan?)null;
            int? jobExecutionCountArg = jobExecutionCount >= 0 ? jobExecutionCount : (int?) null;
            DateTime timeBasis = new DateTime(1970, 1, timeBasisDay);
            DateTime? expectedFireTime = expectedFireDay != 0 ? new DateTime(1970, 1, expectedFireDay) : (DateTime?) null;
            int? expectedJobExecutionCountRemainingArg = expectedJobExecutionCountRemaining >= 0 ? expectedJobExecutionCountRemaining : (int?)null;

            PeriodicTrigger trigger = new PeriodicTrigger(startTime, endTime, period, jobExecutionCountArg);
            trigger.MisfireAction = misfireAction;
            trigger.MisfireThreshold = TimeSpan.Zero;
            trigger.IsDirty = false;

            TriggerScheduleAction action = trigger.Schedule(condition, timeBasis);
            Assert.AreEqual(expectedAction, action);
            Assert.AreEqual(expectedFireTime, trigger.NextFireTime);
            Assert.AreEqual(expectedJobExecutionCountRemainingArg, trigger.JobExecutionCountRemaining);
            Assert.AreEqual(expectedIsActive, trigger.IsActive);
            Assert.IsTrue(trigger.IsDirty);
        }

        [Test]
        public void SchedulePeriodicTriggerCorrectlyComputesNextCycle()
        {
            // Constructs a 3-day period trigger on an odd date.
            PeriodicTrigger trigger = new PeriodicTrigger(new DateTime(1970, 1, 5), null, new TimeSpan(3, 0, 0, 0), null);
            trigger.MisfireAction = TriggerScheduleAction.Skip;
            trigger.MisfireThreshold = TimeSpan.Zero;

            // Ensure returns next period if skipping with time basis on an even boundary.
            trigger.Schedule(TriggerScheduleCondition.Misfire, new DateTime(1970, 1, 8));
            Assert.AreEqual(new DateTime(1970, 1, 11), trigger.NextFireTime);

            // Ensure returns next period if skipping with time basis at 1/3 beyond even boundary.
            trigger.Schedule(TriggerScheduleCondition.Misfire, new DateTime(1970, 1, 9));
            Assert.AreEqual(new DateTime(1970, 1, 11), trigger.NextFireTime);

            // Ensure returns next period if skipping with time basis at 2/3 beyond even boundary.
            trigger.Schedule(TriggerScheduleCondition.Misfire, new DateTime(1970, 1, 10));
            Assert.AreEqual(new DateTime(1970, 1, 11), trigger.NextFireTime);
        }

        [Test]
        [ExpectedException(typeof(SchedulerException))]
        public void ScheduleThrowsIfConditionIsUnrecognized()
        {
            PeriodicTrigger trigger = new PeriodicTrigger(new DateTime(1970, 1, 5), null, new TimeSpan(3, 0, 0, 0), null);
            trigger.Schedule((TriggerScheduleCondition) 9999, DateTime.UtcNow);
        }
    }
}
