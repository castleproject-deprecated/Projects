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

using System.ComponentModel.Design;
using EnvDTE;

namespace Altinoren.ActiveWriter
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Diagnostics;
    using System.IO;
    using System.Windows.Forms;
    using Microsoft.VisualStudio.DataTools;
    using Microsoft.VisualStudio.Modeling;
    using Microsoft.VisualStudio.Modeling.Diagrams;
    using ServerExplorerSupport;

    public partial class ActiveRecordMapping
    {
        private DiagramManager _manager = null;
        private List<Relation> _relations = null;
        private OutputWindowHelper _output;

        /// <summary>
        /// Initialize the drag/drop when the diagram is associated with a view (i.e. when
        /// a model is loaded from a file)
        /// </summary>
        protected override void OnAssociated(DiagramAssociationEventArgs e)
        {
            base.OnAssociated(e);

            // Ensure we have a view
            if (e.DiagramView == null || e.DiagramView.DiagramClientView == null)
                return;

            // Wireup the drag/drop support
            Control ctrl = e.DiagramView.DiagramClientView;
            ctrl.AllowDrop = true;
            ctrl.DragOver += new DragEventHandler(OnDragOver);
            ctrl.DragDrop += new DragEventHandler(OnDragDrop);
        }

        /// <summary>
        /// OnDragOver is used to inform OLE that the DiagramClientView associated with
        /// the current diagram is willing to accept the DSRef data format. Additionally,
        /// we enforce that only tables can be copied onto the diagram. We would
        /// normally do this in OnDragEnter as it's the appropriate event, but the
        /// DiagramClientView does not properly implement OnDragEnter by calling its
        /// base method (i.e. any event we wire up to DragEnter would never fire)
        /// </summary>
        private void OnDragOver(object sender, DragEventArgs e)
        {
            // Check if the data present is in the DSRef format
            if (e.Data.GetDataPresent(DSRefNavigator.DataSourceReferenceFormat))
            {
                try
                {
                    // Create a navigator for the DSRef Consumer (and dispose it when finished)
                    using (DSRefNavigator navigator = new DSRefNavigator(e.Data.GetData(
                                                                             DSRefNavigator.DataSourceReferenceFormat)
                                                                         as Stream))
                        // Only allow a copy if they selected table leaf nodes (we of
                        // course don't set the Effect to None if not as the base classes
                        // do their own drag/drop manipulation on other data formats)
                        if (navigator.ContainsOnlyTables)
                            e.Effect = DragDropEffects.Copy;
                        }
                catch
                {
                }
            }
        }

        /// <summary>
        /// OnDragDrop is used to create classes corresponding to the selection dragged
        /// from the Server Explorer
        /// </summary>
        private void OnDragDrop(object sender, DragEventArgs e)
        {
            // Check if the data present is in the DSRef format
            if (e.Data.GetDataPresent(DSRefNavigator.DataSourceReferenceFormat))
            {
                try
                {
                    // Create a navigator for the DSRef Consumer (and dispose it when finished)
                    using (DSRefNavigator navigator = new DSRefNavigator(e.Data.GetData(
                                                                             DSRefNavigator.DataSourceReferenceFormat)
                                                                         as Stream))
                    {
                        _output = new OutputWindowHelper(DTEHelper.GetDTE(this.Store));

                        // Get connection info of the connection of selected tables
                        string providerType = null;
                        IDbConnection connection = ServerExplorerHelper.GetConnection(navigator, out providerType);

                        IDbHelper helper;

                        switch (providerType)
                        {
                            case "System.Data.SqlClient.SqlConnection":
                                helper = new SqlHelper((SqlConnection)connection);
                                break;
                            case "MySql.Data.MySqlClient.MySqlConnection":
                                helper = new MySqlHelper((MySql.Data.MySqlClient.MySqlConnection)connection);
                                break;
                            default:
                                // TODO: Support Oracle
                                // TODO: Support other databases with native providers.
                                // TODO: Shall we ask the user to describe the underlying connection? Ex: OleDB but SQL is underlying etc.
								Log(string.Format(@"Failed: ActiveWriter does not support model generation through {0}. Supported providers: System.Data.SqlClient.SqlConnection, MySql.Data.MySqlClient.MySqlConnection. You can help us improve this functionality, though. See http://www.castleproject.org/others/contrib/index.html to access ActiveWriter source code under the contrib repository, and check Dsl\ServerExplorerSupport\IDbHelper.cs for the start.", providerType));
                                return;
                        }

                        // Get the root element where we'll add the classes
                        Model model = Helper.GetModel(this.Store);
                        if (model == null)
                        {
                            Log("Failed: Cannot get the model for the store.");
                            return;
                        }

                        _manager = new DiagramManager(this.Store, model);
                        _manager.OutputWindow = _output;

                        // Create a transaction to add the clases.
                        using (Transaction txAdd =
                            model.Store.TransactionManager.BeginTransaction("Add classes"))
                        {
                            List<DSRefNode> tableList = new List<DSRefNode>();
                            // Get the tables from the Server Explorer selection
                            // We'll iterate this list twice to use nodes' list to
                            // determine if we have to generate relations for each
                            // table or not.
                            foreach (DSRefNode node in navigator.ChildTableNodes)
                            {
                                tableList.Add(node);
                            }

                            _manager.Tables = tableList;

                            _relations = new List<Relation>();

                            foreach (DSRefNode node in tableList)
                            {
                                // Create the table and add it to the model
                                ModelClass cls = _manager.NewClass(node.Owner, node.Name);
                                PopulateClass(cls, connection, helper);
                                _manager.AssignModel(cls);
                            }

                            // Create relations
                            if (_relations != null && _relations.Count > 0)
                            {
                                HandleRelations();
                            }

                            // Commit the transaction and add tables to the model
                            txAdd.Commit();
                        }

                        // TODO: Auto layout doesn't work well. Will check with future versions of DSL tools.
                        // this.AutoLayoutShapeElements(this.NestedChildShapes);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    _manager = null;
                    _relations = null;
                    _output = null;
                }
            }
        }

        private void PopulateClass(ModelClass cls, IDbConnection connection, IDbHelper helper)
        {
            if (cls != null && connection != null)
            {
                // Relations to this class (this class has the primary key)
                List<Relation> relationsTo = helper.GetPKRelations(cls);
                // If the other side of the relation (FK part) is not in our model, we won't
                // need the relation at all
                _manager.FilterPKRelations(relationsTo);

                // Relations from this class (this class has the foreign key)
                List<Relation> relationsFrom = helper.GetFKRelations(cls);
                // If the other side of the relation (PK part) is not in our model, we won't
                // need the relation at all
                _manager.FilterFKRelations(relationsFrom);

                // Properties
                List<Column> columns = helper.GetProperties(cls);
                if (columns != null && columns.Count > 0)
                {
                    List<Column> secondPass = new List<Column>();
                    foreach (Column column in columns)
                    {
                        // We'll handle primary and foreigns later. First, we'll create ordinary properties.
                        if (column.Primary ||
                                (column.ForeignConstraints.Count > 0 &&
                                 (Relation.GetCountOfMatchingRelations(relationsTo, column.ForeignConstraints) > 0 ||
                                  Relation.GetCountOfMatchingRelations(relationsFrom, column.ForeignConstraints) > 0)
                                )
                            )
                            secondPass.Add(column);
                        else
                            _manager.NewProperty(cls, column).ColumnType = helper.GetNHibernateType(column.DataType);
                    }

                    // Keys
                    List<Column> primaryKeys = null;
                    if (secondPass.Count > 0)
                    {
                        primaryKeys = Column.FindPrimaryKeys(secondPass);
                        if (primaryKeys != null && primaryKeys.Count > 0)
                        {
                            // Create primary and composite keys
                            if (primaryKeys.Count == 1)
                                _manager.NewPrimaryKey(cls, primaryKeys[0]).ColumnType =
                                    helper.GetNHibernateType(primaryKeys[0].DataType);
                            else
                            {
                                string keyClassName = cls.Name + Common.CompositeClassNameSuffix;
                                foreach (Column key in primaryKeys)
                                {
                                    _manager.NewCompositeKey(cls, key, keyClassName).ColumnType =
                                        helper.GetNHibernateType(key.DataType);
                                }
                            }
                        }
                    }

                    if (relationsTo != null && relationsTo.Count > 0)
                        foreach (Relation relation in relationsTo)
                        {
                            relation.PrimaryModelClass = cls;
                        }
                    _relations.AddRange(relationsTo);

                    if (relationsFrom != null && relationsFrom.Count > 0)
                    {
                        foreach (Relation relation in relationsFrom)
                        {
                            relation.ForeignModelClass = cls;
                        }
                        _relations.AddRange(relationsFrom);

                        // To define many to many and one to one, we need to know the pattern of foreign keys.
                        if (primaryKeys != null && primaryKeys.Count > 0)
                            foreach (Column key in primaryKeys)
                            {
                                Relation relation = Relation.GetForeginColumn(relationsFrom, key);
                                if (relation != null)
                                    relation.IsForeignColumnPrimary = true;
                            }
                    }
                }
            }
        }

        private void HandleRelations()
        {
            MatchRelations();

            CreateRelations();
        }

        private void MatchRelations()
        {
            List<Relation> processed = new List<Relation>();
            List<Relation> toDelete = new List<Relation>();

            foreach (Relation discoveredRelation in _relations)
            {
                if (!processed.Contains(discoveredRelation))
                {
                    Relation match = Relation.GetMatchingRelation(_relations, discoveredRelation);
                    if (match != null)
                    {
                        if (discoveredRelation.PrimaryModelClass != null)
                        {
                            discoveredRelation.ForeignModelClass = match.ForeignModelClass;
                            discoveredRelation.ForeignColumn = match.ForeignColumn;
                            discoveredRelation.ForeignOwner = match.ForeignOwner;
                            discoveredRelation.ForeignTable = match.ForeignTable;
                            discoveredRelation.IsForeignColumnPrimary = match.IsForeignColumnPrimary;
                        }
                        else
                        {
                            discoveredRelation.PrimaryModelClass = match.PrimaryModelClass;
                            discoveredRelation.PrimaryColumn = match.PrimaryColumn;
                            discoveredRelation.PrimaryOwner = match.PrimaryOwner;
                            discoveredRelation.PrimaryTable = match.PrimaryTable;
                        }

                        processed.Add(discoveredRelation);
                        processed.Add(match);

                        toDelete.Add(match);
                    }
                }
            }

            foreach (Relation relation in toDelete)
            {
                _relations.Remove(relation);
            }
        }

        private void CreateRelations()
        {
            // Only many2one and many2many right now.
            // TODO: Implement one2one.
            // Current implementation:
            // If a class have nothing but two FK columns to two different tables then it's a m2m...

            List<Relation> processed = new List<Relation>();
            foreach (Relation discoveredRelation in _relations)
            {
                if (!processed.Contains(discoveredRelation))
                {
                    if (discoveredRelation.ForeignModelClass != null && discoveredRelation.ForeignModelClass.Properties.Count == 0)
                    {
                        List<Relation> matches = Relation.GetFKRelationsFromSameClassToDifferentClasses(_relations, discoveredRelation);
                        if (matches.Count == 1)
                        {
                            processed.Add(discoveredRelation);
                            processed.AddRange(matches);

                            _manager.NewManyToManyRelation(discoveredRelation, matches[0]);
                            discoveredRelation.ForeignModelClass.Model = null;
                        }
                    }
                    else
                    {
                        _manager.NewManyToOneRelation(discoveredRelation.PrimaryModelClass, discoveredRelation.ForeignModelClass, discoveredRelation.ForeignColumn);
                    }
                }
            }
        }

        private void Log(string message)
        {
            _output.Write(string.Format("ActiveWriter: {0}", message));
        }
    }
}
