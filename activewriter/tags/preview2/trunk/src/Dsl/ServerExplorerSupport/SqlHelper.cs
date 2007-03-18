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
    using System.Data.Common;
    using System.Data.SqlClient;
    
    internal class SqlHelper : IDbHelper
    {
        private SqlConnection _connection = null;

        public SqlHelper(SqlConnection connection)
        {
            Connection = connection;
        }

        public DbConnection Connection
        {
            get { return _connection; }
            set { _connection = (SqlConnection) value; }
        }

        public List<Column> GetProperties(ModelClass cls)
        {
            List<Column> list = new List<Column>();

            SqlCommand command = GetColumnComand(cls.Table, cls.Schema);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet data = new DataSet();
            adapter.Fill(data);

            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    Column column;

                    column = list.Find(delegate(Column col)
                                           {
                                               return col.Name == row["COLUMN_NAME"].ToString();
                                           }
                        );

                    if (column == null)
                    {
                        column = new Column();
                        column.Name = row["COLUMN_NAME"].ToString();
                        column.Schema = cls.Schema;
                        column.Table = cls.Table;
                        column.DataType = row["DATA_TYPE"].ToString();
                        if (row["CONSTRAINT_TYPE"] != DBNull.Value)
                        {
                            switch (row["CONSTRAINT_TYPE"].ToString())
                            {
                                case "PRIMARY KEY":
                                    column.Primary = true;
                                    column.PrimaryConstraintName = row["CONSTRAINT_NAME"].ToString();
                                    break;
                                case "FOREIGN KEY":
                                    column.ForeignConstraints.Add(row["CONSTRAINT_NAME"].ToString());
                                    break;
                                    // Check constraints not supported right now.
                            }
                        }
                        column.Nullable = row["IS_NULLABLE"].ToString() == "NO" ? false : true;
                        column.Identity = row["IS_IDENTITY"].ToString() == "1" ? true : false;
                        column.Computed = row["IS_COMPUTED"].ToString() == "1" ? true : false;

                        list.Add(column);
                    }
                }
            }

            return list;
        }

        public NHibernateType GetNHibernateType(string type)
        {
            switch (type.ToUpperInvariant())
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

            SqlCommand command = GetForeginKeyCommand(null, null, cls.Table, cls.Schema);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet data = new DataSet();
            adapter.Fill(data);

            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    Relation relation = new Relation();
                    relation.RelationType = RelationType.Unknown; // Caller will decide 
                    relation.RelationName = row["FK_NAME"].ToString();
                    relation.PrimaryOwner = row["PKTABLE_OWNER"].ToString();
                    relation.PrimaryTable = row["PKTABLE_NAME"].ToString();
                    relation.PrimaryColumn = row["PKCOLUMN_NAME"].ToString();
                    relation.ForeignOwner = cls.Schema;
                    relation.ForeignTable = cls.Table;
                    relation.ForeignColumn = row["FKCOLUMN_NAME"].ToString();

                    list.Add(relation);
                }

            return list;
        }

        public List<Relation> GetPKRelations(ModelClass cls)
        {
            List<Relation> list = new List<Relation>();

            SqlCommand command = GetForeginKeyCommand(cls.Table, cls.Schema, null, null);
            SqlDataAdapter adapter = new SqlDataAdapter(command);
            DataSet data = new DataSet();
            adapter.Fill(data);

            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    Relation relation = new Relation();
                    relation.RelationType = RelationType.Unknown; // Caller will decide
                    relation.RelationName = row["FK_NAME"].ToString();
                    relation.PrimaryOwner = cls.Schema;
                    relation.PrimaryTable = cls.Table;
                    relation.PrimaryColumn = row["PKCOLUMN_NAME"].ToString();
                    relation.ForeignOwner = row["FKTABLE_OWNER"].ToString();
                    relation.ForeignTable = row["FKTABLE_NAME"].ToString();
                    relation.ForeignColumn = row["FKCOLUMN_NAME"].ToString();

                    list.Add(relation);
                }

            return list;
        }

        private SqlCommand GetForeginKeyCommand(string pkTable, string pkOwner, string fkTable, string fkOwner)
        {
            SqlCommand command = GetCommand();
            command.CommandText = "sp_fkeys";
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.Add("@pktable_name", SqlDbType.NVarChar, 128).Value = pkTable;
            command.Parameters.Add("@pktable_owner ", SqlDbType.NVarChar, 128).Value = pkOwner;
            command.Parameters.Add("@fktable_name", SqlDbType.NVarChar, 128).Value = fkTable;
            command.Parameters.Add("@fktable_owner", SqlDbType.NVarChar, 128).Value = fkOwner;

            return command;
        }

        private SqlCommand GetColumnComand(string table, string owner)
        {
            SqlCommand command = GetCommand();
            command.CommandText =
                @"SELECT c.TABLE_CATALOG, c.TABLE_SCHEMA, c.TABLE_NAME, c.COLUMN_NAME, c.DATA_TYPE, c.IS_NULLABLE, tc.CONSTRAINT_TYPE, tc.CONSTRAINT_NAME,
                COLUMNPROPERTY(OBJECT_ID('[' + c.TABLE_SCHEMA + '].[' + c.TABLE_NAME + ']'), c.COLUMN_NAME, 'IsIdentity') AS [IS_IDENTITY],
                COLUMNPROPERTY(OBJECT_ID('[' + c.TABLE_SCHEMA + '].[' + c.TABLE_NAME + ']'), c.COLUMN_NAME, 'IsComputed') AS [IS_COMPUTED]
                FROM INFORMATION_SCHEMA.COLUMNS c
                LEFT OUTER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
                ON c.TABLE_CATALOG = kcu.TABLE_CATALOG AND c.TABLE_SCHEMA = kcu.TABLE_SCHEMA AND c.TABLE_NAME = kcu.TABLE_NAME AND c.COLUMN_NAME = kcu.COLUMN_NAME
                LEFT OUTER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc ON kcu.CONSTRAINT_NAME=tc.CONSTRAINT_NAME AND kcu.CONSTRAINT_SCHEMA=tc.CONSTRAINT_SCHEMA
                WHERE c.TABLE_SCHEMA = @schema AND c.TABLE_NAME = @table";
            command.Parameters.Add("@schema", SqlDbType.NVarChar, 128).Value = owner;
            command.Parameters.Add("@table", SqlDbType.NVarChar, 128).Value = table;

            return command;
        }

        private SqlCommand GetCommand()
        {
            SqlCommand command = new SqlCommand();
            command.Connection = _connection;

            return command;
        }
    }
}