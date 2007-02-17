// Copyright 2004-2005 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Views.AspView
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Collections;

    using Castle.MonoRail.Framework;
    using Castle.MonoRail.Framework.Helpers;
    using System.Reflection;

    public abstract class AspViewBase
    {
        #region members
        protected TextWriter _outputWriter;
        protected TextWriter _viewOutput;
        protected Dictionary<string, object> _properties;
        protected IList<IDictionary> _extentedPropertiesList;
        protected IRailsEngineContext _context;
        protected Controller _controller;
        protected AspViewEngine _viewEngine;
        protected AspViewBase _parentView;
        #endregion
        #region props
		/// <summary>
		/// Gets the AjaxHelper instance
		/// </summary>
		protected AjaxHelper AjaxHelper
		{
			get { return (AjaxHelper)Properties["AjaxHelper"]; }
		}
		/// <summary>
		/// Gets the Application's virtual root
		/// </summary>
		protected string siteRoot
		{
			get { return (string)Properties["siteRoot"]; }
		}
		/// <summary>
		/// Gets the Application's full virtual root, including protocol, host and port
		/// </summary>
        protected string fullSiteRoot
		{
            get { return (string)Properties["fullSiteRoot"]; }
		}
        /// <summary>
        /// Gets the DictHelper instance
        /// </summary>
        protected DictHelper DictHelper 
        {
            get { return (DictHelper)Properties["DictHelper"]; }
        }
        /// <summary>
        /// Gets the Effects2Helper instance
        /// </summary>
        protected ScriptaculousHelper ScriptaculousHelper
        {
            get { return (ScriptaculousHelper)Properties["ScriptaculousHelper"]; }
        }
        /// <summary>
        /// Gets the EffectsFatHelper instance
        /// </summary>
        protected EffectsFatHelper EffectsFatHelper 
        {
            get { return (EffectsFatHelper)Properties["EffectsFatHelper"]; }
        }
        /// <summary>
        /// Gets the FormHelper instance
        /// </summary>
        protected FormHelper FormHelper 
        {
            get { return (FormHelper)Properties["FormHelper"]; }
        }
        /// <summary>
        /// Gets the HtmlHelper instance
        /// </summary>
        protected HtmlHelper HtmlHelper 
        {
            get { return (HtmlHelper)Properties["HtmlHelper"]; }
        }
        /// <summary>
        /// Gets the PaginationHelper instance
        /// </summary>
        protected PaginationHelper PaginationHelper 
        {
            get { return (PaginationHelper)Properties["PaginationHelper"]; }
        }
        /// <summary>
        /// Gets the ValidationHelper instance
        /// </summary>
        protected ValidationHelper ValidationHelper 
        {
            get { return (ValidationHelper)Properties["ValidationHelper"]; }
        }
        /// <summary>
        /// Gets the WizardHelper instance
        /// </summary>
        protected WizardHelper WizardHelper 
        {
            get { return (WizardHelper)Properties["WizardHelper"]; }
        }
        
        /// <summary>
        /// Gets the output writer for the current view rendering
        /// </summary>
        public TextWriter OutputWriter
        {
            get { return _outputWriter; }
        }
        /// <summary>
        /// Used only in layouts. Gets or sets the output writer for the view
        /// </summary>
        public TextWriter ViewOutput
        {
            get { return _viewOutput; }
            set { _viewOutput = value; }
        }
        /// <summary>
        /// Used only in layouts. Gets the view contents
        /// </summary>
        public string ViewContents
        {
            get
            {
                return ((StringWriter)ViewOutput).GetStringBuilder().ToString();
            }
        }
        /// <summary>
        /// Gets the properties container. Based on current property containers that was sent from the controller, such us PropertyBag, Flash, etc.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get { return _properties; }
        }
        /// <summary>
        /// Gets the extended properties collection
        /// </summary>
        protected IList<IDictionary> ExtentedPropertiesList
        {
            get { return _extentedPropertiesList; }
        }
        /// <summary>
        /// Gets the current Rails context
        /// </summary>
        public IRailsEngineContext Context
        {
            get { return _context; }
        }
        /// <summary>
        /// Gets the calling controller
        /// </summary>
        public Controller Controller
        {
            get { return _controller; }
        }
        /// <summary>
        /// Gets the view engine instance
        /// </summary>
        public AspViewEngine ViewEngine
        {
            get { return _viewEngine; }
        }
        /// <summary>
        /// Gets a reference to the view's parent view
        /// </summary>
        public AspViewBase ParentView
        {
            get { return _parentView; }
        }
        #endregion
        public AspViewBase(AspViewEngine viewEngine, TextWriter output, IRailsEngineContext context, Controller controller)
        {
            _viewEngine = viewEngine;
            _outputWriter = output;
            _context = context;
            _controller = controller;
            InitProperties(context, controller);
        }

        private void InitProperties(IRailsEngineContext context, Controller controller)
        {
            _properties = new Dictionary<string, object>(CaseInsensitiveStringComparer.Default);
            _properties.Add("context", _context);
            _properties.Add("request", _context.Request);
            _properties.Add("response", _context.Response);
            _properties.Add("session", _context.Session);
            _properties.Add("controller", _controller);
            if (_controller.Resources != null)
                foreach (string key in _controller.Resources.Keys)
                    if (key != null)
                        _properties[key] = _controller.Resources[key];
            foreach (string key in _controller.Helpers.Keys)
                if (key != null)
                    _properties[key] = _controller.Helpers[key];
            foreach (string key in _context.Params.Keys)
                if (key != null)
                    _properties[key] = _context.Params[key];
            foreach (DictionaryEntry entry in _context.Flash)
                _properties[entry.Key.ToString()] = entry.Value;
            foreach (DictionaryEntry entry in _controller.PropertyBag)
                _properties[entry.Key.ToString()] = entry.Value;
            _properties["siteRoot"] = _context.ApplicationPath;
            _properties["fullSiteRoot"] = _context.Request.Uri.GetLeftPart(UriPartial.Authority) + _context.ApplicationPath;
            _extentedPropertiesList = new List<IDictionary>();
        }
        /// <summary>
        /// When overriden in a concrete view class, renders the view content to the output writer
        /// </summary>
        public abstract void Render();
		/*  DELETE
        protected virtual string ScriptDirectory
        {
            get { return _viewEngine.ViewRootDir; }
        }
		 * */
        /// <summary>
        /// Renders another view in place
        /// </summary>
        /// <param name="subViewName">The sub view's name</param>
        protected void OutputSubView(string subViewName)
        {
            OutputSubView(subViewName, new Dictionary<string,object>());
        }
        /// <summary>
        /// Renders another view in place
        /// </summary>
        /// <param name="subViewName">The sub view's name</param>
        /// <param name="parameters">Parameters that can be sent to the sub view's Properties container</param>
        protected void OutputSubView(string subViewName, IDictionary<string, object> parameters)
        {
			OutputSubView(subViewName, _outputWriter, parameters);
        }
		/// <summary>
		/// Renders another view in place
		/// </summary>
		/// <param name="subViewName">The sub view's name</param>
		/// <param name="parameters">Parameters that can be sent to the sub view's Properties container</param>
		/// <param name="writer">The writer that will be used for the sub view's output</param>
		protected void OutputSubView(string subViewName, TextWriter writer, IDictionary<string, object> parameters)
		{
			string subViewFileName = GetSubViewFileName(subViewName);
			AspViewBase subView = _viewEngine.GetView(subViewFileName, writer, _context, _controller);
			if (parameters != null)
				foreach (string key in parameters.Keys)
					if (parameters[key] != null)
						subView.Properties[key] = parameters[key];
			subView.Render();
		}

		/// <summary>
		/// Renders another view in place
		/// </summary>
		/// <param name="subViewName">The sub view's name</param>
		/// <param name="parameters">Parameters that can be sent to the sub view's Properties container</param>
		protected void OutputSubView(string subViewName, params object[] arguments)
		{
			OutputSubView(subViewName, _outputWriter, arguments);
		}
		
		/// <summary>
        /// Renders another view in place
        /// </summary>
        /// <param name="subViewName">The sub view's name</param>
        /// <param name="parameters">Parameters that can be sent to the sub view's ordered as name, value</param>
		/// <param name="writer">The writer that will be used for the sub view's output</param>
		internal void OutputSubView(string subViewName, TextWriter writer, params object[] arguments)
        {
			IDictionary<string, object> parameters =
				Utilities.ConvertArgumentsToParameters(arguments);
            OutputSubView(subViewName, writer, parameters);
        }

        /// <summary>
        /// Stack of writers, used as buffers for viewfilters
        /// </summary>
        Stack<TextWriter> outputWriters = new Stack<TextWriter>();
        /// <summary>
        /// Maintains the currently active view filters
        /// </summary>
        Stack<IViewFilter> viewFilters = new Stack<IViewFilter>();
        /// <summary>
        /// Searching a view filter given it's type's name
        /// </summary>
        /// <param name="filterName">the filter's typeName</param>
        /// <returns>System.Type of the filter</returns>
		private Type GetFilterType(string filterName)
		{
			Type filterType = null;
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				foreach (Type type in assembly.GetTypes())
				{
					if (type.Name.Equals(filterName, StringComparison.CurrentCultureIgnoreCase) && type.GetInterface("IViewFilter") != null)
					{
						filterType = type;
						break;
					}

				}
			}
			if (filterType == null)
				throw new RailsException("Cannot find a viewfilter [{0}]", filterName);
			return filterType;
		}
        /// <summary>
        /// Signaling the current view to start bufferring the following writes, filtering it later
        /// </summary>
        /// <param name="filterName">The filter's type name to apply</param>
        protected void StartFiltering(string filterName)
        {
            Type filterType = GetFilterType(filterName);
            IViewFilter filter = (IViewFilter)Activator.CreateInstance(filterType);
            StartFiltering(filter);
        }
        /// <summary>
        /// Signaling the current view to start bufferring the following writes, filtering it later
        /// </summary>
        /// <param name="filter">The filter to apply</param>
        protected void StartFiltering(IViewFilter filter)
        {
            outputWriters.Push(_outputWriter);
            _outputWriter = new StringWriter();
            viewFilters.Push(filter);
        }
        /// <summary>
        /// Signals the current view to apply the last view filter that was started on the buffered output
        /// </summary>
        protected void EndFiltering()
        {
            string original = _outputWriter.ToString();
            IViewFilter filter = viewFilters.Pop();
            string filtered = filter.ApplyOn(original);
            _outputWriter.Dispose();
            _outputWriter = outputWriters.Pop();
            _outputWriter.Write(filtered);
        }
        /// <summary>
        /// Output a string to the current output writer
        /// </summary>
        /// <param name="message">Message to output</param>
        protected void Output(string message)
        {
            OutputWriter.Write(message);
        }
        /// <summary>
        /// Output a string to the current output writer
        /// </summary>
        /// <param name="message">Message to output</param>
        /// <param name="arguments">Message's format arguments</param>
        protected void Output(string message, params object[] arguments)
        {
            Output(string.Format(message, arguments));
        }
        /// <summary>
        /// Output an object's 'ToString()' to the current output writer
        /// </summary>
        /// <param name="message">object to output</param>
        protected void Output(object message)
        {
            Output(message.ToString());
        }
        /// <summary>
        /// Gets a quallified path and filename to a sub view given it's name
        /// </summary>
        /// <param name="subViewName">The sub view's name</param>
        /// <returns>Relative or absolute path and filename to the sub view</returns>
        private string GetSubViewFileName(string subViewName)
        {
            if (subViewName[0] == '/' || subViewName[0] == '\\')
                return subViewName + _viewEngine.Extension;
            else
                return Path.Combine(ViewDirectory, subViewName + _viewEngine.Extension);
        }
        /// <summary>
        /// Gets a parameter's value from the view's propery containers.
        /// will throw exception if the parameter is not found
        /// </summary>
        /// <param name="parameterName">Parameter's name to look for</param>
        /// <returns>The parametr's value</returns>
        protected object GetParameter(string parameterName)
        {
            object value;
            if (!TryGetParameter(parameterName, out value))
                throw new RailsException("Parameter '" + parameterName + "' was not found!");
            return value;
        }
        /// <summary>
        /// Gets a parameter's value from the view's propery containers.
        /// will return a default value if the parameter is not found
        /// </summary>
        /// <param name="parameterName">Parameter's name to look for</param>
        /// <param name="defaultValue">The value to use if the parameter will not be found</param>
        /// <returns>The parametr's value</returns>
        protected object GetParameter(string parameterName, object defaultValue)
        {
            object value;
            if (!TryGetParameter(parameterName, out value))
                value = defaultValue;
            return value;
        }
        /// <summary>
        /// Actually looking in the property containers for a parameter's value given it's name
        /// </summary>
        /// <param name="parameterName">The parameter's name</param>
        /// <param name="parameter">The parameter's value</param>
        /// <returns>True if the property is found, False elsewhere</returns>
        protected bool TryGetParameter(string parameterName, out object parameter)
        {
            if (_properties.ContainsKey(parameterName))
            {
                parameter = _properties[parameterName];
                return true;
            }
            
            foreach (IDictionary dic in _extentedPropertiesList)
                if (dic.Contains(parameterName))
                {
                    parameter = dic[parameterName];
                    return true;
                }
			if (ParentView != null)
				return ParentView.TryGetParameter(parameterName, out parameter);
            parameter = null;
            return false;
        }
        /// <summary>
        /// Adds a property container to the _extentedPropertiesList
        /// </summary>
        /// <param name="properties">A property container </param>
        protected void AddProperties(IDictionary properties)
        {
            _extentedPropertiesList.Add(properties);
        }

        protected abstract string ViewDirectory { get; }
        protected abstract string ViewName { get; }
        /// <summary>
        /// Sets a view's parent view. Used in layouts
        /// </summary>
        /// <param name="view">The view's parent</param>
        internal void SetParent(AspViewBase view)
        {
            _parentView = view;
        }
		/// <summary>
		/// This is required because we may want to replace the output stream and get the correct
		/// behavior from components call RenderText() or RenderSection()
		/// </summary>
		public IDisposable SetOutputWriter(TextWriter newOutputWriter)
		{
			ReturnOutputStreamToInitialWriter disposable = new ReturnOutputStreamToInitialWriter(OutputWriter, this);
			_outputWriter = newOutputWriter;
			return disposable;
		}

		private class ReturnOutputStreamToInitialWriter : IDisposable
		{
			private TextWriter initialWriter;
			private AspViewBase parent;

			public ReturnOutputStreamToInitialWriter(TextWriter initialWriter, AspViewBase parent)
			{
				this.initialWriter = initialWriter;
				this.parent = parent;
			}

			public void Dispose()
			{
				parent._outputWriter = initialWriter;
			}
		}
	}
}