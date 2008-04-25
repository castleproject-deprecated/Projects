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
        private List<string> _foreignConstraints = new List<string>();

        public string Schema { get; set; }
        public string Table { get; set; }
        public string Name { get; set; }
        public string DataType { get; set; }
        public string PrimaryConstraintName { get; set; }
        public bool Nullable { get; set; }
        public bool Identity { get; set; }
        public bool Computed { get; set; }
        public bool Primary { get; set; }

        public List<string> ForeignConstraints
        {
            get { return _foreignConstraints; }
            set { _foreignConstraints = value; }
        }

        public static List<Column> FindPrimaryKeys(List<Column> columns)
        {
            return columns.FindAll(column => (column.Primary));
        }

        public static Column FindColumn(List<Column> columns, string name)
        {
            return columns.Find(column => (column.Name == name));
        }
    }

    internal class ColumnComparer : IComparer<Column>
    {
        public int Compare(Column x, Column y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }

    internal class Relation
    {
        private RelationType _type = RelationType.Unknown;

        public string PrimaryColumn { get; set; }
        public string PrimaryTable { get; set; }
        public string ForeignColumn { get; set; }
        public string ForeignTable { get; set; }
        public string PrimaryOwner { get; set; }
        public string ForeignOwner { get; set; }
        public string RelationName { get; set; }
        public ModelClass PrimaryModelClass { get; set; }
        public ModelClass ForeignModelClass { get; set; }
        public bool IsForeignColumnPrimary { get; set; }

        public RelationType RelationType
        {
            get { return _type; }
            set { _type = value; }
        }

        public static int GetCountOfMatchingRelations(List<Relation> relations, List<string> relationNames)
        {
            int count = 0;
            foreach (string name in relationNames)
            {
                count += relations.FindAll(relation => (relation.RelationName == name)).Count;
            }
            return count;
        }

        public static Relation GetMatchingRelation(List<Relation> relations, Relation relation)
        {
            return relations.Find(
                rel => (rel.RelationName == relation.RelationName &&
                        rel.PrimaryModelClass != relation.PrimaryModelClass &&
                        rel.PrimaryColumn == relation.PrimaryColumn &&
                        rel.ForeignColumn == relation.ForeignColumn &&
                        rel.PrimaryOwner == relation.PrimaryOwner &&
                        rel.PrimaryTable == relation.PrimaryTable &&
                        rel.ForeignOwner == relation.ForeignOwner &&
                        rel.ForeignTable == relation.ForeignTable)
                );
        }

        public static List<Relation> GetFKRelationsFromSameClassToDifferentClasses(List<Relation> relations, Relation relation)
        {
            return relations.FindAll(
                rel => (rel.ForeignOwner == relation.ForeignOwner &&
                        rel.ForeignTable == relation.ForeignTable &&
                        !(rel.PrimaryOwner == relation.PrimaryOwner && rel.PrimaryTable == relation.PrimaryTable)
                       )
                );
        }
        
        public static Relation GetForeginColumn(List<Relation> relations, Column key)
        {
            return relations.Find(
                rel => (rel.ForeignOwner == key.Schema &&
                        rel.ForeignTable == key.Table &&
                        rel.ForeignColumn == key.Name)
                );
        }
    }
}