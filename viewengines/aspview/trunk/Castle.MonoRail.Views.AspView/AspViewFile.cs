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
	public class AspViewFile
	{
		private string _viewName;
		private string _viewSource;
		private string _className;
		private ScriptingLanguage _language;
		private string _concreteClass;

		public string ViewName
		{
			get { return _viewName; }
			set { _viewName = value; }
		}

		public string ViewSource
		{
			get { return _viewSource; }
			set { _viewSource = value; }
		}

		public string ClassName
		{
			get { return _className; }
			set { _className = value; }
		}

		public ScriptingLanguage Language
		{
			get { return _language; }
			set { _language = value; }
		}

		public string ConcreteClass
		{
			get { return _concreteClass; }
			set { _concreteClass = value; }
		}

		public string FileName
		{
			get { return ClassName + AspViewPreProcessor.GetExtension(_language); }
		}
	}
}
