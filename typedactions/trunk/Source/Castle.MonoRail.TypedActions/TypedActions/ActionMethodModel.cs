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
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using Castle.MonoRail.Framework.Descriptors;

	/// <summary>
	/// Represents a strongly typed action.
	/// <para>
	/// The query parameters of the action are stored in the <see cref="Parameters"/> collection.
	/// </para>
	/// </summary>
	public class ActionMethodModel
	{
		private readonly MethodInfo _method;
		private readonly ControllerDescriptor _descriptor;
		private IEnumerable<ActionParameterModel> _parameters;

		public ActionMethodModel(ControllerDescriptor descriptor, MethodInfo method)
		{
			_descriptor = descriptor;
			_method = method;
		}

		public virtual MethodInfo MethodInfo
		{
			get { return _method; }
		}

		public virtual IEnumerable<ActionParameterModel> Parameters
		{
			get { return _parameters; }
		}

		public virtual ControllerDescriptor Descriptor
		{
			get { return _descriptor; }
		}

		public string Area
		{
			get { return Descriptor.Area ?? string.Empty; }
		}

		public string Controller
		{
			get { return Descriptor.Name; }
		}

		public string Action
		{
			get { return MethodInfo.Name; }
		}

		public virtual void SetParameters(IEnumerable<ActionParameterModel> parameters)
		{
			_parameters = parameters ?? new List<ActionParameterModel>();
		}

		/// <summary>
		/// Converts the <see cref="Parameters"/> into a parameter dictionary,
		/// with the parameter name as key, and the parameter value.
		/// <para>
		/// This is useful for redirects for example, or for use with <see cref="Castle.MonoRail.Framework.Helpers.UrlHelper"/>.
		/// </para>
		/// </summary>
		/// <returns></returns>
		public virtual IDictionary GetParameterDictionary()
		{
			return Parameters.ToDictionary(p => p.Name, p => p.Value);
		}
	}
}