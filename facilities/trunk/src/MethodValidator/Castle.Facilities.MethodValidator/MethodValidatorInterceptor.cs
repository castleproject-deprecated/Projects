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
	using System.Reflection;
	using System.Text;
	using Castle.Components.Validator;
	using Castle.Core.Interceptor;

	public class MethodValidatorInterceptor : StandardInterceptor
	{
		private readonly MethodValidationContributor contributor;

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodValidatorInterceptor"/> class.
		/// </summary>
		/// <param name="contributor">The contributor.</param>
		public MethodValidatorInterceptor(MethodValidationContributor contributor)
		{
			this.contributor = contributor;
		}

		/// <summary>
		/// Invoked before the interceptor proceeds with the method in
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		protected override void PreProceed(IInvocation invocation)
		{
			MethodInfo method = invocation.Method;

			IValidatorRunner runner = CreateRunner();

			bool isValid = runner.IsValid(invocation);

			if (!isValid)
			{
				ErrorSummary errors = runner.GetErrorSummary(invocation);
				OnInvalidMethod(method, errors);
			}
		}

		/// <summary>
		/// Creates the validator runner.
		/// </summary>
		/// <returns></returns>
		protected virtual IValidatorRunner CreateRunner()
		{
			IValidationContributor[] contributors = new IValidationContributor[] { contributor };
			IValidatorRunner runner = new ValidatorRunner(contributors, new CachedValidationRegistry());
			return runner;
		}

		/// <summary>
		/// Called when the validator finds an invalid method.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="errors">The errors.</param>
		protected virtual void OnInvalidMethod(MethodInfo method, ErrorSummary errors)
		{
			string errorMessage = BuildErrorMessage(method, errors);
			throw new ValidationException(errorMessage, errors.ErrorMessages);
		}

		/// <summary>
		/// Builds the error message to throw when there are validator errors.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="errors">The errors.</param>
		/// <returns></returns>
		protected virtual string BuildErrorMessage(MethodInfo method, ErrorSummary errors)
		{
			StringBuilder builder = new StringBuilder();
			builder.Append(string.Format("Validation failure will invoking method: {0}", method)).Append(Environment.NewLine);
			builder.Append(ErrorSummaryHelper.CreateSummary(errors));
			return builder.ToString();
		}
	}
}
