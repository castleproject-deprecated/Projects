// Copyright 2004-2008 Castle Project - http://www.castleproject.org/
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

namespace Castle.MonoRail.Framework.View.Xslt
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Diagnostics;
	
	/// <summary>
	/// Class responsible for always providing 
	/// the most recent version of a certain stylesheet.
	/// </summary>
	public class XsltTemplateStore
	{
		private IXsltEngine _xsltEngine;
		private Dictionary<string, IXsltTransform> _templateCache = new Dictionary<string, IXsltTransform>();
		private Dictionary<string, List<string>> _templateRelations = new Dictionary<string, List<string>>();
		
		private object _lock = new object();
		private IViewSourceLoader _viewSourceLoader;

		internal IViewSourceLoader ViewSourceLoader
		{
			get
			{
				return _viewSourceLoader;
			}
			set
			{
				_viewSourceLoader = value;
			}
		}

		public XsltTemplateStore(IViewSourceLoader viewSourceLoader, IXsltEngine xsltEngine)
		{
			ViewSourceLoader = viewSourceLoader;
			ViewSourceLoader.ViewChanged += new FileSystemEventHandler(_viewSourceLoader_ViewChanged);
			_xsltEngine = xsltEngine;
		}

		void _viewSourceLoader_ViewChanged(object sender, FileSystemEventArgs e)
		{
			string templateKey = e.Name.Replace(".xslt", string.Empty).ToLower();
			Debug.WriteLine(templateKey + " changed");
			
			lock (_lock)
			{
				//Uncache all templates that have a dependency on the change template
				if (_templateRelations.ContainsKey(templateKey))
				{
					foreach (string dependency in _templateRelations[templateKey])
					{
						_templateCache.Remove(dependency);
					}
					_templateRelations.Remove(templateKey);
				}

				//Uncache the template itself
				if (_templateCache.ContainsKey(templateKey))
				{
					_templateCache.Remove(templateKey);
				}
			}
		}


		private Stream GetTemplateStream(String templateName)
		{

			return ViewSourceLoader.GetViewSource(ResolveTemplateName(templateName)).OpenViewStream();

		}


		private IXsltTransform CompileTransform(string templateName, XsltTransformationArguments arguments)
		{
			using (Stream templateStream = GetTemplateStream(templateName))
			{
				return CompileTransform(new XsltTemplateStoreBasedXsltTemplateResolver(this, templateName), templateStream, arguments);
			}
		}

		private IXsltTransform CompileTransform(IXsltTemplateResolver resolver, Stream transformStream, XsltTransformationArguments arguments)
		{
			return _xsltEngine.LoadTransform(resolver, transformStream, arguments);
		}

		public IXsltTransform LoadTemplate(string templateName, XsltTransformationArguments arguments)
		{
			string key = templateName.ToLower();

			lock (_lock)
			{
				if (_templateCache.ContainsKey(key))
					return _templateCache[key];

				IXsltTransform transform = CompileTransform(key, arguments);

				_templateCache[key] = transform;

				return transform;
			}
		}

		internal void AddDependency(string reference, string dependency)
		{
			lock (_lock)
			{
				List<string> dependencies;
				if (!_templateRelations.TryGetValue(reference, out dependencies))
				{
					dependencies = new List<string>();
					_templateRelations.Add(reference, dependencies);
				}
				dependencies.Add(dependency);
			}
		}

		void watcher_Changed(object sender, FileSystemEventArgs e)
		{
			lock (_lock)
			{
				if (_templateCache.ContainsKey(e.FullPath))
				{
					_templateCache.Remove(e.FullPath);
				}
			}
		}

		protected virtual String ResolveTemplateName(string templateName)
		{
			return templateName + ".xslt";
		}

		/// <summary>
		/// Evaluates whether the specified template exists.
		/// </summary>
		/// <returns><c>true</c> if it exists</returns>
		public bool HasTemplate(string templateName)
		{
			return ViewSourceLoader.HasTemplate(ResolveTemplateName(templateName));
		}
	}
}
