using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using Castle.Components.Scheduler.JobStores;
using MbUnit.Framework;

namespace Castle.Components.Scheduler.Tests.UnitTests.JobStores
{
    [TestFixture(TimeOut = 1)]
    [TestsOn(typeof(SqlServerJobStore))]
    [Author("Jeff Brown", "jeff@ingenio.com")]
    public class SqlServerJobStoreTest : AdoNetJobStoreTest
    {
        private const string ConnectionString = "server=.; database=SchedulerTestDb; uid=SchedulerTestUser; pwd=test;";

        public override void SetUp()
        {
            PurgeAllData();

            base.SetUp();
        }

        protected override AdoNetJobStore CreateAdoNetJobStore()
        {
            return new InstrumentedSqlServerJobStore(ConnectionString);
        }

        protected override void SetBrokenConnectionMocking(AdoNetJobStore jobStore, bool brokenConnections)
        {
            InstrumentedSqlServerJobStore instrumentedJobStore = (InstrumentedSqlServerJobStore)jobStore;
            instrumentedJobStore.BrokenConnections = brokenConnections;
        }

        protected void PurgeAllData()
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                SqlCommand command = new SqlCommand("spSCHED_TEST_PurgeAllData", connection);
                command.CommandType = CommandType.StoredProcedure;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private class InstrumentedSqlServerJobStore : SqlServerJobStore
        {
            private bool brokenConnections;

            public InstrumentedSqlServerJobStore(string connectionString)
                : base(connectionString)
            {
            }

            public bool BrokenConnections
            {
                get { return brokenConnections; }
                set { brokenConnections = value; }
            }

            protected override IDbConnection CreateConnection()
            {
                if (brokenConnections)
                    throw new Exception("Simulated Db connection failure.");

                return base.CreateConnection();
            }
        }
    }
}