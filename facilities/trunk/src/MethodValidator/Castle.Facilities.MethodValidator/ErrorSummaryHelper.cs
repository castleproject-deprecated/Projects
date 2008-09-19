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

namespace Castle.Facilities.MethodValidator
{

	using System;
	using System.Text;
	using Castle.Components.Validator;

	public static class ErrorSummaryHelper
	{

		/// <summary>
		/// Creates a string representation of the error summary.
		/// </summary>
		/// <param name="errors">The errors.</param>
		/// <returns></returns>
		public static string  CreateSummary(ErrorSummary errors)
		{
			StringBuilder builder = new StringBuilder();
			foreach (string property in errors.InvalidProperties)
			{
				builder.Append("    - ").Append(property).Append(": ");
				builder.Append(string.Join(",", errors.GetErrorsForProperty(property)));
				builder.Append(Environment.NewLine);
			}
			return builder.ToString();
		}

	}
}
