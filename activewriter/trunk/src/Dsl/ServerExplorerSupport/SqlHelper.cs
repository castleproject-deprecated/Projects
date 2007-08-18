// Copyright 2006 Gokhan Altinoren - http://altinoren.com/
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

namespace Altinoren.ActiveWriter.ServerExplorerSupport
{
	using System;
	using System.Collections.Generic;
	using System.Data;

	internal class SqlHelper : IDbHelper
	{
		private IDbConnection _connection = null;

		public SqlHelper(IDbConnection connection)
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
						column.Computed = reader["IS_COMPUTED"].ToString() == "1" ? true : false;

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
				case "BIT":
					return NHibernateType.Boolean;
				case "DECIMAL":
				case "NUMERIC":
					return NHibernateType.Decimal;
				case "MONEY":
					// TODO  DbType.Currency?
					break;
				case "SMALLMONEY":
					// TODO:
					break;
				case "FLOAT":
					return NHibernateType.Double; // TODO: synonym DOUBLE PRECISION ?
				case "REAL":
					return NHibernateType.Single;
				case "DATETIME":
					return NHibernateType.Timestamp; // TODO?
				case "SMALLDATETIME":
					return NHibernateType.DateTime;
				case "CHAR":
					return NHibernateType.AnsiChar; // TODO
				case "VARCHAR":
					return NHibernateType.String; // TODO
				case "TEXT":
					return NHibernateType.StringClob;
				case "NCHAR":
					return NHibernateType.Char; // TODO
				case "NVARCHAR":
					return NHibernateType.String; // TODO
				case "NTEXT":
					return NHibernateType.StringClob; // TODO
				case "BINARY":
					return NHibernateType.Binary;
				case "IMAGE":
					return NHibernateType.Binary;
				case "VARBINARY":
					return NHibernateType.Binary;
				case "SQL_VARIANT":
					// TODO return NHibernateType.Serializable;
					break;
				case "TIMESTAMP":
					return NHibernateType.Timestamp;
				case "UNIQUEIDENTIFIER":
					return NHibernateType.Guid;
				case "XML":
					return NHibernateType.String;
			}

			return NHibernateType.String; // TODO:
		}

		public List<Relation> GetFKRelations(ModelClass cls)
		{
			List<Relation> list = new List<Relation>();

			IDbCommand command = GetForeginKeyCommand(null, null, cls.Table, cls.Schema);
			using(IDataReader reader = command.ExecuteReader())
			{
				while(reader.Read())
				{
					Relation relation = new Relation();
					relation.RelationType = RelationType.Unknown; // Caller will decide 
					relation.RelationName = reader["FK_NAME"].ToString();
					relation.PrimaryOwner = reader["PKTABLE_OWNER"].ToString();
					relation.PrimaryTable = reader["PKTABLE_NAME"].ToString();
					relation.PrimaryColumn = reader["PKCOLUMN_NAME"].ToString();
					relation.ForeignOwner = cls.Schema;
					relation.ForeignTable = cls.Table;
					relation.ForeignColumn = reader["FKCOLUMN_NAME"].ToString();

					list.Add(relation);
				}
			}

			return list;
		}

		public List<Relation> GetPKRelations(ModelClass cls)
		{
			List<Relation> list = new List<Relation>();

			IDbCommand command = GetForeginKeyCommand(cls.Table, cls.Schema, null, null);
			using(IDataReader reader = command.ExecuteReader())
			{
				while(reader.Read())
				{
					Relation relation = new Relation();
					relation.RelationType = RelationType.Unknown; // Caller will decide
					relation.RelationName = reader["FK_NAME"].ToString();
					relation.PrimaryOwner = cls.Schema;
					relation.PrimaryTable = cls.Table;
					relation.PrimaryColumn = reader["PKCOLUMN_NAME"].ToString();
					relation.ForeignOwner = reader["FKTABLE_OWNER"].ToString();
					relation.ForeignTable = reader["FKTABLE_NAME"].ToString();
					relation.ForeignColumn = reader["FKCOLUMN_NAME"].ToString();

					list.Add(relation);
				}
			}

			return list;
		}

		#endregion

		private IDbCommand GetForeginKeyCommand(string pkTable, string pkOwner, string fkTable, string fkOwner)
		{
			IDbCommand command = _connection.CreateCommand();
			command.CommandText = "sp_fkeys";
			command.CommandType = CommandType.StoredProcedure;

			IDbDataParameter param1 = command.CreateParameter();
			param1.DbType = DbType.String;
			param1.ParameterName = "@pktable_name";
			param1.Size = 128;
			param1.Value = pkTable;
			command.Parameters.Add(param1);

			IDbDataParameter param2 = command.CreateParameter();
			param2.DbType = DbType.String;
			param2.ParameterName = "@pktable_owner";
			param2.Size = 128;
			param2.Value = pkOwner;
			command.Parameters.Add(param2);

			IDbDataParameter param3 = command.CreateParameter();
			param3.DbType = DbType.String;
			param3.ParameterName = "@fktable_name";
			param3.Size = 128;
			param3.Value = fkTable;
			command.Parameters.Add(param3);

			IDbDataParameter param4 = command.CreateParameter();
			param4.DbType = DbType.String;
			param4.ParameterName = "@fktable_owner";
			param4.Size = 128;
			param4.Value = fkOwner;
			command.Parameters.Add(param4);

			return command;
		}

		private IDbCommand GetColumnCommand(string table, string owner)
		{
			IDbCommand command = _connection.CreateCommand();
			command.CommandText =
				@"SELECT c.TABLE_CATALOG, c.TABLE_SCHEMA, c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE, c.IS_NULLABLE, tc.CONSTRAINT_TYPE, tc.CONSTRAINT_NAME,
                COLUMNPROPERTY(OBJECT_ID('[' + c.TABLE_SCHEMA + '].[' + c.TABLE_NAME + ']'), c.COLUMN_NAME, 'IsIdentity') AS [IS_IDENTITY],
                COLUMNPROPERTY(OBJECT_ID('[' + c.TABLE_SCHEMA + '].[' + c.TABLE_NAME + ']'), c.COLUMN_NAME, 'IsComputed') AS [IS_COMPUTED]
                FROM INFORMATION_SCHEMA.COLUMNS c
                LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                ON c.TABLE_CATALOG = kcu.TABLE_CATALOG AND c.TABLE_SCHEMA = kcu.TABLE_SCHEMA AND c.TABLE_NAME = kcu.TABLE_NAME AND c.COLUMN_NAME = kcu.COLUMN_NAME
                LEFT OUTER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON kcu.CONSTRAINT_NAME=tc.CONSTRAINT_NAME AND kcu.CONSTRAINT_SCHEMA=tc.CONSTRAINT_SCHEMA
                WHERE c.TABLE_SCHEMA = @schema AND c.TABLE_NAME = @table";

			IDbDataParameter param1 = command.CreateParameter();
			param1.DbType = DbType.String;
			param1.ParameterName = "@schema";
			param1.Size = 128;
			param1.Value = owner;
			command.Parameters.Add(param1);

			IDbDataParameter param2 = command.CreateParameter();
			param2.DbType = DbType.String;
			param2.ParameterName = "@table";
			param2.Size = 128;
			param2.Value = table;
			command.Parameters.Add(param2);

			return command;
		}
	}
}