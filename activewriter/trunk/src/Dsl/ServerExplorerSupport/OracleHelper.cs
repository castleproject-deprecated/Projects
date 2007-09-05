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
    using System.Text;

	internal class OracleHelper : IDbHelper
	{

        private bool useCamelCase = true;
        public bool UseCamelCase
        {
            get { return useCamelCase; }
            set { useCamelCase = value; }
        }

		private IDbConnection _connection = null;

        public OracleHelper(IDbConnection connection)
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

					column = list.Find(delegate(Column col) { return col.Name == reader["CNAME"].ToString(); }
						);

					if (column == null)
					{
						column = new Column();
                        column.Name = reader["CNAME"].ToString();
                        
						column.Schema = cls.Schema;
						column.Table = cls.Table;


						column.DataType = reader["COLTYPE"].ToString();
                        if (column.DataType == "NUMBER")
                        {
                            string scale = reader["SCALE"].ToString();
                            if (scale == "0" || scale == "")
                            {
                                column.DataType = "INTEGER";
                            }
                        }

                        ///TODO Check for foreign, primary keys


                        column.Nullable = reader["NULLS"].ToString() == "NOT NULL" ? false : true;
						//column.Identity = reader["IS_IDENTITY"].ToString() == "1" ? true : false;
                        // Not sure what this means
                        column.Identity = false;
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
				case "NUMBER":
					return NHibernateType.Decimal;
                case "INTEGER":
                    return NHibernateType.Int32;
                case "DATE":
					return NHibernateType.DateTime;
				case "CHAR":
                case "NCHAR":
				case "VARCHAR":
                case "VARCHAR2":
                case "NVARCHAR2":
					return NHibernateType.String;
				case "LONG":
                case "CLOB":
                case "NCLOB":
					return NHibernateType.StringClob;
                case "RAW":
                case "LONG RAW":
					return NHibernateType.Binary;
                case "BLOB": 
                    return NHibernateType.BinaryBlob;
				case "TIMESTAMP":
					return NHibernateType.Timestamp;
			}

			return NHibernateType.String;
		}

		public List<Relation> GetFKRelations(ModelClass cls)
		{
			List<Relation> list = new List<Relation>();
            return list;

            /*

			IDbCommand command = _connection.CreateCommand();
			command.CommandText =
                @"select 
                    distinct b.column_name
                from
                    ALL_CONSTRAINTS a,
                    ALL_CONS_COLUMNS b

                where
                    UPPER(b.table_name) = (:table)
                    AND (UPPER(a.table_name) = (:table) and a.constraint_type = 'P')
                    AND (a.constraint_name = b.constraint_name)
                ";

			AddSchemaAndTableParams(command, cls.Schema, cls.Table);

			using(IDataReader reader = command.ExecuteReader())
			{
				while(reader.Read())
				{
					Relation relation = new Relation();
					relation.RelationType = RelationType.Unknown;
					relation.RelationName = reader["CONSTRAINT_NAME"].ToString();
					relation.PrimaryOwner = reader["REFERENCED_TABLE_SCHEMA"].ToString();
					relation.PrimaryTable = reader["REFERENCED_TABLE_NAME"].ToString();
					relation.PrimaryColumn = reader["REFERENCED_COLUMN_NAME"].ToString();
					relation.ForeignOwner = cls.Schema;
					relation.ForeignTable = cls.Table;
					relation.ForeignColumn = reader["COLUMN_NAME"].ToString();

					list.Add(relation);
				}
			}

			return list;
             * 
             */
		}

		public List<Relation> GetPKRelations(ModelClass cls)
		{
			List<Relation> list = new List<Relation>();
            return list;
            /*
			IDbCommand command = _connection.CreateCommand();
			command.CommandText =
                @"select 
                    a.*, b.COLUMN_NAME
                from
                    ALL_CONSTRAINTS a,
                    ALL_CONS_COLUMNS b

                where
                    UPPER(b.table_name) = (:tname)
                    AND (UPPER(a.table_name) = (:tname) and a.constraint_type = 'P')
                    AND (a.constraint_name = b.constraint_name)
                ";

			AddSchemaAndTableParams(command, cls.Schema, cls.Table);

			using(IDataReader reader = command.ExecuteReader())
			{
				while(reader.Read())
				{
					Relation relation = new Relation();
					relation.RelationType = RelationType.Unknown;
					relation.RelationName = reader["CONSTRAINT_NAME"].ToString();
					relation.PrimaryOwner = cls.Schema;
					relation.PrimaryTable = cls.Table;
                    relation.PrimaryColumn = reader["COLUMN_NAME"].ToString();
					relation.ForeignOwner = reader["OWNER"].ToString();
					relation.ForeignTable = reader["TABLE_NAME"].ToString();
					relation.ForeignColumn = reader["COLUMN_NAME"].ToString();

					list.Add(relation);
				}
			}

			return list;
             */
		}

		#endregion

		private IDbCommand GetColumnCommand(string table, string owner)
		{
			IDbCommand command = _connection.CreateCommand();
			command.CommandText =
                @"  select * from col
                    where tname = :tname
                    order by colno
                ";

			AddSchemaAndTableParams(command, owner, table);

			return command;
		}

		private void AddSchemaAndTableParams(IDbCommand command, string owner, string table)
		{
			//IDbDataParameter param1 = command.CreateParameter();
			//param1.DbType = DbType.String;
			//param1.ParameterName = "?schema";
			//param1.Size = 128;
			//param1.Value = owner;
			//command.Parameters.Add(param1);

			IDbDataParameter param2 = command.CreateParameter();
			param2.DbType = DbType.String;
			param2.ParameterName = ":tname";
			param2.Size = 128;
			param2.Value = table;
			command.Parameters.Add(param2);
		}
	}
}
