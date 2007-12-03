#region license
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
#endregion

using System.Collections.Generic;
using Castle.MonoRail.Views.AspView.Internal;

namespace Castle.MonoRail.Views.AspView.Compiler
{
	public class SourceFile
	{
		private string viewName;
		private string viewSource;
		private string className;
		private string baseClassName;
		private string typedViewName;
		private string concreteClass;
		private readonly StringSet imports = new StringSet();
		private readonly IDictionary<string, ViewProperty> properties = new Dictionary<string, ViewProperty>();
		private readonly IDictionary<string, string> viewComponentSectionHandlers = new Dictionary<string, string>();
		
		public string ViewName
		{
			get { return viewName; }
			set { viewName = value; }
		}

		public string ViewSource
		{
			get { return viewSource; }
			set { viewSource = value; }
		}

		public string ClassName
		{
			get { return className; }
			set { className = value; }
		}

		public string BaseClassName
		{
			get { return baseClassName; }
			set { baseClassName = value; }
		}

		public string TypedViewName
		{
			get { return typedViewName; }
			set { typedViewName = value; }
		}

		public string ConcreteClass
		{
			get { return concreteClass; }
			set { concreteClass = value; }
		}

		public string FileName
		{
			get { return ClassName + ".cs"; }
		}

		public StringSet Imports
		{
			get { return imports; }
		}

		public IDictionary<string, ViewProperty> Properties
		{
			get { return properties; }
		}

		public IDictionary<string, string> ViewComponentSectionHandlers
		{
			get { return viewComponentSectionHandlers; }
		}
		
	}
}
