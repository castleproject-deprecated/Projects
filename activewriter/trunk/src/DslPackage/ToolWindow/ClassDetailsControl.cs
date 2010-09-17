// Copyright 2006 Gokhan Castle - http://altinoren.com/
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

namespace Castle.ActiveWriter.ToolWindow
{
    using System.Windows.Forms;
    using System.ComponentModel;
    using Microsoft.VisualStudio.Modeling;
    
    public partial class ClassDetailsControl : UserControl
    {
        private ModelClass modelClass;
 
        private DataGridView propertyGrid;
        private DataGridViewTextBoxColumn propertyNameColumn;
        private DataGridViewComboBoxColumn columnTypeColumn;
        private DataGridViewTextBoxColumn columnColumn;
        private DataGridViewComboBoxColumn keyTypeColumn;
        private DataGridViewTextBoxColumn descriptionColumn;

        private const string _addNewCellValue = "<Add Property>";
        
        public ClassDetailsControl()
        {
            InitializeComponent();
            SetupControls();
        }

        protected override bool ProcessDialogChar(char charCode)
        {
            if (charCode != ' ' && ProcessMnemonic(charCode))
            {
                return true;
            }
            return base.ProcessDialogChar(charCode);
        }
        
        public void Clear()
        {
            modelClass = null;
            propertyGrid.Rows.Clear();
            propertyGrid.RowCount = 0;
            propertyGrid.Visible = false;
        }

        public void Display(ModelClass modelClassToHandle)
        {
            propertyGrid.CancelEdit();
            propertyGrid.Rows.Clear();
            modelClass = modelClassToHandle;
            if (modelClassToHandle != null && modelClassToHandle.Properties != null)
                propertyGrid.RowCount = modelClassToHandle.Properties.Count + 1;
            propertyGrid.ClearSelection();
            propertyGrid.Visible = true;
        }
        
        private void SetupControls()
        {
            propertyGrid = new DataGridView();
            
            propertyNameColumn = new DataGridViewTextBoxColumn();
            columnTypeColumn = new DataGridViewComboBoxColumn();
            columnColumn = new DataGridViewTextBoxColumn();
            keyTypeColumn = new DataGridViewComboBoxColumn();
            descriptionColumn = new DataGridViewTextBoxColumn();
            
            ((ISupportInitialize)(propertyGrid)).BeginInit();
            
            SuspendLayout();
            
            propertyGrid.AutoGenerateColumns = false;
            propertyGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            propertyGrid.Columns.AddRange(new DataGridViewColumn[]
                                              {
                                                  propertyNameColumn,
                                                  columnTypeColumn,
                                                  columnColumn,
                                                  keyTypeColumn,
                                                  descriptionColumn
                                              });
            propertyGrid.Dock = DockStyle.Fill;
            propertyGrid.Location = new System.Drawing.Point(0, 0);
            propertyGrid.Name = "propertyGrid";
            propertyGrid.Size = new System.Drawing.Size(754, 153);
            propertyGrid.TabIndex = 2;
            propertyGrid.VirtualMode = true;
            propertyGrid.EditMode = DataGridViewEditMode.EditOnEnter;
            propertyGrid.RowHeadersVisible = false;
            propertyGrid.VirtualMode = true;
            propertyGrid.AllowUserToAddRows = false;
            propertyGrid.AllowUserToDeleteRows = true;
            propertyGrid.AllowUserToOrderColumns = false;

            propertyNameColumn.DataPropertyName = "Name";
            propertyNameColumn.HeaderText = "Name";
            propertyNameColumn.Name = "propertyNameColumn";

            columnTypeColumn.DataPropertyName = "ColumnTypeForToolWindow";
            columnTypeColumn.HeaderText = "ColumnType";
            columnTypeColumn.Name = "columnTypeColumn";
            columnTypeColumn.CellTemplate = new ColumnTypeComboBoxCell();
            columnTypeColumn.FlatStyle = FlatStyle.Flat;
            
            columnColumn.DataPropertyName = "Column";
            columnColumn.HeaderText = "Column";
            columnColumn.Name = "columnColumn";

            keyTypeColumn.DataPropertyName = "KeyTypeForToolWindow";
            keyTypeColumn.HeaderText = "KeyType";
            keyTypeColumn.Name = "keyTypeColumn";
            keyTypeColumn.CellTemplate = new KeyTypeComboBoxCell();
            keyTypeColumn.FlatStyle = FlatStyle.Flat;
 
            descriptionColumn.DataPropertyName = "Description";
            descriptionColumn.HeaderText = "Description";
            descriptionColumn.Name = "descriptionColumn";
            descriptionColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

            Controls.Add(propertyGrid);

            propertyGrid.CellValueNeeded += propertyGrid_CellValueNeeded;
            propertyGrid.CellValuePushed += propertyGrid_CellValuePushed;
            //propertyGrid.NewRowNeeded += new DataGridViewRowEventHandler(propertyGrid_NewRowNeeded);
            //propertyGrid.RowValidated += new DataGridViewCellEventHandler(propertyGrid_RowValidated);
            //propertyGrid.RowDirtyStateNeeded += new QuestionEventHandler(propertyGrid_RowDirtyStateNeeded);
            //propertyGrid.CancelRowEdit += new QuestionEventHandler(propertyGrid_CancelRowEdit);
            //propertyGrid.UserDeletingRow += new DataGridViewRowCancelEventHandler(propertyGrid_UserDeletingRow);

            propertyGrid.CellBeginEdit += propertyGrid_CellBeginEdit;

            ResumeLayout(false);
        }

        #region DataGridView Virtual Mode Events

        private void propertyGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e)
        {
            if (e.RowIndex == propertyGrid.RowCount - 1)
            {
                if (e.ColumnIndex == 0)
                {
                    e.Value = _addNewCellValue;
                }
            }
            else if (modelClass != null && modelClass.Properties.Count > 0)
            {
                ModelProperty property = modelClass.Properties[e.RowIndex];
                switch (e.ColumnIndex)
                {
                    case 0:
                        e.Value = property.Name;
                        break;
                    case 1:
                        e.Value = property.ColumnType.ToString();
                        break;
                    case 2:
                        e.Value = property.Column;
                        break;
                    case 3:
                        e.Value = property.KeyType.ToString();
                        break;
                    case 4:
                        e.Value = property.Description;
                        break;
                    default:
                        e.Value = null;
                        break;
                }
            }
        }

        private void propertyGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e)
        {
            if (modelClass != null)
            {
                if (e.RowIndex > modelClass.Properties.Count - 1)
                {
                    using (Transaction txAdd = modelClass.Model.Store.TransactionManager.BeginTransaction("Add property"))
                    {
                        ModelProperty property = new ModelProperty(modelClass.Model.Store);
                        property.Name = e.Value.ToString();
                        property.ModelClass = modelClass;
                        txAdd.Commit();
                    }
                }
                else
                {
                    ModelProperty property = modelClass.Properties[e.RowIndex];

                    using (Transaction txAdd = property.ModelClass.Model.Store.TransactionManager.BeginTransaction("Add property"))
                    {
                        switch (e.ColumnIndex)
                        {
                            case 0:
                                property.Name = e.Value.ToString();
                                break;
                            case 1:
                                property.ColumnType = (NHibernateType)System.Enum.Parse(typeof(NHibernateType), e.Value.ToString(), true);
                                break;
                            case 2:
                                property.Column = e.Value.ToString();
                                break;
                            case 3:
                                property.KeyType = (KeyType)System.Enum.Parse(typeof(KeyType), e.Value.ToString(), true);
                                break;
                            case 4:
                                property.Description = e.Value.ToString();
                                break;
                            default:
                                e.Value = null;
                                break;
                        }

                        txAdd.Commit();
                    }
                }

                propertyGrid.Invalidate();
            }
        }
        
        #endregion

        #region DataGridView Events

        // Cancels the edit if the cell being edited is in the last row but not the first cell when the
        // first cell is still in the state of <Add New>.
        private void propertyGrid_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == propertyGrid.RowCount - 1 && e.ColumnIndex > 0)
            {
                if (propertyGrid.Rows[e.RowIndex].Cells[0].Value.ToString() == _addNewCellValue)
                    e.Cancel = true;
            }
        }
        #endregion

    }
}
