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

	using System.Runtime.Serialization;
	using System.Text;
	using Castle.Components.Validator;


	public class MethodValidationException : ValidationException
	{

		private readonly ErrorSummary errors;

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodValidationException"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="errors">The errors.</param>
		public MethodValidationException(string message, ErrorSummary errors) : base(message, errors.ErrorMessages)
		{
			this.errors = errors;
		}

		/// <summary>
		/// Gets the errors.
		/// </summary>
		/// <value>The errors.</value>
		public ErrorSummary Errors
		{
			get { return errors; }
		}
	}
}
