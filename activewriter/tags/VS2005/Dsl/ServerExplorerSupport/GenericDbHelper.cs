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

using System;

namespace Altinoren.ActiveWriter.ServerExplorerSupport
{
    using System.Collections.Generic;
    
    public enum RelationType
    {
        Unknown,
        ManyToOne,
        OneToOne,
        ManyToMany
    }

    public enum ConstraintType
    {
        None,
        PrimaryKey,
        ForeignKey,
        Check
    }

    internal class Column
    {
        private string _schema = null;
        private string _table = null;
        private string _name = null;
        private string _dataType = null;
        private bool _primary = false;
        private string _primaryConstraintName = null;
        private List<string> _foreignConstraints = new List<string>();
        private bool _nullable = false;
        private bool _identity = false;
        private bool _computed = false;

        public string Schema
        {
            get { return _schema; }
            set { _schema = value; }
        }

        public string Table
        {
            get { return _table; }
            set { _table = value; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public string DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public bool Nullable
        {
            get { return _nullable; }
            set { _nullable = value; }
        }

        public bool Identity
        {
            get { return _identity; }
            set { _identity = value; }
        }

        public bool Computed
        {
            get { return _computed; }
            set { _computed = value; }
        }

        public bool Primary
        {
            get { return _primary; }
            set { _primary = value; }
        }

        public string PrimaryConstraintName
        {
            get { return _primaryConstraintName; }
            set { _primaryConstraintName = value; }
        }

        public List<string> ForeignConstraints
        {
            get { return _foreignConstraints; }
            set { _foreignConstraints = value; }
        }

        public static List<Column> FindPrimaryKeys(List<Column> columns)
        {
            return columns.FindAll(
                delegate(Column column)
                    {
                        return (column.Primary);
                    }
                );
        }

        public static Column FindColumn(List<Column> columns, string name)
        {
            return columns.Find(
                delegate(Column column)
                    {
                        return (column.Name == name);
                    }
                );
        }
    }

    internal class Relation
    {
        private string _primaryColumn = null;
        private string _primaryTable = null;
        private string _primaryOwner = null;
        private string _foreignColumn = null;
        private string _foreignTable = null;
        private string _foreignOwner = null;
        private bool _isForeignColumnPrimary = false;
        private RelationType _type = RelationType.Unknown;
        private string _relationName = null;
        private ModelClass _primaryModelClass = null;
        private ModelClass _foreignModelClass = null;

        public string PrimaryColumn
        {
            get { return _primaryColumn; }
            set { _primaryColumn = value; }
        }

        public string PrimaryTable
        {
            get { return _primaryTable; }
            set { _primaryTable = value; }
        }

        public string ForeignColumn
        {
            get { return _foreignColumn; }
            set { _foreignColumn = value; }
        }

        public string ForeignTable
        {
            get { return _foreignTable; }
            set { _foreignTable = value; }
        }

        public RelationType RelationType
        {
            get { return _type; }
            set { _type = value; }
        }

        public string PrimaryOwner
        {
            get { return _primaryOwner; }
            set { _primaryOwner = value; }
        }

        public string ForeignOwner
        {
            get { return _foreignOwner; }
            set { _foreignOwner = value; }
        }

        public string RelationName
        {
            get { return _relationName; }
            set { _relationName = value; }
        }

        public ModelClass PrimaryModelClass
        {
            get { return _primaryModelClass; }
            set { _primaryModelClass = value; }
        }

        public ModelClass ForeignModelClass
        {
            get { return _foreignModelClass; }
            set { _foreignModelClass = value; }
        }

        public bool IsForeignColumnPrimary
        {
            get { return _isForeignColumnPrimary; }
            set { _isForeignColumnPrimary = value; }
        }

        public static int GetCountOfMatchingRelations(List<Relation> relations, List<string> relationNames)
        {
            int count = 0;
            foreach (string name in relationNames)
            {
                count += relations.FindAll(
                    delegate(Relation relation)
                        {
                            return (relation.RelationName == name);
                        }
                    ).Count;
            }
            return count;
        }

        public static Relation GetMatchingRelation(List<Relation> relations, Relation relation)
        {
            return relations.Find(
                delegate(Relation rel)
                    {
                        return (rel.RelationName == relation.RelationName &&
                                rel.PrimaryModelClass != relation.PrimaryModelClass &&
                                rel.PrimaryColumn == relation.PrimaryColumn &&
                                rel.ForeignColumn == relation.ForeignColumn &&
                                rel.PrimaryOwner == relation.PrimaryOwner &&
                                rel.PrimaryTable == relation.PrimaryTable &&
                                rel.ForeignOwner == relation.ForeignOwner &&
                                rel.ForeignTable == relation.ForeignTable);
                    }
                );
        }

        public static List<Relation> GetFKRelationsFromSameClassToDifferentClasses(List<Relation> relations, Relation relation)
        {
            return relations.FindAll(
                delegate(Relation rel)
                    {
                        return (rel.ForeignOwner == relation.ForeignOwner &&
                                rel.ForeignTable == relation.ForeignTable &&
                                !(rel.PrimaryOwner == relation.PrimaryOwner && rel.PrimaryTable == relation.PrimaryTable)
                               );
                    }
                );
        }
        
        public static Relation GetForeginColumn(List<Relation> relations, Column key)
        {
            return relations.Find(
                delegate(Relation rel)
                    {
                        return (rel.ForeignOwner == key.Schema &&
                                rel.ForeignTable == key.Table &&
                                rel.ForeignColumn == key.Name);
                    }
                );
        }
    }
}