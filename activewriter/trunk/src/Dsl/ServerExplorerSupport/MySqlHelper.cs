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

using MySql.Data.MySqlClient;

namespace Altinoren.ActiveWriter.ServerExplorerSupport
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;

    internal class MySqlHelper : IDbHelper
    {
        private MySqlConnection _connection = null;

        public MySqlHelper(MySqlConnection connection)
        {
            Connection = connection;
        }

        public IDbConnection Connection
        {
            get { return _connection; }
            set { _connection = (MySqlConnection)value; }
        }

        public List<Column> GetProperties(ModelClass cls)
        {
            List<Column> list = new List<Column>();

            MySqlCommand command = GetColumnComand(cls.Table, cls.Schema);
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
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

            MySqlCommand command = GetCommand();
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
            command.Parameters.Add("?schema", MySqlDbType.VarChar, 128).Value = cls.Schema;
            command.Parameters.Add("?table", MySqlDbType.VarChar, 128).Value = cls.Table;
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataSet data = new DataSet();
            adapter.Fill(data);

            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    Relation relation = new Relation();
                    relation.RelationType = RelationType.Unknown;
                    relation.RelationName = row["CONSTRAINT_NAME"].ToString();
                    relation.PrimaryOwner = row["REFERENCED_TABLE_SCHEMA"].ToString();
                    relation.PrimaryTable = row["REFERENCED_TABLE_NAME"].ToString();
                    relation.PrimaryColumn = row["REFERENCED_COLUMN_NAME"].ToString();
                    relation.ForeignOwner = cls.Schema;
                    relation.ForeignTable = cls.Table;
                    relation.ForeignColumn = row["COLUMN_NAME"].ToString();

                    list.Add(relation);
                }

            return list;
        }

        public List<Relation> GetPKRelations(ModelClass cls)
        {
            List<Relation> list = new List<Relation>();

            MySqlCommand command = GetCommand();
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
            command.Parameters.Add("?schema", MySqlDbType.VarChar, 128).Value = cls.Schema;
            command.Parameters.Add("?table", MySqlDbType.VarChar, 128).Value = cls.Table;
            MySqlDataAdapter adapter = new MySqlDataAdapter(command);
            DataSet data = new DataSet();
            adapter.Fill(data);

            if (data.Tables.Count > 0 && data.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in data.Tables[0].Rows)
                {
                    Relation relation = new Relation();
                    relation.RelationType = RelationType.Unknown;
                    relation.RelationName = row["CONSTRAINT_NAME"].ToString();
                    relation.PrimaryOwner = cls.Schema;
                    relation.PrimaryTable = cls.Table;
                    relation.PrimaryColumn = row["REFERENCED_COLUMN_NAME"].ToString();
                    relation.ForeignOwner = row["TABLE_SCHEMA"].ToString();
                    relation.ForeignTable = row["TABLE_NAME"].ToString();
                    relation.ForeignColumn = row["COLUMN_NAME"].ToString();

                    list.Add(relation);
                }
            }

            return list;
        }

        private MySqlCommand GetColumnComand(string table, string owner)
        {
            MySqlCommand command = GetCommand();
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
            command.Parameters.Add("?schema", MySqlDbType.VarChar, 128).Value = owner;
            command.Parameters.Add("?table", MySqlDbType.VarChar, 128).Value = table;

            return command;
        }

        private MySqlCommand GetCommand()
        {
            MySqlCommand command = new MySqlCommand();
            command.Connection = _connection;

            return command;
        }
    }
}
