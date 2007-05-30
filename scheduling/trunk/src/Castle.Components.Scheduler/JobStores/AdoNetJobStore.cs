using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using Castle.Core;

namespace Castle.Components.Scheduler.JobStores
{
    /// <summary>
    /// Abstract base class for an ADO.Net based job store that maintains all job state
    /// in a database.  Jobs are persisted across processes and are shared among all
    /// scheduler instances that belong to the same cluster.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each active scheduler instance is represented as a row in the database.
    /// When the scheduler instance shuts down (normally or abnormally), its
    /// information expires.  If the scheduler instance had any running jobs
    /// associated with it, they are transitioned into the Orphaned state and
    /// reclaimed automatically.
    /// </para>
    /// <para>
    /// The database schema and stored procedures must be deployed to the database
    /// manually or by some other means before the SQL Server job store is used.
    /// For enhanced security, the database user specified in the connection
    /// string should only be a member of the SchedulerRole (thus having EXECUTE
    /// permission to the stored procedures but no direct access to the tables.)
    /// </para>
    /// </remarks>
    [Singleton]
    public abstract class AdoNetJobStore : BaseJobStore
    {
        private readonly object syncRoot = new object();

        private string connectionString;
        private string parameterPrefix;

        private string clusterName;
        private int schedulerExpirationTimeInSeconds;
        private int pollIntervalInSeconds;

        private Dictionary<Guid, string> registeredSchedulers;
        private BinaryFormatter formatter;

        private Timer registrationTimer;

        /// <summary>
        /// Creates an ADO.Net based job store.
        /// </summary>
        /// <param name="connectionString">The connection string</param>
        /// <param name="parameterPrefix">The stored procedure parameter prefix, if any</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="connectionString"/> or <paramref name="parameterPrefix"/> is null</exception>
        protected AdoNetJobStore(string connectionString, string parameterPrefix)
        {
            if (connectionString == null)
                throw new ArgumentNullException("connectionString");
            if (parameterPrefix == null)
                throw new ArgumentNullException("parameterPrefix");

            this.connectionString = connectionString;
            this.parameterPrefix = parameterPrefix;

            clusterName = "Default";
            schedulerExpirationTimeInSeconds = 120;
            pollIntervalInSeconds = 15;

            registeredSchedulers = new Dictionary<Guid, string>();

            formatter = new BinaryFormatter();
            formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
            formatter.FilterLevel = TypeFilterLevel.Low;
            formatter.TypeFormat = FormatterTypeStyle.TypesAlways;

            StartRegistrationTimer();
        }

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        public string ConnectionString
        {
            get { return connectionString; }
        }

        /// <summary>
        /// Gets or sets the unique name of the cluster to which scheduler
        /// instances using this job store should belong.
        /// </summary>
        /// <remarks>
        /// The default value is "Default".
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="value"/> is null</exception>
        public string ClusterName
        {
            get { return clusterName; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                clusterName = value;
            }
        }

        /// <summary>
        /// Gets or sets the number of seconds before a scheduler instance that has lost database
        /// connectivity expires.  Any jobs that the scheduler instance was running will be orphaned when
        /// the scheduler instance expires.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The default value is 120 (2 minutes).
        /// </para>
        /// <para>
        /// The same value must be used in each job store instance belonging to the same cluster
        /// to ensure correct behavior.
        /// </para>
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less
        /// than or equal to 0</exception>
        public int SchedulerExpirationTimeInSeconds
        {
            get { return schedulerExpirationTimeInSeconds; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "The scheduler expiration time must be greater than 0.");

                lock (syncRoot)
                {
                    if (value != schedulerExpirationTimeInSeconds)
                    {
                        schedulerExpirationTimeInSeconds = value;

                        StopRegistrationTimer();
                        StartRegistrationTimer();
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the number of seconds to wait between database polls for new jobs to be processed.
        /// </summary>
        /// <remarks>
        /// The default value is 15.
        /// </remarks>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if <paramref name="value"/> is less
        /// than or equal to 0</exception>
        public int PollIntervalInSeconds
        {
            get { return pollIntervalInSeconds; }
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException("value", "The polling interval must be greater than 0.");

                pollIntervalInSeconds = value;
            }
        }

        public override void Dispose()
        {
            lock (syncRoot)
            {
                StopRegistrationTimer();

                IsDisposed = true;
                Monitor.PulseAll(syncRoot);
            }
        }

        public override void RegisterScheduler(Guid schedulerGuid, string schedulerName)
        {
            if (schedulerName == null)
                throw new ArgumentNullException("schedulerName");

            ThrowIfDisposed();

            lock (registeredSchedulers)
            {
                registeredSchedulers[schedulerGuid] = schedulerName;

                DateTime lastSeen = DateTime.UtcNow;
                DbRegisterScheduler(schedulerGuid, schedulerName, lastSeen);
            }
        }

        public override void UnregisterScheduler(Guid schedulerGuid)
        {
            ThrowIfDisposed();

            lock (registeredSchedulers)
            {
                registeredSchedulers.Remove(schedulerGuid);
                DbUnregisterScheduler(schedulerGuid);
            }
        }

        public override IJobWatcher CreateJobWatcher(Guid schedulerGuid)
        {
            ThrowIfDisposed();

            return new JobWatcher(this, schedulerGuid);
        }

        public override JobDetails GetJobDetails(string jobName)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            ThrowIfDisposed();

            return DbGetJobDetails(jobName);
        }

        public override void SaveJobDetails(JobDetails jobDetails)
        {
            if (jobDetails == null)
                throw new ArgumentNullException("jobStatus");

            VersionedJobDetails versionedJobDetails = (VersionedJobDetails)jobDetails;

            ThrowIfDisposed();

            DbSaveJobDetails(versionedJobDetails);

            jobDetails.JobSpec.Trigger.IsDirty = false;
        }

        public override bool CreateJob(JobSpec jobSpec, JobData jobData, DateTime creationTime, CreateJobConflictAction conflictAction)
        {
            if (jobSpec == null)
                throw new ArgumentNullException("jobSpec");

            ThrowIfDisposed();

            return DbCreateJob(jobSpec, jobData, creationTime, conflictAction);
        }

        public override bool DeleteJob(string jobName)
        {
            if (jobName == null)
                throw new ArgumentNullException("jobName");

            ThrowIfDisposed();

            return DbDeleteJob(jobName);
        }

        protected override void SignalBlockedThreads()
        {
            lock (syncRoot)
                Monitor.PulseAll(syncRoot);
        }

        protected override JobDetails GetNextJobToProcessOrWaitUntilSignaled(Guid schedulerGuid)
        {
            lock (syncRoot)
            {
                ThrowIfDisposed();

                DateTime timeBasis = DateTime.UtcNow;
                DateTime? waitNextTriggerFireTime;
                try
                {
                    VersionedJobDetails nextJob = DbGetNextJobToProcess(schedulerGuid, timeBasis, out waitNextTriggerFireTime);

                    if (nextJob != null)
                        return nextJob;
                }
                catch (Exception ex)
                {
                    Logger.Warn(String.Format("The job store was unable to poll the database for the next job to process.  "
                        + "It will try again in {0} seconds.", pollIntervalInSeconds), ex);
                    waitNextTriggerFireTime = null;
                }

                // Wait for a signal or the next fire time whichever comes first.
                if (waitNextTriggerFireTime.HasValue)
                {
                    // Update the time basis because the SP may have taken a non-trivial
                    // amount of time to run.
                    timeBasis = DateTime.UtcNow;
                    if (waitNextTriggerFireTime.Value <= timeBasis)
                        return null;

                    // Need to ensure that wait time in millis will fit in a 32bit integer, otherwise the
                    // Monitor.Wait will throw an ArgumentException (even when using the TimeSpan based overload).
                    // This can happen when the next trigger fire time is very far out into the future like DateTime.MaxValue.
                    TimeSpan waitTimeSpan = waitNextTriggerFireTime.Value - timeBasis;
                    int waitMillis = Math.Min((int)Math.Min(int.MaxValue, waitTimeSpan.TotalMilliseconds), pollIntervalInSeconds * 1000);
                    Monitor.Wait(syncRoot, waitMillis);
                }
                else
                {
                    Monitor.Wait(syncRoot, pollIntervalInSeconds * 1000);
                }
            }

            return null;
        }

        /// <summary>
        /// Starts a timer to refresh registrations at 1/3 of the scheduler expiration rate.
        /// This ensures that the scheduler instances get 2 chances to be refreshed before
        /// they expire.
        /// </summary>
        private void StartRegistrationTimer()
        {
            lock (syncRoot)
            {
                ThrowIfDisposed();

                if (registrationTimer == null)
                {
                    int period = schedulerExpirationTimeInSeconds * 333;
                    registrationTimer = new Timer(RefreshRegistrations, null, period, period);
                }
            }
        }

        /// <summary>
        /// Stops the registration refresh timer.
        /// </summary>
        private void StopRegistrationTimer()
        {
            lock (syncRoot)
            {
                if (registrationTimer != null)
                {
                    registrationTimer.Dispose();
                    registrationTimer = null;
                }
            }
        }

        private void RefreshRegistrations(object dummy)
        {
            try
            {
                lock (registeredSchedulers)
                {
                    DateTime lastSeen = DateTime.UtcNow;

                    foreach (KeyValuePair<Guid, string> entry in registeredSchedulers)
                        DbRegisterScheduler(entry.Key, entry.Value, lastSeen);
                }
            }
            catch (Exception ex)
            {
                Logger.Warn(String.Format(CultureInfo.CurrentCulture,
                    "The job store was unable to refresh certain scheduler instance "
                    + "registrations in the database.  It will try again in {0} seconds.", schedulerExpirationTimeInSeconds * 333), ex);
            }
        }

        protected virtual void DbRegisterScheduler(Guid schedulerGuid, string schedulerName, DateTime lastSeen)
        {
            try
            {
                using (IDbConnection connection = CreateConnection())
                {
                    IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_RegisterScheduler");

                    AddInputParameter(command, "ClusterName", DbType.String, clusterName);
                    AddInputParameter(command, "SchedulerGUID", DbType.Guid, schedulerGuid);
                    AddInputParameter(command, "SchedulerName", DbType.String, schedulerName);
                    AddInputParameter(command, "LastSeen", DbType.DateTime, lastSeen.ToUniversalTime());

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new SchedulerException("The job store was unable to register a scheduler instance in the database.", ex);
            }
        }

        protected virtual void DbUnregisterScheduler(Guid schedulerGuid)
        {
            try
            {
                using (IDbConnection connection = CreateConnection())
                {
                    IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_UnregisterScheduler");

                    AddInputParameter(command, "ClusterName", DbType.String, clusterName);
                    AddInputParameter(command, "SchedulerGUID", DbType.Guid, schedulerGuid);

                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new SchedulerException("The job store was unable to unregister a scheduler instance in the database.", ex);
            }
        }

        protected virtual bool DbCreateJob(JobSpec jobSpec, JobData jobData, DateTime creationTime, CreateJobConflictAction conflictAction)
        {
            try
            {
                using (IDbConnection connection = CreateConnection())
                {
                    IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_CreateJob");

                    AddInputParameter(command, "ClusterName", DbType.String, clusterName);
                    AddInputParameter(command, "JobName", DbType.String, jobSpec.Name);
                    AddInputParameter(command, "JobDescription", DbType.String, jobSpec.Description);
                    AddInputParameter(command, "JobKey", DbType.String, jobSpec.JobKey);
                    AddInputParameter(command, "TriggerObject", DbType.Binary, MapObjectToDbValue(SerializeObject(jobSpec.Trigger)));
                    AddInputParameter(command, "JobDataObject", DbType.Binary, MapObjectToDbValue(SerializeObject(jobData)));
                    AddInputParameter(command, "CreationTime", DbType.DateTime, creationTime.ToUniversalTime());
                    AddInputParameter(command, "ReplaceExisting", DbType.Boolean, conflictAction == CreateJobConflictAction.Update);
                    IDbDataParameter wasCreatedParam = AddOutputParameter(command, "WasCreated", DbType.Boolean);

                    connection.Open();
                    command.ExecuteNonQuery();

                    bool wasCreated = (bool)wasCreatedParam.Value;
                    if (!wasCreated && conflictAction == CreateJobConflictAction.Throw)
                        throw new SchedulerException(String.Format(CultureInfo.CurrentCulture,
                            "Job '{0}' already exists.", jobSpec.Name));

                    return wasCreated;
                }
            }
            catch (Exception ex)
            {
                throw new SchedulerException("The job store was unable to create a job in the database.", ex);
            }
        }

        protected virtual bool DbDeleteJob(string jobName)
        {
            try
            {
                using (IDbConnection connection = CreateConnection())
                {
                    IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_DeleteJob");

                    AddInputParameter(command, "ClusterName", DbType.String, clusterName);
                    AddInputParameter(command, "JobName", DbType.String, jobName);
                    IDbDataParameter wasDeletedParam = AddOutputParameter(command, "WasDeleted", DbType.Boolean);

                    connection.Open();
                    command.ExecuteNonQuery();

                    return (bool)wasDeletedParam.Value;
                }
            }
            catch (Exception ex)
            {
                throw new SchedulerException("The job store was unable to delete a job in the database.", ex);
            }
        }

        protected virtual VersionedJobDetails DbGetJobDetails(string jobName)
        {
            try
            {
                using (IDbConnection connection = CreateConnection())
                {
                    IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_GetJobDetails");

                    AddInputParameter(command, "ClusterName", DbType.String, clusterName);
                    AddInputParameter(command, "JobName", DbType.String, jobName);

                    connection.Open();

                    VersionedJobDetails jobDetails;
                    using (IDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
                    {
                        jobDetails = reader.Read() ? BuildJobDetailsFromResultSet(reader) : null;
                    }

                    return jobDetails;
                }
            }
            catch (Exception ex)
            {
                throw new SchedulerException("The job store was unable to get job details from the database.", ex);
            }
        }

        protected virtual void DbSaveJobDetails(VersionedJobDetails jobDetails)
        {
            try
            {
                using (IDbConnection connection = CreateConnection())
                {
                    IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_SaveJobDetails");

                    AddInputParameter(command, "ClusterName", DbType.String, clusterName);

                    AddInputParameter(command, "JobName", DbType.String, jobDetails.JobSpec.Name);
                    AddInputParameter(command, "JobDescription", DbType.String, jobDetails.JobSpec.Description);
                    AddInputParameter(command, "JobKey", DbType.String, jobDetails.JobSpec.JobKey);
                    AddInputParameter(command, "TriggerObject", DbType.Binary, MapObjectToDbValue(SerializeObject(jobDetails.JobSpec.Trigger)));

                    AddInputParameter(command, "JobDataObject", DbType.Binary, MapObjectToDbValue(SerializeObject(jobDetails.JobData)));
                    AddInputParameter(command, "JobState", DbType.Int32, jobDetails.JobState);
                    AddInputParameter(command, "NextTriggerFireTime", DbType.DateTime, MapNullableToDbValue(ToUniversalTime(jobDetails.NextTriggerFireTime)));
                    int? nextTriggerMisfireThresholdSeconds = jobDetails.NextTriggerMisfireThreshold.HasValue ?
                        (int?) jobDetails.NextTriggerMisfireThreshold.Value.TotalSeconds : null;
                    AddInputParameter(command, "NextTriggerMisfireThresholdSeconds", DbType.Int32, MapNullableToDbValue(nextTriggerMisfireThresholdSeconds));

                    JobExecutionDetails execution = jobDetails.LastJobExecutionDetails;
                    AddInputParameter(command, "LastExecutionSchedulerGUID", DbType.Guid,
                        execution != null ? (object) execution.SchedulerGuid : DBNull.Value);
                    AddInputParameter(command, "LastExecutionStartTime", DbType.DateTime,
                        execution != null ? (object)execution.StartTime.ToUniversalTime() : DBNull.Value);
                    AddInputParameter(command, "LastExecutionEndTime", DbType.DateTime,
                        execution != null ? MapNullableToDbValue(ToUniversalTime(execution.EndTime)) : DBNull.Value);
                    AddInputParameter(command, "LastExecutionSucceeded", DbType.Boolean,
                        execution != null ? (object)execution.Succeeded : DBNull.Value);
                    AddInputParameter(command, "LastExecutionStatusMessage", DbType.String,
                        execution != null ? (object) execution.StatusMessage : DBNull.Value);

                    AddInputParameter(command, "Version", DbType.Int32, jobDetails.Version);

                    IDbDataParameter wasSavedParam = AddOutputParameter(command, "WasSaved", DbType.Boolean);

                    connection.Open();
                    command.ExecuteNonQuery();

                    bool wasSaved = (bool)wasSavedParam.Value;
                    if (! wasSaved)
                        throw new ConcurrentModificationException(String.Format("Job '{0}' was concurrently modified in the database.",
                            jobDetails.JobSpec.Name));

                    jobDetails.Version += 1;
                }
            }
            catch (Exception ex)
            {
                throw new SchedulerException("The job store was unable to save job details to the database.", ex);
            }
        }

        protected virtual VersionedJobDetails DbGetNextJobToProcess(Guid schedulerGuid, DateTime timeBasis, out DateTime? nextTriggerFireTime)
        {
            try
            {
                using (IDbConnection connection = CreateConnection())
                {
                    IDbCommand command = CreateStoredProcedureCommand(connection, "spSCHED_GetNextJobToProcess");

                    AddInputParameter(command, "ClusterName", DbType.String, clusterName);
                    AddInputParameter(command, "SchedulerGUID", DbType.Guid, schedulerGuid);
                    AddInputParameter(command, "TimeBasis", DbType.DateTime, timeBasis.ToUniversalTime());
                    AddInputParameter(command, "SchedulerExpirationTimeInSeconds", DbType.Int32, schedulerExpirationTimeInSeconds);
                    IDbDataParameter nextTriggerFireTimeParam = AddOutputParameter(command, "NextTriggerFireTime", DbType.DateTime);

                    connection.Open();

                    VersionedJobDetails jobDetails;
                    using (IDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult | CommandBehavior.SingleRow))
                    {
                        jobDetails = reader.Read() ? BuildJobDetailsFromResultSet(reader) : null;
                    }

                    nextTriggerFireTime = AssumeUniversalTime(MapDbValueToNullable<DateTime>(nextTriggerFireTimeParam.Value));
                    return jobDetails;
                }
            }
            catch (Exception ex)
            {
                throw new SchedulerException("The job store was unable to get job details for the next job to process from the database.", ex);
            }
        }

        protected virtual VersionedJobDetails BuildJobDetailsFromResultSet(IDataReader reader)
        {
            string jobName = reader.GetString(0);
            string jobDescription = reader.GetString(1);
            string jobKey = reader.GetString(2);
            Trigger trigger = (Trigger)DeserializeObject(MapDbValueToObject<byte[]>(reader.GetValue(3)));

            JobData jobData = (JobData)DeserializeObject(MapDbValueToObject<byte[]>(reader.GetValue(4)));
            DateTime creationTime = AssumeUniversalTime(reader.GetDateTime(5));
            JobState jobState = (JobState)reader.GetInt32(6);
            DateTime? nextTriggerFireTime = AssumeUniversalTime(MapDbValueToNullable<DateTime>(reader.GetValue(7)));
            int? nextTriggerMisfireThresholdSeconds = MapDbValueToNullable<int>(reader.GetValue(8));
            TimeSpan? nextTriggerMisfireThreshold = nextTriggerMisfireThresholdSeconds.HasValue ?
                new TimeSpan(0, 0, nextTriggerMisfireThresholdSeconds.Value) : (TimeSpan?) null;

            Guid? lastExecutionSchedulerGuid = MapDbValueToNullable<Guid>(reader.GetValue(9));
            DateTime? lastExecutionStartTime = AssumeUniversalTime(MapDbValueToNullable<DateTime>(reader.GetValue(10)));
            DateTime? lastExecutionEndTime = AssumeUniversalTime(MapDbValueToNullable<DateTime>(reader.GetValue(11)));
            bool? lastExecutionSucceeded = MapDbValueToNullable<bool>(reader.GetValue(12));
            string lastExecutionStatusMessage = MapDbValueToObject<string>(reader.GetValue(13));

            int version = reader.GetInt32(14);

            JobSpec jobSpec = new JobSpec(jobName, jobDescription, jobKey, trigger);
            VersionedJobDetails details = new VersionedJobDetails(jobSpec, creationTime, version);
            details.JobData = jobData;
            details.JobState = jobState;
            details.NextTriggerFireTime = nextTriggerFireTime;
            details.NextTriggerMisfireThreshold = nextTriggerMisfireThreshold;

            if (lastExecutionSchedulerGuid.HasValue && lastExecutionStartTime.HasValue)
            {
                JobExecutionDetails execution = new JobExecutionDetails(lastExecutionSchedulerGuid.Value, lastExecutionStartTime.Value);
                execution.EndTime = lastExecutionEndTime;
                execution.Succeeded = lastExecutionSucceeded.GetValueOrDefault();
                execution.StatusMessage = lastExecutionStatusMessage == null ? "" : lastExecutionStatusMessage;

                details.LastJobExecutionDetails = execution;
            }

            return details;
        }

        protected static T MapDbValueToObject<T>(object value) where T : class
        {
            if (value == DBNull.Value)
                return null;
            return (T)value;
        }

        protected static T? MapDbValueToNullable<T>(object value) where T : struct
        {
            if (value == null || value == DBNull.Value)
                return null;
            return (T?)value;
        }

        protected static object MapNullableToDbValue<T>(T? value) where T : struct
        {
            if (! value.HasValue)
                return DBNull.Value;
            return value.Value;
        }

        protected static object MapObjectToDbValue<T>(T value) where T : class
        {
            if (value == null)
                return DBNull.Value;
            return value;
        }

        protected static DateTime? ToUniversalTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? dateTime.Value.ToUniversalTime() : (DateTime?) null;
        }

        protected static DateTime? AssumeUniversalTime(DateTime? dateTime)
        {
            return dateTime.HasValue ? AssumeUniversalTime(dateTime.Value) : (DateTime?) null;
        }

        protected static DateTime AssumeUniversalTime(DateTime dateTime)
        {
            return new DateTime(dateTime.Ticks, DateTimeKind.Utc);
        }

        protected abstract IDbConnection CreateConnection();

        protected virtual IDbCommand CreateStoredProcedureCommand(IDbConnection connection, string spName)
        {
            IDbCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = spName;
            return command;
        }

        protected virtual IDbDataParameter AddParameter(IDbCommand command, string name, DbType type)
        {
            IDbDataParameter parameter = command.CreateParameter();
            parameter.ParameterName = parameterPrefix + name;
            parameter.DbType = type;

            command.Parameters.Add(parameter);
            return parameter;
        }

        protected IDbDataParameter AddInputParameter(IDbCommand command, string name, DbType type, object value)
        {
            IDbDataParameter parameter = AddParameter(command, name, type);
            parameter.Direction = ParameterDirection.Input;
            parameter.Value = value;
            return parameter;
        }

        protected IDbDataParameter AddOutputParameter(IDbCommand command, string name, DbType type)
        {
            IDbDataParameter parameter = AddParameter(command, name, type);
            parameter.Direction = ParameterDirection.Output;
            return parameter;
        }

        protected byte[] SerializeObject(object obj)
        {
            if (obj == null)
                return null;

            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, obj);
            return stream.ToArray();
        }

        protected object DeserializeObject(byte[] bytes)
        {
            if (bytes == null)
                return null;

            MemoryStream stream = new MemoryStream(bytes);
            return formatter.Deserialize(stream);
        }
    }
}
