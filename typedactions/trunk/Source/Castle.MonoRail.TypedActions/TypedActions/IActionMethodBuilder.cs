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
	using System.Linq.Expressions;
	using Framework;

	/// <summary>
	/// Converts a Linq Expression into a <see cref="ActionMethodModel"/>, 
	/// using the expression as the source for the controller name, action name
	/// and action parameters.
	/// <para>
	/// Usually uses a <see cref="IActionParametersBuilder"/> to convert the parameters
	/// of the expression into usable query parameters.
	/// </para>
	/// </summary>
	public interface IActionMethodBuilder
	{
		ActionMethodModel BuildAction<TController>(Expression<Action<TController>> expression)
			where TController : IController;
	}
}