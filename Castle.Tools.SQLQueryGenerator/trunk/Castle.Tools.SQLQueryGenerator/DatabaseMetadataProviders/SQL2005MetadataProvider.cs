#region license
// Copyright 2008 Ken Egozi http://www.kenegozi.com/blog
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
#endregion

namespace Castle.Tools.SQLQueryGenerator.DatabaseMetadataProviders
{
	using System.Data;
	using System.Data.SqlClient;
	using System.Collections.Generic;

	public class SQL2005MetadataProvider : IDatabaseMetadataProvider
	{
		#region SQL
		const string SQL = @"
SELECT
	schemas.name AS [Schema],
	tables.name AS [Table],
	columns.name AS [Column],
	CASE 
		WHEN columns.system_type_id = 34	THEN 'System.Byte[]'
		WHEN columns.system_type_id = 35	THEN 'System.String'
		WHEN columns.system_type_id = 36	THEN 'System.Guid'
		WHEN columns.system_type_id = 48	THEN 'System.Byte'
		WHEN columns.system_type_id = 52	THEN 'System.Int16'
		WHEN columns.system_type_id = 56	THEN 'System.Int32'
		WHEN columns.system_type_id = 58	THEN 'System.DateTime'
		WHEN columns.system_type_id = 59	THEN 'System.Double'
		WHEN columns.system_type_id = 60	THEN 'System.Decimal'
		WHEN columns.system_type_id = 61	THEN 'System.DateTime'
		WHEN columns.system_type_id = 62	THEN 'System.Double'
		WHEN columns.system_type_id = 98	THEN 'System.Object'
		WHEN columns.system_type_id = 99	THEN 'System.String'
		WHEN columns.system_type_id = 104	THEN 'System.Boolean'
		WHEN columns.system_type_id = 106	THEN 'System.Decimal'
		WHEN columns.system_type_id = 108	THEN 'System.Decimal'
		WHEN columns.system_type_id = 122	THEN 'System.Decimal'
		WHEN columns.system_type_id = 127	THEN 'System.Int64'
		WHEN columns.system_type_id = 165	THEN 'System.Byte[]'
		WHEN columns.system_type_id = 167	THEN 'System.String'
		WHEN columns.system_type_id = 173	THEN 'System.Byte[]'
		WHEN columns.system_type_id = 175	THEN 'System.String'
		WHEN columns.system_type_id = 189	THEN 'System.Int64'
		WHEN columns.system_type_id = 231	THEN 'System.String'
		WHEN columns.system_type_id = 239	THEN 'System.String'
		WHEN columns.system_type_id = 241	THEN 'System.String'
		WHEN columns.system_type_id = 241	THEN 'System.String'
	END AS [Type],
	columns.is_nullable AS [IsNullable]

FROM
				sys.tables tables
	INNER JOIN	sys.schemas schemas ON (tables.schema_id = schemas.schema_id )
	INNER JOIN	sys.columns columns ON (columns.object_id = tables.object_id)

WHERE
				tables.name <> 'sysdiagrams' 
	AND			tables.name <> 'dtproperties'

ORDER BY [Schema], [Table], [Column], [Type]";
		#endregion

		readonly string connectionString;

		public SQL2005MetadataProvider(string connectionString)
		{
			this.connectionString = connectionString;
		}

		public SQL2005MetadataProvider(string server, string dbName, string userId, string password)
		{
			connectionString = string.Format(
				"Server={0};Initial Catalog={1};User ID={2};Password={3};",
				server, dbName, userId, password);
		}

		public SQL2005MetadataProvider(string server, string dbName)
		{
			connectionString = string.Format(
				"Server={0};Initial Catalog={1};Integrated Security=SSPI;",
				server, dbName);
		}

		public IEnumerable<DbPropertyMetadata> ExtractMetadata()
		{
			List<DbPropertyMetadata> properties = new List<DbPropertyMetadata>();

			using (SqlConnection con = new SqlConnection(connectionString))
			using (IDbCommand cmd = new SqlCommand(SQL, con))
			{
				con.Open();
				using (IDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
						properties.Add(new DbPropertyMetadata(
							reader.GetString(0),
							reader.GetString(1),
							reader.GetString(2),
							reader.GetString(3),
							reader.GetBoolean(4)));
				}
			}

			return properties;
		}
	}
}