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
            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
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
                        if (column.DataType == "NUMBER")
                        {
                            string scale = reader["DATA_SCALE"].ToString();
                            if (scale == "0" || string.IsNullOrEmpty(scale))
                            {
                                column.DataType = "INTEGER";
                                // If field is primary key and "integer" it is reasonably safe to assume that it is has
                                // a sequence behind it. Confirming that would in Oracle require trigger parsing
                                column.Identity = reader["PKEY"] != DBNull.Value ? true : false;
                            }
                        }

                        if (reader["PKEY"] != DBNull.Value)
                        {
                            column.Primary = true;
                            column.PrimaryConstraintName = reader["PKEY"].ToString();
                        }

                        if (reader["FKEY"] != DBNull.Value)
                        {
                            column.ForeignConstraints.Add(reader["FKEY"].ToString());
                        }

                        column.Nullable = reader["NULLABLE"].ToString() == "N" ? false : true;
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

        // Konèano
        public List<Relation> GetFKRelations(ModelClass cls)
        {
            List<Relation> list = new List<Relation>();

            IDbCommand command = _connection.CreateCommand();
            command.CommandText =
                @"
SELECT st.column_name, om.r_owner, om.constraint_name, rm.table_name r_table_name, sr.column_name r_column_name
  FROM all_constraints om INNER JOIN all_constraints rm ON om.r_owner = rm.owner
                                                      AND om.r_constraint_name = rm.constraint_name
       INNER JOIN all_cons_columns st ON om.constraint_name = st.constraint_name
                                    AND st.owner = om.owner
                                    AND st.table_name = om.table_name
       INNER JOIN all_cons_columns sr ON rm.constraint_name = sr.constraint_name
                                    AND sr.owner = rm.owner
                                    AND sr.table_name = rm.table_name
 WHERE om.constraint_type = 'R'
   AND st.table_name = :tname
   AND st.owner = :owner
                ";

            AddSchemaAndTableParams(command, cls.Schema, cls.Table);

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Relation relation = new Relation();
                    relation.RelationType = RelationType.Unknown;
                    relation.RelationName = reader["CONSTRAINT_NAME"].ToString();
                    relation.PrimaryOwner = reader["R_OWNER"].ToString();
                    relation.PrimaryTable = reader["R_TABLE_NAME"].ToString();
                    relation.PrimaryColumn = reader["R_COLUMN_NAME"].ToString();
                    relation.ForeignOwner = cls.Schema;
                    relation.ForeignTable = cls.Table;
                    relation.ForeignColumn = reader["COLUMN_NAME"].ToString();

                    list.Add(relation);
                }
            }

            return list;

        }

        public List<Relation> GetPKRelations(ModelClass cls)
        {
            List<Relation> list = new List<Relation>();

            IDbCommand command = _connection.CreateCommand();
            command.CommandText =
                @"
            SELECT st.column_name, om.owner, om.constraint_name, om.table_name, sr.column_name r_column_name
              FROM all_constraints om INNER JOIN all_constraints rm ON om.r_owner = rm.owner
                                                                  AND om.r_constraint_name = rm.constraint_name
                   INNER JOIN all_cons_columns st ON om.constraint_name = st.constraint_name
                                                AND st.owner = om.owner
                                                AND st.table_name = om.table_name
                   INNER JOIN all_cons_columns sr ON rm.constraint_name = sr.constraint_name
                                                AND sr.owner = rm.owner
                                                AND sr.table_name = rm.table_name
             WHERE om.constraint_type = 'R'
               AND rm.table_name = :tname
               AND om.r_owner = :owner
                ";

            AddSchemaAndTableParams(command, cls.Schema, cls.Table);

            using (IDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Relation relation = new Relation();
                    relation.RelationType = RelationType.Unknown;
                    relation.RelationName = reader["CONSTRAINT_NAME"].ToString();
                    relation.PrimaryOwner = cls.Schema;
                    relation.PrimaryTable = cls.Table;
                    relation.PrimaryColumn = reader["R_COLUMN_NAME"].ToString();
                    relation.ForeignOwner = reader["OWNER"].ToString();
                    relation.ForeignTable = reader["TABLE_NAME"].ToString();
                    relation.ForeignColumn = reader["COLUMN_NAME"].ToString();

                    list.Add(relation);
                }
            }

            return list;

        }

        #endregion

        private IDbCommand GetColumnCommand(string table, string owner)
        {
            IDbCommand command = _connection.CreateCommand();
            // "group by" in commandtext is in there just becouse the test runs 20x faster with it
            command.CommandText =
                @"
SELECT owner, table_name, tcols.column_name, data_type, data_length, data_precision, data_scale, nullable, pkey, fkey
  FROM (SELECT owner, table_name, column_name, data_type, data_length, data_precision, data_scale, nullable
          FROM all_tab_columns
         WHERE table_name = :tname
           AND owner = :owner) tcols
       LEFT JOIN
       (SELECT   column_name, MAX (DECODE (constraint_type, 'P', om.constraint_name, NULL)) pkey,
                 MAX (DECODE (constraint_type, 'R', om.constraint_name, NULL)) fkey
            FROM all_constraints om INNER JOIN all_cons_columns st
                 ON om.constraint_name = st.constraint_name
               AND st.owner = om.owner
               AND st.table_name = om.table_name
           WHERE (   constraint_type = 'P'
                  OR constraint_type = 'R')
             AND st.table_name = :tname
             AND st.owner = :owner
        GROUP BY column_name) tcons ON tcols.column_name = tcons.column_name 
                ";

            AddSchemaAndTableParams(command, owner, table);

            return command;
        }

        private void AddSchemaAndTableParams(IDbCommand command, string owner, string table)
        {
            IDbDataParameter param1 = command.CreateParameter();
            param1.DbType = DbType.String;
            param1.ParameterName = ":owner";
            param1.Size = 128;
            param1.Value = owner;
            command.Parameters.Add(param1);

            IDbDataParameter param2 = command.CreateParameter();
            param2.DbType = DbType.String;
            param2.ParameterName = ":tname";
            param2.Size = 128;
            param2.Value = table;
            command.Parameters.Add(param2);
        }
    }
}
