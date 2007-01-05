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

    public abstract class AspViewBase
    {
        #region members
        protected TextWriter _outputStream;
        protected TextWriter _viewOutput;
        protected Dictionary<string, object> _properties;
        protected IList<IDictionary<string, object>> _extentedPropertiesList;
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
        /// Gets the DictHelper instance
        /// </summary>
        protected DictHelper DictHelper 
        {
            get { return (DictHelper)Properties["DictHelper"]; }
        }
        /// <summary>
        /// Gets the Effects2Helper instance
        /// </summary>
        protected Effects2Helper Effects2Helper 
        {
            get { return (Effects2Helper)Properties["Effects2Helper"]; }
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
        public TextWriter OutputStream
        {
            get { return _outputStream; }
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
        protected IList<IDictionary<string, object>> ExtentedPropertiesList
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
            _outputStream = output;
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
            _extentedPropertiesList = new List<IDictionary<string, object>>();
        }
        /// <summary>
        /// When overriden in a concrete view class, renders the view content to the output writer
        /// </summary>
        public abstract void Render();
        protected virtual string ScriptDirectory
        {
            get { return _viewEngine.ViewRootDir; }
        }
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
            string subViewFileName = GetSubViewFileName(subViewName);
            AspViewBase subView = _viewEngine.GetView(subViewFileName, _outputStream, _context, _controller);
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
        /// <param name="parameters">Parameters that can be sent to the sub view's ordered as name, value</param>
        protected void OutputSubView(string subViewName, params object[] arguments)
        {
            if (arguments.Length % 2 != 0)
                throw new RailsException("SubView parameters should be arranged as key and value pairs");
            int i = 0;
            IDictionary<string, object> parameters = new Dictionary<string, object>(arguments.Length / 2);
            while (i < arguments.Length)
            {
                string name = arguments[i] as string;
                if (name==null)
                    throw new RailsException("SubView parameters should be arranged as key and value pairs");
                object key = arguments[i + 1];
                parameters.Add(name, key);
                i += 2;
            }
            OutputSubView(subViewName, parameters);
        }

        protected void Output(string message)
        {
            OutputStream.Write(message);
        }
        protected void Output(string message, params object[] arguments)
        {
            Output(string.Format(message, arguments));
        }
        protected void Output(object message)
        {
            Output(message.ToString());
        }

        private string GetSubViewFileName(string subViewName)
        {
            if (subViewName[0] == '/' || subViewName[0] == '\\')
                return subViewName + _viewEngine.Extension;
            else
                return Path.Combine(ViewDirectory, subViewName + _viewEngine.Extension);
        }

        protected object GetParameter(string parameterName)
        {
            object value = GetParameterInternal(parameterName);
            if (value == null)
                throw new RailsException("Parameter '" + parameterName + "' was not found!");
            return value;
        }
        protected bool TryGetParameter(string parameterName, out object parameter)
        {
            parameter = GetParameterInternal(parameterName);
            return parameter != null;
        }
        protected object GetParameterInternal(string propertyName)
        {
            object value = null;
            if (_properties.ContainsKey(propertyName))
                value = _properties[propertyName];
            if (value == null)
            {
                foreach (IDictionary<string, object> dic in _extentedPropertiesList)
                    if (dic.ContainsKey(propertyName))
                    {
                        value = dic[propertyName];
                        break;
                    }
            }
            return value;
        }
        protected void AddProperties(IDictionary<string, object> properties)
        {
            _extentedPropertiesList.Add(properties);
        }

        protected abstract string ViewDirectory { get; }
        protected abstract string ViewName { get; }

        internal void SetParent(AspViewBase view)
        {
            _parentView = view;
        }
    }
}
