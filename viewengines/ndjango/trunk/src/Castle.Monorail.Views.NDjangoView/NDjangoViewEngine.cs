// Copyright 2004-2009 Castle Project - http://www.castleproject.org/
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

namespace Castle.Monorail.Views.NDjangoView
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Text;
	using System.Web;
	using System.IO;
	using System.Collections;
	using System.Reflection;
	using Castle.MonoRail.Framework;
	using Castle.Core;
	using NDjango;
	using NDjango.Interfaces;
	

	/// <summary>
	/// Implements a view engine using NDjango rendering engine. 
	/// 
	/// For details - see http://www.ndjango.org
	/// </summary>
	public class NDjangoViewEngine : ViewEngineBase, IInitializable
	{
		/// <summary>
		/// Key to use in HttpApplicationState for storing ITemplateManager.
		/// </summary>
		private const String cDjangoManagerKey = "_djangoManagerKey";
		/// <summary>
		/// Template extension
		/// </summary>
		private const String cTemplateExtension = ".django";

		/// <summary>
		/// Template Manager Provider stored here.
		/// </summary>
		private TemplateManagerProvider managerProvider;

		#region IInitializable Members
		/// <summary>
		/// initializing managerProvider and loader.
		/// </summary>
		public void Initialize()
		{
			string path = typeof(TemplateManagerProvider).Assembly.CodeBase;
			List<Tag> tags = new List<Tag>();
			List<NDjango.Filter> filters = new List<NDjango.Filter>();
			// Searching for NDjango tags and filters in the dll's.
			if (path.StartsWith("file:///"))
			{
				foreach (string file in
					Directory.GetFiles(
						Path.GetDirectoryName(path.Substring(8)),
						"*.dll",
						SearchOption.AllDirectories))
				{
					AssemblyName name = new AssemblyName();
					name.CodeBase = file;
					try
					{
						foreach (Type t in Assembly.Load(name).GetExportedTypes())
						{
							if (typeof(ITag).IsAssignableFrom(t))
								CreateEntry<Tag>(tags, t);
							if (typeof(ISimpleFilter).IsAssignableFrom(t))
								CreateEntry<NDjango.Filter>(filters, t);
						}
					}
					// There may be unmanaged dll's, and in that case dll fails to load. 
					// We have no other option to check it
					catch
					{
					}
				}
			}

			managerProvider =
				new TemplateManagerProvider()
					.WithLoader(new TemplateLoader())
					.WithFilters(filters)
					.WithTags(tags)
					.WithTag("url", new MonorailUrlTag(HttpRuntime.AppDomainAppVirtualPath));
		}

		/// <summary>
		/// Creates the class instance and adds it to the list
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list.</param>
		/// <param name="t">The type to create instance of.</param>
		private void CreateEntry<T>(List<T> list, Type t) where T : class
		{
			if (t.IsAbstract)
				return;
			if (t.IsInterface)
				return;

			var attrs = t.GetCustomAttributes(typeof(NameAttribute), false) as NameAttribute[];
			if (attrs.Length == 0)
				return;

			if (t.GetConstructor(new Type[] { }) == null)
				return;

			list.Add((T)Activator.CreateInstance(typeof(T), attrs[0].Name, Activator.CreateInstance(t)));
		}

		#endregion




		#region IViewEngine implementation
		/// <summary>
		/// Not Implemented - Implementors should return a generator instance if
		/// the view engine supports JS generation.
		/// </summary>
		/// <param name="context">The request context.</param>
		/// <returns>A JS generator instance</returns>
		public override object CreateJSGenerator(IRailsEngineContext context)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not Implemented - Processes the js generation view template - using the templateName
		/// to obtain the correct template, and using the specified <see cref="T:System.IO.TextWriter"/>
		/// to output the result.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="templateName">Name of the template.</param>
		public override void GenerateJS(System.IO.TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not Implemented - Evaluates whether the specified template exists.
		/// </summary>
		/// <param name="templateName"></param>
		/// <returns><c>true</c> if it exists</returns>
		public override bool HasTemplate(string templateName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not Implemented - Gets the JS generator file extension.
		/// </summary>
		/// <value>The JS generator file extension.</value>
		public override string JSGeneratorFileExtension
		{
			get { throw new NotImplementedException(); }
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template
		/// and writes the results to the System.IO.TextWriter.
		/// </summary>
		/// <param name="output"></param>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="templateName"></param>
		public override void Process(System.IO.TextWriter output, IRailsEngineContext context, Controller controller, string templateName)
		{

			HttpApplicationState app = context.UnderlyingContext.Application;
			// If there's no manager - managerProvider will return new one for us.
			if (app[cDjangoManagerKey] == null)
			{
				// Since one HttpApplication processed by a single thread - we don't need no locking here.
				app[cDjangoManagerKey] = managerProvider.GetNewManager();
			}

			ITemplateManager mgr = app[cDjangoManagerKey] as ITemplateManager;
			if (mgr == null)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Couldn't get ITemplateManager from the HttpApplicationState");
				}

				throw new RailsException("Couldn't get ITemplateManager from the HttpApplicationState");
			}
			
			
			AdjustContentType(context);

			string resolvedName = Path.HasExtension(templateName) ? templateName : templateName + cTemplateExtension;

			try
			{
				var djangoContext = CreateContext(context, controller);

				TextReader reader = mgr.RenderTemplate(resolvedName, djangoContext);
				char[] buffer = new char[4096];
				int count = 0;
				while ((count = reader.ReadBlock(buffer, 0, 4096)) > 0)
					output.Write(buffer, 0, count);
			}
			catch (Exception ex)
			{
				if (Logger.IsErrorEnabled)
				{
					Logger.Error("Could not render view", ex);
				}

				throw new RailsException("Could not render view: " + resolvedName, ex);
			}
		}

		/// <summary>
		/// Processes the view - using the templateName
		/// to obtain the correct template,
		/// and using the context's response's output to output the result. 
		/// </summary>
		/// <param name="context"></param>
		/// <param name="controller"></param>
		/// <param name="templateName"></param>
		public override void Process(IRailsEngineContext context, Controller controller, string templateName)
		{
			Process(context.Response.Output, context, controller, templateName);
		}

		/// <summary>
		/// Not Implemented - Processes the contents.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="contents">The contents.</param>
		public override void ProcessContents(IRailsEngineContext context, Controller controller, string contents)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Not Implemented - Should process the specified partial. The partial name must contains
		/// the path relative to the views folder.
		/// </summary>
		/// <param name="output">The output.</param>
		/// <param name="context">The request context.</param>
		/// <param name="controller">The controller.</param>
		/// <param name="partialName">The partial name.</param>
		public override void ProcessPartial(System.IO.TextWriter output, IRailsEngineContext context, Controller controller, string partialName)
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a value indicating whether [supports JS generation].
		/// </summary>
		/// <value>
		/// 	<c>true</c> if [supports JS generation]; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsJSGeneration
		{
			get { return false; }
		}

		/// <summary>
		/// Gets the view file extension for ndjango templates.
		/// </summary>
		/// <value>The view file extension for ndjango templates.</value>
		public override string ViewFileExtension
		{
			get { return cTemplateExtension; }
		}
		#endregion

		#region Context creation
		/// <summary>
		/// Creates the context form the Rails Context and the controller.
		/// </summary>
		/// <param name="context">The context.</param>
		/// <param name="controller">The controller.</param>
		/// <returns></returns>
		private IDictionary<string, object> CreateContext(IRailsEngineContext context, Controller controller)
		{
			// Ndjango context is actually a dictionary with string keys.
			IDictionary<string, object> ndjangoContext = new Dictionary<string, object>();
			ndjangoContext.Add(TemplateKeys.Controller, controller);
			ndjangoContext.Add(TemplateKeys.Context, context);
			ndjangoContext.Add(TemplateKeys.Request, context.Request);
			ndjangoContext.Add(TemplateKeys.Response, context.Response);
			ndjangoContext.Add(TemplateKeys.Session, context.Session);

			// Drop in controller's resources
			if (controller.Resources != null)
			{
				foreach (String key in controller.Resources.Keys)
				{
					ndjangoContext[key] = controller.Resources[key];
				}
			}

			// All context parameters
			foreach (String key in context.Params.AllKeys)
			{
				if (key == null) continue;
				object value = context.Params[key];
			}

			// And finally - the property bag of the controller
			if (controller.PropertyBag != null)
			{
				foreach (DictionaryEntry entry in controller.PropertyBag)
				{
					String entryKey = entry.Key as string;
					if (entryKey == null) continue;
					ndjangoContext[entryKey] = entry.Value;
				}
			}

			return ndjangoContext;

		}

		#endregion

	}
}
