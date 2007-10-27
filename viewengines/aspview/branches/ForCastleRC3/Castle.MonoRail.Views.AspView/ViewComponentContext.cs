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

namespace Castle.MonoRail.Views.AspView
{
	using System.Collections;
	using Framework;
	using System.IO;

	public class ViewComponentContext : IViewComponentContext
	{
		readonly string componentName;

		readonly IDictionary componentParameters;
		IDictionary sections;
		string viewToRender;

		ViewComponentSectionRendereDelegate body;
		readonly TextWriter default_writer;
		readonly private AspViewBase parent;

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewComponentContext"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="body">The body.</param>
		/// <param name="name">The name.</param>
		/// <param name="writer">The text writer.</param>
		/// <param name="parameters">The parameters.</param>
		public ViewComponentContext(AspViewBase parent, ViewComponentSectionRendereDelegate body,
										 string name, TextWriter writer, IDictionary parameters)
		{
			this.parent = parent;
			this.body = body;
			componentName = name;
			default_writer = writer;
			componentParameters = parameters;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ViewComponentContext"/> class.
		/// </summary>
		/// <param name="parent">The parent.</param>
		/// <param name="body">The body.</param>
		/// <param name="name">The name.</param>
		/// <param name="writer">The text writer.</param>
		/// <param name="arguments">The arguments.</param>
		public ViewComponentContext(AspViewBase parent, ViewComponentSectionRendereDelegate body,
										 string name, TextWriter writer, params object[] arguments)
			: this(parent, body, name, writer, (IDictionary)Utilities.ConvertArgumentsToParameters(arguments)) { }

		public ViewComponentSectionRendereDelegate Body
		{
			get { return body; }
			set { body = value; }
		}

		public void RegisterSection(string name, object section)
		{
			if (sections == null)
				sections = new Hashtable();
			sections[name] = section;
		}

		#region IViewComponentContext Members

		public string ComponentName
		{
			get { return componentName; }
		}

		public IDictionary ComponentParameters
		{
			get { return componentParameters; }
		}

		public IDictionary ContextVars
		{
			get { return parent.Properties; }
		}

		public bool HasSection(string sectionName)
		{
			return sections != null && sections.Contains(sectionName);
		}

		public void RenderBody()
		{
			RenderBody(default_writer);
		}

		public void RenderBody(TextWriter writer)
		{
			if (body == null)
			{
				throw new RailsException("This component does not have a body content to be rendered");
			}
			using (parent.SetOutputWriter(writer))
			{
				body.Invoke();
			}
		}

		public void RenderSection(string sectionName)
		{
			RenderSection(sectionName, default_writer);
		}

		/// <summary>
		/// Renders the the specified section
		/// </summary>
		/// <param name="sectionName">Name of the section.</param>
		/// <param name="writer">The writer.</param>
		public void RenderSection(string sectionName, TextWriter writer)
		{
			if (!HasSection(sectionName))
				return;//matching the Brail and NVelocity behavior, but maybe should throw?
			ViewComponentSectionRendereDelegate section = (ViewComponentSectionRendereDelegate)sections[sectionName];
			section.Invoke();
		}

		public void RenderView(string name, TextWriter writer)
		{
			parent.OutputSubView(name, writer, new Hashtable());
		}

		public IViewEngine ViewEngine
		{
			get { return parent.ViewEngine; }
		}

		public string ViewToRender
		{
			get { return viewToRender; }
			set { viewToRender = value; }
		}

		public TextWriter Writer
		{
			get { return default_writer; }
		}

		#endregion
	}
}
