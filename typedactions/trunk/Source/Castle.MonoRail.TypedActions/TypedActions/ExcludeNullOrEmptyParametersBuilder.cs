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
	using System.Collections;
	using System.Collections.Generic;

	/// <summary>
	/// Inspects the action parameters created by <see cref="DecoratingActionParametersBuilder.InnerBuilder"/>, 
	/// and only let a parameter pass through if its value is not null, 
	/// and it is not a empty string or collection.
	/// </summary>
	public class ExcludeNullOrEmptyParametersBuilder : DecoratingActionParametersBuilder
	{
		public ExcludeNullOrEmptyParametersBuilder(IActionParametersBuilder innerBuilder)
			: base(innerBuilder)
		{
		}

		protected override IEnumerable<ActionParameterModel> Decorate(ActionParameterModel paramToDecorate)
		{
			bool isValid = true;
			if (paramToDecorate.Value == null)
			{
				isValid = false;
			}

			string stringValue = paramToDecorate.Value as string;
			if (stringValue != null && stringValue == string.Empty)
			{
				isValid = false;
			}

			IEnumerable enumValue = paramToDecorate.Value as IEnumerable;
			if (enumValue != null)
			{
				IEnumerator enumerator = null;
				try
				{
					enumerator = enumValue.GetEnumerator();
					isValid = enumerator.MoveNext();
				}
				finally
				{
					var disposable = enumerator as IDisposable;
					if (disposable != null)
					{
						disposable.Dispose();
					}
				}
			}

			if (isValid)
			{
				yield return paramToDecorate;
			}
			yield break;
		}
	}
}