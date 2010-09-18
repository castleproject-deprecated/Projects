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
    using System.Collections.Generic;
    using Microsoft.VisualStudio.Modeling;
    using CodeGeneration;
    
    internal class DiagramManager
    {
        public Store Store { get; private set; }
        private Model _model = null;
        private List<DSRefNode> _tableList = null;
        private Dictionary<string, ModelClass> classes = new Dictionary<string, ModelClass>();
        private OutputWindowHelper _output;
        private bool? sortProperties;

        public DiagramManager(Store store, Model model)
        {
            Store = store;
            _model = model;
        }

        public List<DSRefNode> Tables
        {
            get { return _tableList; }
            set { _tableList = value; }
        }

        public OutputWindowHelper OutputWindow
        {
            get { return _output; }
            set { _output = value; }
        }

        public bool SortProperties
        {
            get
            {
                return DTEHelper.GetOptions(this.Store).SortProperties;
            }
        }

        public ModelClass NewClass(string owner, string name)
        {
            ModelClass cls = new ModelClass(Store);
            // TODO: Disabled to test server explorer drag drop bug of DeviceBuffer
            //Log(String.Format("Class: Name={0}, Schema={1}", name, owner));
            cls.Name = ModelHelper.GetSafeName(name, string.Empty);
            cls.Schema = owner;
            cls.Table = name;

            classes.Add(name, cls);
            return cls;
        }

        public ModelProperty NewProperty(ModelClass cls, Column column)
        {
            ModelProperty property = new ModelProperty(Store);
            property.Name = ModelHelper.GetSafeName(column.Name, _model.PropertyNameFilterExpression);
            property.Column = column.Name;
            property.NotNull = !column.Nullable;
            property.Accessor = Accessor.Public;
            property.ModelClass = cls;

            return property;
        }

        public ModelProperty NewPrimaryKey(ModelClass cls, Column column)
        {
            ModelProperty property = NewProperty(cls, column);
            property.KeyType = KeyType.PrimaryKey;

            return property;
        }


        public ModelProperty NewCompositeKey(ModelClass cls, Column column, string compositeKeyName)
        {
            ModelProperty property = NewProperty(cls, column);
            property.KeyType = KeyType.CompositeKey;
            property.CompositeKeyName = compositeKeyName;

            return property;
        }

        public ManyToOneRelation NewManyToOneRelation(ModelClass pkClass, ModelClass fkClass, string fkColumn)
        {
            ManyToOneRelation relation = new ManyToOneRelation(fkClass, pkClass)
                                             {
                                                 SourceColumn = fkColumn,
                                                 TargetColumnKey = fkColumn,
                                                 TargetTable = fkClass.Table,
                                                 TargetPropertyName = NamingHelper.GetPlural(fkClass.Name)
                                             };
            return relation;
        }

        public ManyToManyRelation NewManyToManyRelation(Relation source, Relation target)
        {
            ManyToManyRelation relation = new ManyToManyRelation(source.PrimaryModelClass, target.PrimaryModelClass)
                                              {
                                                  Table = source.ForeignModelClass.Name,
                                                  Schema = source.ForeignModelClass.Schema,
                                                  SourceColumn = source.ForeignColumn,
                                                  TargetColumn = target.ForeignColumn
                                              };
            return relation;
        }

        public void AssignModel(ModelClass cls)
        {
            cls.Model = _model;
        }

        public void FilterPKRelations(List<Relation> relations)
        {
            FilterRelations(relations, false);
        }

        public void FilterFKRelations(List<Relation> relations)
        {
            FilterRelations(relations, true);
        }

        private void FilterRelations(List<Relation> relations, bool isPrimary)
        {
            List<Relation> removeList = new List<Relation>();
            foreach (Relation relation in relations)
            {
                // We don't support creating relations between newly dropped classes and existing classes.
                // If so, we would also check classes already in the model. In such case, we would also remove
                // existing properties of existing classes which are part of the relation.
                // Current implementation does not take existing classes into account.
                if (isPrimary)
                {
                    if (_tableList.Find(
                            node => (node.Owner == relation.PrimaryOwner &&
                                     node.Name == relation.PrimaryTable)
                            ) == null)
                        removeList.Add(relation);
                }
                else
                {
                    if (_tableList.Find(
                            node => (node.Owner == relation.ForeignOwner &&
                                     node.Name == relation.ForeignTable)
                            ) == null)
                        removeList.Add(relation);
                }
            }
            foreach (Relation relation in removeList)
            {
                relations.Remove(relation);
            }
        }
    }
}
