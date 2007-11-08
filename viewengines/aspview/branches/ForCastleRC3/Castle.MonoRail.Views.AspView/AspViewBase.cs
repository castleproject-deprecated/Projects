// Copyright 2006-2007 Ken Egozi http://www.kenegozi.com/
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

using Castle.Components.DictionaryAdapter;

namespace Castle.MonoRail.Views.AspView
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Collections;

    using Framework;
    using Framework.Helpers;
    using System.Reflection;

    public abstract class AspViewBase
    {
        #region members
		private bool initialized = false;
		private TextWriter outputWriter;
		private TextWriter viewOutput;
		private Dictionary<string, object> properties;
		private IList<IDictionary> extentedPropertiesList;
		private IRailsEngineContext context;
		private Controller controller;
		private AspViewEngine viewEngine;
		private AspViewBase parentView;
		protected IDictionaryAdapterFactory dictionaryAdapterFactory;
		private IHelpersAccesor helpers;
		/// <summary>
		/// Stack of writers, used as buffers for viewfilters
		/// </summary>
		private Stack<TextWriter> outputWriters;
		/// <summary>
		/// Maintains the currently active view filters
		/// </summary>
		private Stack<IViewFilter> viewFilters;

        #endregion
        #region props

		/// <summary>
		/// Gets the builtin helpers
		/// </summary>
		protected IHelpersAccesor Helpers
		{
			get
			{
				if (helpers == null)
					helpers = dictionaryAdapterFactory.GetAdapter<IHelpersAccesor>(Properties);
				return helpers;
			}
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

		#region obsolete helper accessors
		/// <summary>
		/// Gets the AjaxHelper instance
		/// </summary>
		[Obsolete("Use Helpers.Ajax instead")]
		protected AjaxHelper AjaxHelper
		{
			get { return (AjaxHelper)Properties["AjaxHelper"]; }
		}
		
		/// <summary>
        /// Gets the DictHelper instance
        /// </summary>
		[Obsolete("Use Helpers.Dict instead")]
		protected DictHelper DictHelper 
        {
            get { return (DictHelper)Properties["DictHelper"]; }
        }

		/// <summary>
		/// Gets the ScriptaculousHelper instance
        /// </summary>
		[Obsolete("Use Helpers.Scriptaculous instead")]
		protected ScriptaculousHelper ScriptaculousHelper
        {
            get { return (ScriptaculousHelper)Properties["ScriptaculousHelper"]; }
        }

		/// <summary>
        /// Gets the EffectsFatHelper instance
        /// </summary>
		[Obsolete("Use Helpers.Effects instead")]
		protected EffectsFatHelper EffectsFatHelper 
        {
            get { return (EffectsFatHelper)Properties["EffectsFatHelper"]; }
        }

		/// <summary>
        /// Gets the FormHelper instance
        /// </summary>
		[Obsolete("Use Helpers.Form instead")]
		protected FormHelper FormHelper 
        {
            get { return (FormHelper)Properties["FormHelper"]; }
        }

		/// <summary>
        /// Gets the HtmlHelper instance
        /// </summary>
		[Obsolete("Use Helpers.Html instead")]
		protected HtmlHelper HtmlHelper 
        {
            get { return (HtmlHelper)Properties["HtmlHelper"]; }
        }

		/// <summary>
        /// Gets the PaginationHelper instance
        /// </summary>
		[Obsolete("Use Helpers.Pagination instead")]
		protected PaginationHelper PaginationHelper 
        {
            get { return (PaginationHelper)Properties["PaginationHelper"]; }
        }

		/// <summary>
        /// Gets the ValidationHelper instance
        /// </summary>
		[Obsolete("Use Helpers.Validation instead")]
		protected ValidationHelper ValidationHelper 
        {
            get { return (ValidationHelper)Properties["ValidationHelper"]; }
        }

		/// <summary>
        /// Gets the WizardHelper instance
        /// </summary>
		[Obsolete("Use Helpers.Wizard instead")]
		protected WizardHelper WizardHelper 
        {
            get { return (WizardHelper)Properties["WizardHelper"]; }
        }
		#endregion

        /// <summary>
        /// Gets the output writer for the current view rendering
        /// </summary>
        public TextWriter OutputWriter
        {
            get { return outputWriter; }
        }
        /// <summary>
        /// Used only in layouts. Gets or sets the output writer for the view
        /// </summary>
        public TextWriter ViewOutput
        {
            get { return viewOutput; }
            set { viewOutput = value; }
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
            get { return properties; }
        }
        /// <summary>
        /// Gets the extended properties collection
        /// </summary>
        protected IList<IDictionary> ExtentedPropertiesList
        {
            get { return extentedPropertiesList; }
        }
        /// <summary>
        /// Gets the current Rails context
        /// </summary>
        public IRailsEngineContext Context
        {
            get { return context; }
        }
        /// <summary>
        /// Gets the calling controller
        /// </summary>
        public Controller Controller
        {
            get { return controller; }
        }
        /// <summary>
        /// Gets the view engine instance
        /// </summary>
        public AspViewEngine ViewEngine
        {
            get { return viewEngine; }
        }
        /// <summary>
        /// Gets a reference to the view's parent view
        /// </summary>
        public AspViewBase ParentView
        {
            get { return parentView; }
        }
        #endregion

		public virtual void Initialize(AspViewEngine newViewEngine, TextWriter output, IRailsEngineContext newContext, Controller newController)
		{
			if (initialized)
				throw new ApplicationException("Sorry, but a view instance cannot be initialized twice");
			initialized = true;
			viewEngine = newViewEngine;
			outputWriter = output;
			context = newContext;
			controller = newController;
			InitProperties();
			dictionaryAdapterFactory = new DictionaryAdapterFactory();
			outputWriters = new Stack<TextWriter>();
			viewFilters = new Stack<IViewFilter>();
		}

        private void InitProperties()
        {
            properties = new Dictionary<string, object>(CaseInsensitiveStringComparer.Default);
            properties.Add("context", context);
            properties.Add("request", context.Request);
            properties.Add("response", context.Response);
            properties.Add("session", context.Session);
            properties.Add("controller", controller);
            if (controller.Resources != null)
                foreach (string key in controller.Resources.Keys)
                    if (key != null)
                        properties[key] = controller.Resources[key];
			if (controller.Helpers != null)
				foreach (string key in controller.Helpers.Keys)
					if (key != null)
						properties[key] = controller.Helpers[key];
			if (controller.Params != null)
				foreach (string key in context.Params.Keys)
					if (key != null)
						properties[key] = context.Params[key];
			if (context.Flash != null)
				foreach (DictionaryEntry entry in context.Flash)
					properties[entry.Key.ToString()] = entry.Value;
			if (controller.PropertyBag != null)
				foreach (DictionaryEntry entry in controller.PropertyBag)
					properties[entry.Key.ToString()] = entry.Value;
            properties["siteRoot"] = context.ApplicationPath ?? string.Empty;
            properties["fullSiteRoot"] = context.Request.Uri.GetLeftPart(UriPartial.Authority) + context.ApplicationPath;
            extentedPropertiesList = new List<IDictionary>();
        }
        
		/// <summary>
        /// When overriden in a concrete view class, renders the view content to the output writer
        /// </summary>
        public abstract void Render();

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
        protected void OutputSubView(string subViewName, IDictionary parameters)
        {
			OutputSubView(subViewName, outputWriter, parameters);
        }

		/// <summary>
		/// Renders another view in place
		/// </summary>
		/// <param name="subViewName">The sub view's name</param>
		/// <param name="parameters">Parameters that can be sent to the sub view's Properties container</param>
		/// <param name="writer">The writer that will be used for the sub view's output</param>
		protected void OutputSubView(string subViewName, TextWriter writer, IDictionary parameters)
		{
			string subViewFileName = GetSubViewFileName(subViewName);
			AspViewBase subView = viewEngine.GetView(subViewFileName, writer, context, controller);
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
		/// <param name="arguments">Parameters that can be sent to the sub view's Properties container</param>
		protected void OutputSubView(string subViewName, params object[] arguments)
		{
			OutputSubView(subViewName, outputWriter, arguments);
		}
		
		/// <summary>
        /// Renders another view in place
        /// </summary>
        /// <param name="subViewName">The sub view's name</param>
		/// <param name="writer">The writer that will be used for the sub view's output</param>
		/// <param name="arguments">Parameters that can be sent to the sub view's ordered as name, value</param>
		internal void OutputSubView(string subViewName, TextWriter writer, params object[] arguments)
        {
			IDictionary parameters =
				Utilities.ConvertArgumentsToParameters(arguments) as IDictionary;
            OutputSubView(subViewName, writer, parameters);
        }

		/// <summary>
        /// Searching a view filter given it's type's name
        /// </summary>
        /// <param name="filterName">the filter's typeName</param>
        /// <returns>System.Type of the filter</returns>
		private static Type GetFilterType(string filterName)
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
            outputWriters.Push(outputWriter);
            outputWriter = new StringWriter();
            viewFilters.Push(filter);
        }

        /// <summary>
        /// Signals the current view to apply the last view filter that was started on the buffered output
        /// </summary>
        protected void EndFiltering()
        {
            string original = outputWriter.ToString();
            IViewFilter filter = viewFilters.Pop();
            string filtered = filter.ApplyOn(original);
            outputWriter.Dispose();
            outputWriter = outputWriters.Pop();
            outputWriter.Write(filtered);
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
                return subViewName + "." + viewEngine.ViewFileExtension;
            else
                return Path.Combine(ViewDirectory, subViewName + "." + viewEngine.ViewFileExtension);
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
            if (!TryGetParameter(parameterName, out value, null))
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
			TryGetParameter(parameterName, out value, defaultValue);
            return value;
        }

        /// <summary>
        /// Actually looking in the property containers for a parameter's value given it's name
        /// </summary>
        /// <param name="parameterName">The parameter's name</param>
		/// <param name="parameter">The parameter's value</param>
		/// <param name="defaultValue">The value to use if <paramref name="parameterName"/> wasn't found in the controller's properties</param>
		/// <returns>True if the property is found, False elsewhere</returns>
        protected bool TryGetParameter(string parameterName, out object parameter, object defaultValue)
        {
            if (properties.ContainsKey(parameterName))
            {
                parameter = properties[parameterName];
                return true;
            }
            
            foreach (IDictionary dic in extentedPropertiesList)
                if (dic.Contains(parameterName))
                {
                    parameter = dic[parameterName];
                    return true;
                }
			if (ParentView != null)
				return ParentView.TryGetParameter(parameterName, out parameter, defaultValue);
            parameter = defaultValue;
            return false;
        }

        /// <summary>
        /// Adds a property container to the extentedPropertiesList
        /// </summary>
		/// <param name="newProperties">A property container </param>
        protected void AddProperties(IDictionary newProperties)
        {
			extentedPropertiesList.Add(newProperties);
        }

        protected abstract string ViewDirectory { get; }
        protected abstract string ViewName { get; }

		/// <summary>
        /// Sets a view's parent view. Used in layouts
        /// </summary>
        /// <param name="view">The view's parent</param>
        internal void SetParent(AspViewBase view)
        {
            parentView = view;
        }

		/// <summary>
		/// This is required because we may want to replace the output stream and get the correct
		/// behavior from components call RenderText() or RenderSection()
		/// </summary>
		public IDisposable SetOutputWriter(TextWriter newOutputWriter)
		{
			ReturnOutputStreamToInitialWriter disposable = new ReturnOutputStreamToInitialWriter(OutputWriter, this);
			outputWriter = newOutputWriter;
			return disposable;
		}

		private class ReturnOutputStreamToInitialWriter : IDisposable
		{
			readonly TextWriter initialWriter;
			readonly AspViewBase parent;

			public ReturnOutputStreamToInitialWriter(TextWriter initialWriter, AspViewBase parent)
			{
				this.initialWriter = initialWriter;
				this.parent = parent;
			}

			public void Dispose()
			{
				parent.outputWriter = initialWriter;
			}
		}
	}
}