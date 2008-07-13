#region Licence

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

#endregion

namespace Castle.MonoRail.TypedActions
{
	using System.Reflection;

	/// <summary>
	/// Represents a parameter of a <see cref="ActionMethodModel"/>.
	/// </summary>
	public class ActionParameterModel
	{
		private readonly ParameterInfo _paramInfo;
		private string _name;
		private object _value;

		public ActionParameterModel(ParameterInfo paramInfo)
		{
			_paramInfo = paramInfo;
		}

		public ActionParameterModel(ParameterInfo paramInfo, object value)
			: this(paramInfo)
		{
			_value = value;
		}

		public ActionParameterModel(ParameterInfo paramInfo, string name, object value)
			: this(paramInfo, value)
		{
			_name = name;
		}

		public virtual ParameterInfo ParamInfo
		{
			get { return _paramInfo; }
		}

		public virtual string Name
		{
			get { return _name ?? ParamInfo.Name; }
			set { _name = value; }
		}

		public virtual object Value
		{
			get { return _value; }
			set { _value = value; }
		}
	}
}