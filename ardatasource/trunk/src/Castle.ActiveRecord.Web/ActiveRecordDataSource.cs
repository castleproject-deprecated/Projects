// Copyright 2004-2006 Castle Project - http://www.castleproject.org/
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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using NHibernate;
using NHibernate.Expression;

namespace Castle.ActiveRecord.Web
{   
    public class ActiveRecordDataSourceEventArgs : CancelEventArgs
    {
        protected Type m_modelType;
        protected bool m_wasError;
        protected bool m_doNotThrow = false;
        protected Exception m_exception;

        public Type ModelType
        {
            get { return m_modelType; }
            set { m_modelType = value; }
        }

        public bool WasError
        {
            get { return m_wasError; }
            set { m_wasError = value; }
        }

        public bool DoNotThrow
        {
            get { return m_doNotThrow; }
            set { m_doNotThrow = value; }
        }

        public Exception Exception
        {
            get { return m_exception; }
            set { m_exception = value; }
        }
    }

    public class ActiveRecordDataSourceFindEventArgs : ActiveRecordDataSourceEventArgs
    {
        protected List<ICriterion> m_criteria = new List<ICriterion>();
        protected List<Order> m_order = new List<Order>();
        protected IList m_result;

        public List<ICriterion> Criteria
        {
            get { return m_criteria; }
            set { m_criteria = value; }
        }

        public List<Order> Order
        {
            get { return m_order; }
            set { m_order = value; }
        }

        public IList Result
        {
            get { return m_result; }
            set { m_result = value; }
        }
    }  

    public class ActiveRecordDataSourceUpdateEventArgs : ActiveRecordDataSourceEventArgs
    {
        protected IDictionary m_updateValues;
        protected object m_entity;
        protected string m_keyProperty;
        protected object m_keyValue;

        public IDictionary UpdateValues
        {
            get { return m_updateValues; }
            set { m_updateValues = value; }
        }

        public object Entity
        {
            get { return m_entity; }
            set { m_entity = value; }
        }

        public string KeyProperty
        {
            get { return m_keyProperty; }
            set { m_keyProperty = value; }
        }

        public object KeyValue
        {
            get { return m_keyValue; }
            set { m_keyValue = value; }
        }
    }   

    public class ActiveRecordDataSourceDeleteEventArgs : ActiveRecordDataSourceEventArgs
    {        
        protected object m_entity;
        protected string m_keyProperty;
        protected object m_keyValue;

        public object Entity
        {
            get { return m_entity; }
            set { m_entity = value; }
        }

        public string KeyProperty
        {
            get { return m_keyProperty; }
            set { m_keyProperty = value; }
        }

        public object KeyValue
        {
            get { return m_keyValue; }
            set { m_keyValue = value; }
        }
    }

    public class ActiveRecordDataSourceCreateEventArgs : ActiveRecordDataSourceEventArgs
    {
        protected IDictionary m_createValues;
        protected object m_entity;
       
        public IDictionary CreateValues
        {
            get { return m_createValues; }
            set { m_createValues = value; }
        }

        public object Entity
        {
            get { return m_entity; }
            set { m_entity = value; }
        }
    }

    [ToolboxData("<{0}:ActiveRecordDataSource runat=\"server\"></{0}:ActiveRecordDataSource>")]
    [PersistChildren(false), ParseChildren(true), DefaultEvent("BeforeFind"), DefaultProperty("TypeName")]
    public class ActiveRecordDataSource : DataSourceControl
    {
        protected ActiveRecordDataSourceView m_view;
        protected string m_typeName = String.Empty;
        protected bool m_enablePaging = false;
        protected bool m_throwOnError = false;
        protected string m_defaultSort;
        protected int m_defaultMaximumRows = -1;
        protected string m_findMethod;
        protected string m_findMethodFirstResultParam;
        protected string m_findMethodMaxResultsParam;
        protected string m_findMethodOrderParam;
        protected string m_modelInstanceName;

        protected override DataSourceView GetView(string viewName)
        {
            return GetView();           
        }

        protected ActiveRecordDataSourceView GetView()
        {
            if (m_view == null)
            {
                m_view = new ActiveRecordDataSourceView(this, "DefaultView");

                if (IsTrackingViewState)
                {
                    m_view.TrackViewState();
                }
            }

            return m_view;
        }

        /// <summary>
        /// The model type name.
        /// </summary>
        [DefaultValue((string)null)]
        public string TypeName
        {
            get { return m_typeName; }
            set { m_typeName = value; }
        }

        /// <summary>
        /// Enable sliced find.
        /// </summary>
        [DefaultValue(false)]
        public bool EnablePaging
        {
            get { return m_enablePaging; }
            set { m_enablePaging = value; }
        }  
     
        /// <summary>
        /// Whether or not to throw an exception when an error occurs.  If this is false and an error occures, the appropriate error event will instead be raised.
        /// </summary>
        [DefaultValue(false)]
        public bool ThrowOnError
        {
            get { return m_throwOnError; }
            set { m_throwOnError = value; }
        }

        /// <summary>
        /// The default order for find.
        /// </summary>
        [DefaultValue((string)null)]
        public string DefaultSort
        {
            get { return m_defaultSort; }
            set { m_defaultSort = value; }
        }

        /// <summary>
        /// The default maximum results for find.
        /// </summary>
        [DefaultValue((int)-1)]
        public int DefaultMaximumRows
        {
            get { return m_defaultMaximumRows; }
            set { m_defaultMaximumRows = value; }
        }       

        /// <summary>
        /// Criteria for find.
        /// </summary>
        [Category("Data"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false)]
        public ParameterCollection FindParameters
        {
            get
            {
                return GetView().FindParameters;
            }
        }

        /// <summary>
        /// Criteria for update.
        /// </summary>
        [Category("Data"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false)]
        public ParameterCollection UpdateParameters
        {
            get
            {
                return GetView().UpdateParameters;
            }
        }

        /// <summary>
        /// Criteria for create.
        /// </summary>
        [Category("Data"), PersistenceMode(PersistenceMode.InnerProperty), DefaultValue((string)null), Editor("System.Web.UI.Design.WebControls.ParameterCollectionEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor)), MergableProperty(false)]
        public ParameterCollection CreateParameters
        {
            get
            {
                return GetView().CreateParameters;
            }
        }

        /// <summary>
        /// The name of a custom find method to use.  
        /// </summary>
        [DefaultValue((string)null)]
        public string FindMethod
        {
            get { return m_findMethod; }
            set { m_findMethod = value; }
        }

        /// <summary>
        /// The name of the parameter that will be passed the first result value.  If this property is null, the value is not passed to the function.  The parameter must be of type 'int'.
        /// </summary>
        [DefaultValue((string)null)]
        public string FindMethodFirstResultParam
        {
            get { return m_findMethodFirstResultParam; }
            set { m_findMethodFirstResultParam = value; }
        }

        /// <summary>
        /// The name of the parameter that will be passed the max results value.  If this property is null, the value is not passed to the function.  The parameter must be of type 'int'.
        /// </summary>
        [DefaultValue((string)null)]
        public string FindMethodMaxResultsParam
        {
            get { return m_findMethodMaxResultsParam; }
            set { m_findMethodMaxResultsParam = value; }
        }

        /// <summary>
        /// The name of the parameter that will be passed the order value.  If this property is null, the value is not passed to the function.  The parameter must be of type 'Order[]'.
        /// </summary>
        [DefaultValue((string)null)]
        public string FindMethodOrderParam
        {
            get { return m_findMethodOrderParam; }
            set { m_findMethodOrderParam = value; }
        }       

        /// <summary>
        /// Occurs before "Find" is executed.  
        /// </summary>
        /// <remarks>
        /// This event allows for modification of query criteria and order before it is sent to ActiveRecord.
        /// </remarks>
        public event EventHandler<ActiveRecordDataSourceFindEventArgs> BeforeFind;

        /// <summary>
        /// Occurs before "Create" is executed.  Allows for modification of data before it is sent to ActiveRecord.
        /// </summary>
        /// <remarks>
        /// This event allows for modification of model data before it is sent to ActiveRecord to be created. 
        /// </remarks>
        public event EventHandler<ActiveRecordDataSourceCreateEventArgs> BeforeCreate;

        /// <summary>
        /// Occurs before "Delete" is executed.  
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceDeleteEventArgs> BeforeDelete;

        /// <summary>
        /// Occurs before the "Update" is executed.
        /// </summary>
        /// <remarks>
        /// This event allows for modification of model data before it is sent to ActiveRecord to be updated.
        /// </remarks>
        public event EventHandler<ActiveRecordDataSourceUpdateEventArgs> BeforeUpdate;

        /// <summary>
        /// Occurs after the "Find" is executed.
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceFindEventArgs> Find;

        /// <summary>
        /// Occurs after the "Create" is executed.
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceCreateEventArgs> Create;

        /// <summary>
        /// Occurs after the "Delete" is executed.
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceDeleteEventArgs> Delete;

        /// <summary>
        /// Occurs after the "Update" is executed.
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceUpdateEventArgs> Update;

        /// <summary>
        /// Occurs when an error occurs during "Find".  
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceFindEventArgs> FindError;

        /// <summary>
        /// Occurs when an error occurs during "Create".  
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceCreateEventArgs> CreateError;

        /// <summary>
        /// Occurs when an error occurs during "Delete".  
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceDeleteEventArgs> DeleteError;

        /// <summary>
        /// Occurs when an error occurs during "Update".  
        /// </summary>
        public event EventHandler<ActiveRecordDataSourceUpdateEventArgs> UpdateError;

        public void OnBeforeFind(ActiveRecordDataSourceFindEventArgs e)
        {
            if (BeforeFind != null)
                BeforeFind(this, e);
        }

        public void OnBeforeCreate(ActiveRecordDataSourceCreateEventArgs e)
        {
            if (BeforeCreate != null)
                BeforeCreate(this, e);
        }

        public void OnBeforeDelete(ActiveRecordDataSourceDeleteEventArgs e)
        {
            if (BeforeDelete != null)
                BeforeDelete(this, e);
        }

        public void OnBeforeUpdate(ActiveRecordDataSourceUpdateEventArgs e)
        {
            if (BeforeUpdate != null)
                BeforeUpdate(this, e);
        }

        public void OnFind(ActiveRecordDataSourceFindEventArgs e)
        {
            if (Find != null)
                Find(this, e);
        }

        public void OnCreate(ActiveRecordDataSourceCreateEventArgs e)
        {
            if (Create != null)
                Create(this, e);
        }

        public void OnDelete(ActiveRecordDataSourceDeleteEventArgs e)
        {
            if (Delete != null)
                Delete(this, e);
        }

        public void OnUpdate(ActiveRecordDataSourceUpdateEventArgs e)
        {
            if (Update != null)
                Update(this, e);
        }

        public void OnFindError(ActiveRecordDataSourceFindEventArgs e)
        {
            if (FindError != null)
                FindError(this, e);
        }

        public void OnCreateError(ActiveRecordDataSourceCreateEventArgs e)
        {
            if (CreateError != null)
                CreateError(this, e);
        }

        public void OnDeleteError(ActiveRecordDataSourceDeleteEventArgs e)
        {
            if (DeleteError != null)
                DeleteError(this, e);
        }

        public void OnUpdateError(ActiveRecordDataSourceUpdateEventArgs e)
        {
            if (UpdateError != null)
                UpdateError(this, e);
        }
    }
}
