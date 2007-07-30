using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

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
