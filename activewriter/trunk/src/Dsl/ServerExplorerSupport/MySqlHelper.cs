// Copyright 2004-2010 Castle Project - http://www.castleproject.org/
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

namespace Castle.ActiveWriter.ServerExplorerSupport
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	internal class MySqlHelper : IDbHelper
	{
		private IDbConnection _connection = null;

		public MySqlHelper(IDbConnection connection)
		{
			Connection = connection;
		}

		#region IDbHelper Members

		public IDbConnection Connection
		{
			get { return _connection; }
			set { _connection = value; }
		}

		public List<Column> GetProperties(ModelClass cls)
		{
			List<Column> list = new List<Column>();

			IDbCommand command = GetColumnCommand(cls.Table, cls.Schema);
			using(IDataReader reader = command.ExecuteReader())
			{
				while(reader.Read())
				{
					Column column;

					column = list.Find(delegate(Column col) { return col.Name == reader["COLUMN_NAME"].ToString(); }
						);

					if (column == null)
					{
						column = new Column();
						column.Name = reader["COLUMN_NAME"].ToString();
						column.Schema = cls.Schema;
						column.Table = cls.Table;
						column.DataType = reader["DATA_TYPE"].ToString();

						if (reader["CONSTRAINT_TYPE"] != DBNull.Value)
						{
							switch(reader["CONSTRAINT_TYPE"].ToString())
							{
								case "PRIMARY KEY":
									column.Primary = true;
									column.PrimaryConstraintName = reader["CONSTRAINT_NAME"].ToString();
									break;
								case "FOREIGN KEY":
									column.ForeignConstraints.Add(reader["CONSTRAINT_NAME"].ToString());
									break;
									// Check constraints not supported right now.
							}
						}

						column.Nullable = reader["IS_NULLABLE"].ToString() == "NO" ? false : true;
						column.Identity = reader["IS_IDENTITY"].ToString() == "1" ? true : false;

						list.Add(column);
					}
				}
			}

			return list;
		}

		public NHibernateType GetNHibernateType(string type)
		{
			switch(type.ToUpperInvariant())
			{
				case "BIGINT":
					return NHibernateType.Int64;
				case "INT":
					return NHibernateType.Int32;
				case "SMALLINT":
					return NHibernateType.Int16;
				case "TINYINT":
					return NHibernateType.Byte;
				case "DECIMAL":
				case "NUMERIC":
					return NHibernateType.Decimal;
				case "FLOAT":
					return NHibernateType.Single;
				case "DOUBLE":
				case "REAL":
					return NHibernateType.Double;
				case "DATETIME":
					return NHibernateType.DateTime;
				case "CHAR":
				case "VARCHAR":
					return NHibernateType.String;
				case "TINYTEXT":
				case "TEXT":
				case "MEDIUMTEXT":
				case "LONGTEXT":
					return NHibernateType.StringClob;
				case "BINARY":
				case "VARBINARY":
					return NHibernateType.Binary;
				case "TINYBLOB":
				case "BLOB":
				case "MEDIUMBLOB":
				case "LONGBLOB":
					return NHibernateType.BinaryBlob;
				case "TIMESTAMP":
					return NHibernateType.Timestamp;
			}

			return NHibernateType.String;
		}

		public List<Relation> GetFKRelations(ModelClass cls)
		{
			List<Relation> list = new List<Relation>();

			IDbCommand command = _connection.CreateCommand();
			command.CommandText =
				@"select 
                    kcu.CONSTRAINT_NAME,
                    kcu.TABLE_SCHEMA,
                    kcu.TABLE_NAME,
                    kcu.COLUMN_NAME,
                    kcu.REFERENCED_TABLE_SCHEMA,
                    kcu.REFERENCED_TABLE_NAME,
                    kcu.REFERENCED_COLUMN_NAME
                from
                    INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                where
                    kcu.CONSTRAINT_SCHEMA = ?schema 
                    and kcu.TABLE_NAME = ?table
                    and kcu.REFERENCED_TABLE_SCHEMA is not null
                    and kcu.REFERENCED_TABLE_NAME is not null
                    and kcu.REFERENCED_COLUMN_NAME is not null";

			AddSchemaAndTableParams(command, cls.Schema, cls.Table);

			using(IDataReader reader = command.ExecuteReader())
			{
				while(reader.Read())
				{
				    list.Add(new Relation
				                 {
				                     RelationType = RelationType.Unknown,
				                     RelationName = reader["CONSTRAINT_NAME"].ToString(),
				                     PrimaryOwner = reader["REFERENCED_TABLE_SCHEMA"].ToString(),
				                     PrimaryTable = reader["REFERENCED_TABLE_NAME"].ToString(),
				                     PrimaryColumn = reader["REFERENCED_COLUMN_NAME"].ToString(),
				                     ForeignOwner = cls.Schema,
				                     ForeignTable = cls.Table,
				                     ForeignColumn = reader["COLUMN_NAME"].ToString()
				                 });
				}
			}
			return list;
		}

		public List<Relation> GetPKRelations(ModelClass cls)
		{
			List<Relation> list = new List<Relation>();

			IDbCommand command = _connection.CreateCommand();
			command.CommandText =
				@"select 
                    kcu.CONSTRAINT_NAME,
                    kcu.TABLE_SCHEMA,
                    kcu.TABLE_NAME,
                    kcu.COLUMN_NAME,
                    kcu.REFERENCED_TABLE_SCHEMA,
                    kcu.REFERENCED_TABLE_NAME,
                    kcu.REFERENCED_COLUMN_NAME
                from
                    INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                where
                    kcu.CONSTRAINT_SCHEMA is not null
                    and kcu.TABLE_NAME is not null
                    and kcu.COLUMN_NAME is not NULL
                    and kcu.REFERENCED_TABLE_SCHEMA = ?schema
                    and kcu.REFERENCED_TABLE_NAME = ?table";

			AddSchemaAndTableParams(command, cls.Schema, cls.Table);

			using(IDataReader reader = command.ExecuteReader())
			{
				while(reader.Read())
				{
				    list.Add(new Relation
				                 {
				                     RelationType = RelationType.Unknown,
				                     RelationName = reader["CONSTRAINT_NAME"].ToString(),
				                     PrimaryOwner = cls.Schema,
				                     PrimaryTable = cls.Table,
				                     PrimaryColumn = reader["REFERENCED_COLUMN_NAME"].ToString(),
				                     ForeignOwner = reader["TABLE_SCHEMA"].ToString(),
				                     ForeignTable = reader["TABLE_NAME"].ToString(),
				                     ForeignColumn = reader["COLUMN_NAME"].ToString()
				                 });
				}
			}
			return list;
		}

		#endregion

		private IDbCommand GetColumnCommand(string table, string owner)
		{
			IDbCommand command = _connection.CreateCommand();
			command.CommandText =
				@"SELECT DISTINCT
                    c.TABLE_SCHEMA, 
                    c.TABLE_NAME, 
                    c.COLUMN_NAME, 
                    c.DATA_TYPE, 
                    c.IS_NULLABLE,
                    case when c.EXTRA like '%auto_increment%' then 1 else 0 end as IS_IDENTITY,
                    tc.CONSTRAINT_TYPE,
                    tc.CONSTRAINT_NAME
                FROM 
                    INFORMATION_SCHEMA.COLUMNS c
                    LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                         ON c.TABLE_SCHEMA = kcu.TABLE_SCHEMA AND c.TABLE_NAME = kcu.TABLE_NAME AND c.COLUMN_NAME = kcu.COLUMN_NAME
                    LEFT OUTER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc 
                         ON kcu.CONSTRAINT_NAME=tc.CONSTRAINT_NAME AND kcu.CONSTRAINT_SCHEMA=tc.CONSTRAINT_SCHEMA
                WHERE 
                    c.TABLE_SCHEMA = ?schema AND c.TABLE_NAME = ?table";

			AddSchemaAndTableParams(command, owner, table);

			return command;
		}

		private void AddSchemaAndTableParams(IDbCommand command, string owner, string table)
		{
			IDbDataParameter param1 = command.CreateParameter();
			param1.DbType = DbType.String;
			param1.ParameterName = "?schema";
			param1.Size = 128;
			param1.Value = owner;
			command.Parameters.Add(param1);

			IDbDataParameter param2 = command.CreateParameter();
			param2.DbType = DbType.String;
			param2.ParameterName = "?table";
			param2.Size = 128;
			param2.Value = table;
			command.Parameters.Add(param2);
		}
	}
}