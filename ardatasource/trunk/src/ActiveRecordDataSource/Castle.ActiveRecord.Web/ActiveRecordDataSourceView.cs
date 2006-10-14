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
using System.Collections.Specialized;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Compilation;
using System.Web.UI;
using System.Web.UI.WebControls;
using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework;
using Castle.ActiveRecord.Queries;
using NHibernate;
using NHibernate.Expression;
using NHibernate.Metadata;
using NHibernate.SqlCommand;
using Parameter=System.Web.UI.WebControls.Parameter;

namespace Castle.ActiveRecord.Web
{
    public class ActiveRecordDataSourceView : DataSourceView, IStateManager
    {
        [Serializable]
        protected class ViewStateContainer
        {
            public object FindParameters;
            public object UpdateParameters;
            public object CreateParameters;
        }

        protected ActiveRecordDataSource m_source = null;
        protected string m_view = String.Empty;
        protected ParameterCollection m_findParameters = null;
        protected ParameterCollection m_updateParameters = null;
        protected ParameterCollection m_createParameters = null;
        protected bool m_trackingState = false;

        protected ActiveRecordDataSource Source
        {
            get { return m_source; }
            set { m_source = value; }
        }       

        public ParameterCollection FindParameters
        {
            get 
            {
                if (m_findParameters == null)
                {
                    m_findParameters = new ParameterCollection();

                    if (m_trackingState)
                        ((IStateManager)m_findParameters).TrackViewState();

                }
                
                return m_findParameters;
            }
        }

        public ParameterCollection UpdateParameters
        {
            get
            {
                if (m_updateParameters == null)
                {
                    m_updateParameters = new ParameterCollection();

                    if (m_trackingState)
                        ((IStateManager)m_updateParameters).TrackViewState();

                }

                return m_updateParameters;
            }
        }

        public ParameterCollection CreateParameters
        {
            get
            {
                if (m_createParameters == null)
                {
                    m_createParameters = new ParameterCollection();

                    if (m_trackingState)
                        ((IStateManager)m_createParameters).TrackViewState();

                }

                return m_createParameters;
            }
        }

        public override bool CanDelete
        {
            get
            {
                return true;
            }
        }

        public override bool CanInsert
        {
            get
            {
                return true;
            }
        }

        public override bool CanPage
        {
            get
            {
                return Source.EnablePaging;
            }
        }

        public override bool CanRetrieveTotalRowCount
        {
            get
            {
                return true;
            }
        }

        public override bool CanSort
        {
            get
            {
                return true;
            }
        }

        public override bool CanUpdate
        {
            get
            {
                return true;
            }
        }        

        public ActiveRecordDataSourceView(ActiveRecordDataSource _source, string _view) : base(_source, _view)
        {
            m_source = _source;
            m_view = _view;
        }

        protected static Order[] CreateOrderExpression(string sort)
        {
            if (String.IsNullOrEmpty(sort))
                return new Order[0];

            List<Order> order = new List<Order>();
            string[] temp = sort.Split(',');

            foreach (string param in temp)
            {
                Match match = Regex.Match(param, @"(?<param>\w+)(\s+(?<dir>ASC|DESC))?", RegexOptions.IgnoreCase);
                string name = "";
                bool asc = true;

                if (match.Success)
                {
                    name = match.Groups["param"].Value;

                    if (match.Groups["dir"].Success && match.Groups["dir"].Value.ToUpper() == "DESC")
                        asc = false;

                    if (asc)
                        order.Add(Order.Asc(name));
                    else
                        order.Add(Order.Desc(name));
                }
            }

            return order.ToArray();
        }

        protected void CreateCriteriaFromFindParameters(ActiveRecordDataSourceFindEventArgs args)
        {
            IOrderedDictionary values = FindParameters.GetValues(HttpContext.Current, Source);

            for (int i = 0; i < FindParameters.Count; i++)
            {
                Parameter parameter = FindParameters[i];

                if (parameter is NullParameter)
                {
                    NullParameter nullParameter = (NullParameter)parameter;

                    if (nullParameter.Expression == NullExpression.IsNull)
                        args.Criteria.Add(Expression.IsNull(parameter.Name));
                    else if (nullParameter.Expression == NullExpression.IsNotNull)
                        args.Criteria.Add(Expression.IsNotNull(parameter.Name));
                }
                else
                {
                    args.Criteria.Add(Expression.Eq(parameter.Name, values[i]));
                }
            }
        }

        protected void CreateOrderFromFindParameters(ActiveRecordDataSourceFindEventArgs args, string expr)
        {
            if (String.IsNullOrEmpty(expr))
                return;

            args.Order.AddRange(CreateOrderExpression(expr));
        }

        protected int ExecuteCount(ActiveRecordDataSourceFindEventArgs args)
        {
            IList result = ActiveRecordMediator.ExecuteQuery(new CriteraCountQuery(args.ModelType, args.Criteria.ToArray())) as IList;

            return Convert.ToInt32(result[0]);
        }

        protected override IEnumerable ExecuteSelect(DataSourceSelectArguments arguments)
        {
            if (!String.IsNullOrEmpty(Source.FindMethod))
                return ExecuteCustomSelect(arguments);

            if (CanPage)
                arguments.AddSupportedCapabilities(DataSourceCapabilities.Page);

            if (CanSort)
                arguments.AddSupportedCapabilities(DataSourceCapabilities.Sort);

            if (CanRetrieveTotalRowCount)
                arguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);

            ActiveRecordDataSourceFindEventArgs args = new ActiveRecordDataSourceFindEventArgs();
            CreateCriteriaFromFindParameters(args);
            CreateOrderFromFindParameters(args, arguments.SortExpression);

            //set default sort
            if (args.Order.Count <= 0)
                CreateOrderFromFindParameters(args, Source.DefaultSort);

            args.ModelType = BuildManager.GetType(Source.TypeName, false, true);

            Source.OnBeforeFind(args);

            if (args.Cancel)
                return null;

            Array result = null;

            try
            {
                if (arguments.RetrieveTotalRowCount)
                    arguments.TotalRowCount = ExecuteCount(args);

                bool useSlicedFind = false;
                int firstResult = 0;
                int maxResults = 0;

                if (CanPage)
                {
                    useSlicedFind = true;
                    firstResult = arguments.StartRowIndex;
                    maxResults = arguments.MaximumRows;
                }
                else if (Source.DefaultMaximumRows > 0)
                {
                    useSlicedFind = true;
                    firstResult = 0;
                    maxResults = Source.DefaultMaximumRows;
                }

                if (useSlicedFind)
                    result = ActiveRecordMediator.SlicedFindAll(args.ModelType, firstResult, maxResults, args.Order.ToArray(), args.Criteria.ToArray());
                else
                    result = ActiveRecordMediator.FindAll(args.ModelType, args.Order.ToArray(), args.Criteria.ToArray());
                
            }
            catch (Exception e)
            {
                args.Exception = e;
                args.WasError = true;

                Source.OnFindError(args);

                if (Source.ThrowOnError && !args.DoNotThrow)
                    throw;

                return null;
            }

            args.Result = result;

            Source.OnFind(args);

            return result;
        }

        protected Dictionary<string, object> CreateMethodParametersFromFindParameters()
        {
            Dictionary<string, object> methodParameters = new Dictionary<string, object>();
            IOrderedDictionary values = FindParameters.GetValues(HttpContext.Current, Source);

            for (int i = 0; i < FindParameters.Count; i++)
            {
                Parameter parameter = FindParameters[i];

                methodParameters[parameter.Name] = values[i];
            }

            return methodParameters;
        }

        protected void ResolveCustomFindMethod(Type model, Dictionary<string, object> findParameters, out MethodInfo findMethod, out Dictionary<string, int> parameterNameToIndex)
        {           
            findMethod = null;
            parameterNameToIndex = new Dictionary<string,int>();

            MethodInfo[] methods = model.GetMethods(BindingFlags.Static | BindingFlags.Public);

            foreach (MethodInfo method in methods)
            {
                ParameterInfo[] methodParameters = method.GetParameters();

                if (methodParameters.Length != findParameters.Keys.Count)
                    continue;

                int index = -1;
                bool match = true;
                foreach (string findParameterName in findParameters.Keys)
                {
                    if ((index = IndexOfMatchingParameter(methodParameters, findParameterName, findParameters[findParameterName])) >= 0)
                        parameterNameToIndex[findParameterName] = index;
                    else
                        match = false;
                }

                if (match)
                {
                    findMethod = method;
                    return;
                }
            }
        }

        protected int IndexOfMatchingParameter(ParameterInfo[] parameters, string name, object value)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];

                if (String.Compare(parameter.Name, name, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    if (value == null && !parameter.ParameterType.IsValueType)
                        return i;

                    if (parameter.ParameterType.IsAssignableFrom(value.GetType()))
                        return i;
                }
            }

            return -1;
        }     

        protected Array ExecuteCustomSelect(DataSourceSelectArguments arguments)
        {
            if (CanPage)
                arguments.AddSupportedCapabilities(DataSourceCapabilities.Page);

            if (CanSort)
                arguments.AddSupportedCapabilities(DataSourceCapabilities.Sort);

            if (CanRetrieveTotalRowCount)
                arguments.AddSupportedCapabilities(DataSourceCapabilities.RetrieveTotalRowCount);
            
            ActiveRecordDataSourceFindEventArgs args = new ActiveRecordDataSourceFindEventArgs();
            CreateOrderFromFindParameters(args, arguments.SortExpression);

            if (args.Order.Count <= 0)
                CreateOrderFromFindParameters(args, Source.DefaultSort);

            args.ModelType = BuildManager.GetType(Source.TypeName, false, true);

            Source.OnBeforeFind(args);

            if (args.Cancel)
                return null;

            bool useSlicedFind = false;
            int firstResult = 0;
            int maxResults = 0;

            if (CanPage)
            {
                useSlicedFind = true;
                firstResult = arguments.StartRowIndex;
                maxResults = arguments.MaximumRows;
            }
            else if (Source.DefaultMaximumRows > 0)
            {
                useSlicedFind = true;
                firstResult = 0;
                maxResults = Source.DefaultMaximumRows;
            }

            Dictionary<string, object> methodParameters = CreateMethodParametersFromFindParameters();

            if (!String.IsNullOrEmpty(Source.FindMethodFirstResultParam) && useSlicedFind)
                methodParameters[Source.FindMethodFirstResultParam] = firstResult;

            if (!String.IsNullOrEmpty(Source.FindMethodMaxResultsParam) && useSlicedFind)
                methodParameters[Source.FindMethodMaxResultsParam] = maxResults;

            if (!String.IsNullOrEmpty(Source.FindMethodOrderParam) && CanSort)
                methodParameters[Source.FindMethodOrderParam] = args.Order.ToArray();

            Array result = null;

            try
            {
                if (arguments.RetrieveTotalRowCount)
                    arguments.TotalRowCount = ExecuteCount(args);

                MethodInfo findMethod;
                Dictionary<string, int> methodParameterNameToIndex;

                ResolveCustomFindMethod(args.ModelType, methodParameters, out findMethod, out methodParameterNameToIndex);

                if (findMethod == null)
                    throw new ApplicationException(String.Format("Invalid custom find method '{0}'.", Source.FindMethod));

                object[] findMethodArgs = new object[methodParameters.Keys.Count];

                foreach (string key in methodParameters.Keys)
                    findMethodArgs[methodParameterNameToIndex[key]] = methodParameters[key];

                result = findMethod.Invoke(null, findMethodArgs) as Array;
            }
            catch (Exception e)
            {
                args.Exception = e;
                args.WasError = true;

                Source.OnFindError(args);

                if (Source.ThrowOnError && !args.DoNotThrow)
                    throw;

                return null;
            }

            args.Result = result;

            Source.OnFind(args);

            return result;
        }                 

        protected override int ExecuteDelete(IDictionary keys, IDictionary oldValues)
        {
            ActiveRecordDataSourceDeleteEventArgs args = new ActiveRecordDataSourceDeleteEventArgs();
            args.ModelType = BuildManager.GetType(Source.TypeName, false, true);

            IClassMetadata meta = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(args.ModelType).GetClassMetadata(args.ModelType);
            if (!meta.HasIdentifierProperty)
                throw new ApplicationException("ActiveRecordDataSourceView requires a primary key for the delete method.");

            args.KeyProperty = meta.IdentifierPropertyName;
            args.KeyValue = keys[meta.IdentifierPropertyName];

            try
            {
                args.Entity = ActiveRecordMediator.FindByPrimaryKey(args.ModelType, args.KeyValue);            

                Source.OnBeforeDelete(args);

                if (args.Cancel)
                    return 0;
            
                ActiveRecordMediator.Delete(args.Entity);
            }
            catch (Exception e)
            {
                args.Exception = e;
                args.WasError = true;

                Source.OnDeleteError(args);

                if (Source.ThrowOnError && !args.DoNotThrow)
                    throw;

                return 0;
            }

            Source.OnDelete(args);

            return 1;
        }

        protected void CreateValuesFromUpdateParameters(ActiveRecordDataSourceUpdateEventArgs args)
        {
            IOrderedDictionary values = UpdateParameters.GetValues(HttpContext.Current, Source);

            for (int i = 0; i < UpdateParameters.Count; i++)
            {
                Parameter parameter = UpdateParameters[i];
                args.UpdateValues[parameter.Name] = values[i];
            }
        }

        protected override int ExecuteUpdate(IDictionary keys, IDictionary values, IDictionary oldValues)
        {
            ActiveRecordDataSourceUpdateEventArgs args = new ActiveRecordDataSourceUpdateEventArgs();
            args.ModelType = BuildManager.GetType(Source.TypeName, false, true);
            args.UpdateValues = values;

            CreateValuesFromUpdateParameters(args);

            IClassMetadata meta = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(args.ModelType).GetClassMetadata(args.ModelType);
            if (!meta.HasIdentifierProperty)
                throw new ApplicationException("ActiveRecordDataSourceView requires a primary key for the update method.");

            args.KeyProperty = meta.IdentifierPropertyName;
            args.KeyValue = keys[meta.IdentifierPropertyName];

            try
            {
                args.Entity = ActiveRecordMediator.FindByPrimaryKey(args.ModelType, args.KeyValue);

                Source.OnBeforeUpdate(args);

                if (args.Cancel)
                    return 0;

                foreach (string key in args.UpdateValues.Keys)
                {
                    meta.SetPropertyValue(args.Entity, key, args.UpdateValues[key]);
                }

                ActiveRecordMediator.Update(args.Entity);
            }
            catch (Exception e)
            {
                args.Exception = e;
                args.WasError = true;

                Source.OnUpdateError(args);

                if (Source.ThrowOnError && !args.DoNotThrow)
                    throw;

                return 0;
            }

            Source.OnUpdate(args);

            return 1;
        }

        protected void CreateValuesFromCreateParameters(ActiveRecordDataSourceCreateEventArgs args)
        {
            IOrderedDictionary values = CreateParameters.GetValues(HttpContext.Current, Source);

            for (int i = 0; i < CreateParameters.Count; i++)
            {
                Parameter parameter = CreateParameters[i];
                args.CreateValues[parameter.Name] = values[i];
            }
        }

        protected override int ExecuteInsert(IDictionary values)
        {
            ActiveRecordDataSourceCreateEventArgs args = new ActiveRecordDataSourceCreateEventArgs();
            args.ModelType = BuildManager.GetType(Source.TypeName, false, true);
            args.CreateValues = values;

            CreateValuesFromCreateParameters(args);

            IClassMetadata meta = ActiveRecordMediator.GetSessionFactoryHolder().GetSessionFactory(args.ModelType).GetClassMetadata(args.ModelType);
            if (!meta.HasIdentifierProperty)
                throw new ApplicationException("ActiveRecordDataSourceView requires a primary key for the update method.");

            Source.OnBeforeCreate(args);

            if (args.Cancel)
                return 0;

            try
            {
                if (args.Entity == null)
                    args.Entity = Activator.CreateInstance(args.ModelType);

                foreach (string key in args.CreateValues.Keys)
                {
                    meta.SetPropertyValue(args.Entity, key, args.CreateValues[key]);
                }

                ActiveRecordMediator.Create(args.Entity);
            }
            catch (Exception e)
            {
                args.Exception = e;
                args.WasError = true;

                Source.OnCreateError(args);

                if (Source.ThrowOnError && !args.DoNotThrow)
                    throw;

                return 0;
            }

            Source.OnCreate(args);

            return 1;
        }

        #region IStateManager Members

        public bool IsTrackingViewState
        {
            get { return m_trackingState; }
        }
       
        public void LoadViewState(object state)
        {
            if (state == null)
                return;

            ViewStateContainer container = (ViewStateContainer)state;

            if (container.FindParameters != null)
                ((IStateManager)FindParameters).LoadViewState(container.FindParameters);

            if (container.CreateParameters != null)
                ((IStateManager)CreateParameters).LoadViewState(container.CreateParameters);

            if (container.UpdateParameters != null)
                ((IStateManager)UpdateParameters).LoadViewState(container.UpdateParameters);
        }

        public object SaveViewState()
        {
            ViewStateContainer container = new ViewStateContainer();

            if (m_findParameters != null)
                container.FindParameters = ((IStateManager)m_findParameters).SaveViewState();            

            if (m_createParameters != null)
                container.CreateParameters = ((IStateManager)m_createParameters).SaveViewState();

            if (m_updateParameters != null)
                container.UpdateParameters = ((IStateManager)m_updateParameters).SaveViewState();

            if (container.FindParameters == null && container.CreateParameters == null && container.UpdateParameters == null)
                return null;

            return container;
        }

        public void TrackViewState()
        {
            m_trackingState = true;

            if (m_findParameters != null)
                ((IStateManager)m_findParameters).TrackViewState();            

            if (m_createParameters != null)
                ((IStateManager)m_createParameters).TrackViewState();

            if (m_updateParameters != null)
                ((IStateManager)m_updateParameters).TrackViewState();
        }

        #endregion
    }
}
