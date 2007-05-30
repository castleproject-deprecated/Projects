using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Castle.Core;

namespace Castle.Components.Scheduler.JobStores
{
    /// <summary>
    /// The SQL Server job store maintains all job state in a SQL Server database.
    /// </summary>
    [Singleton]
    public class SqlServerJobStore : AdoNetJobStore
    {
        /// <summary>
        /// Creates a SQL Server job store connected to a database.
        /// </summary>
        /// <param name="connectionString">The database connection string</param>
        /// <exception cref="ArgumentNullException">Thrown if <paramref name="connectionString"/> is null</exception>
        public SqlServerJobStore(string connectionString)
            : base(connectionString, "@")
        {
        }

        protected override IDbConnection CreateConnection()
        {
            return new SqlConnection(ConnectionString);
        }
    }
}