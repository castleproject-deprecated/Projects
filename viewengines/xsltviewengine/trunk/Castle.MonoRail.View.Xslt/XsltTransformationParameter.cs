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


	/// <summary>
	/// Class encapsulating a single transformation parameter
	/// </summary>
	public class XsltTransformationParameter
	{
		/// <summary>
		/// Initializes a new instance of the XsltTransformationParameter class.
		/// </summary>
		/// <param name="name"></param>
		/// <param name="namespaceUri"></param>
		/// <param name="value"></param>
		public XsltTransformationParameter(string name, string namespaceUri, object value)
		{
			_Name = name;
			_NameSpace = namespaceUri;
			_Value = value;
		}

		private string _Name;
		public string Name
		{
			get { return _Name; }
			private set
			{
				_Name = value;
			}
		}

		private string _NameSpace;
		public string NameSpace
		{
			get { return _NameSpace; }
			private set
			{
				_NameSpace = value;
			}
		}

		private object _Value;
		public object Value
		{
			get { return _Value; }
			private set
			{
				_Value = value;
			}
		}
	}
}
