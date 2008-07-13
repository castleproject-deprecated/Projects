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
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	/// <summary>
	/// Abstract implementation of <see cref="IActionParametersBuilder"/> with a decorator pattern.
	/// </summary>
	public abstract class DecoratingActionParametersBuilder : IActionParametersBuilder
	{
		protected DecoratingActionParametersBuilder(IActionParametersBuilder innerBuilder)
		{
			InnerBuilder = innerBuilder;
		}

		public IActionParametersBuilder InnerBuilder { get; set; }

		public virtual IEnumerable<ActionParameterModel> BuildParameters(IDictionary<ParameterInfo, object> givenValues)
		{
			if (InnerBuilder == null)
			{
				throw new InvalidOperationException("InnerBuilder can not be null.");
			}

			var innerParams = InnerBuilder.BuildParameters(givenValues);
			var decoratedParams = new List<ActionParameterModel>();

			foreach(var innerParam in innerParams)
			{
				var decorated = Decorate(innerParam);
				if (decorated != null)
				{
					decoratedParams.AddRange(decorated);
				}
			}

			return decoratedParams;
		}

		/// <summary>
		/// Decorates the action parameter. 
		/// This way you can remove or change the existing parameter, or return one or more new parameters.
		/// </summary>
		/// <param name="paramToDecorate">The param to decorate.</param>
		/// <returns></returns>
		protected abstract IEnumerable<ActionParameterModel> Decorate(ActionParameterModel paramToDecorate);
	}
}