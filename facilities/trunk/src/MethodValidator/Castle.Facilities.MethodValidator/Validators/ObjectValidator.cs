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

namespace Castle.Facilities.MethodValidator.Validators
{

	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using System.Text;
	using Castle.Components.Validator;

	public class ObjectValidator : AbstractValidator
	{

		private ParameterValidatorRegistryAdapter validationRegistry;
		private ErrorSummary errorSummary;

		/// <errorSummary>
		/// Gets the error errorSummary.
		/// </errorSummary>
		/// <value>The errorSummary.</value>
		public ErrorSummary ErrorSummary
		{
			get { return errorSummary; }
		}

		/// <errorSummary>
		/// Initializes the specified validation registry.
		/// </errorSummary>
		/// <param name="registry">The validation registry.</param>
		/// <param name="property">The property.</param>
		public override void Initialize(IValidatorRegistry registry, PropertyInfo property)
		{
			ParameterValidatorRegistryAdapter adapter = registry as ParameterValidatorRegistryAdapter;
			if (adapter == null)
				throw new ValidationInternalError("Expected IValidationRegistry of type ParameterValidatorRegistryAdapter.");

			validationRegistry = adapter;
		}

		/// <errorSummary>
		/// Determines whether the specified instance is valid.
		/// </errorSummary>
		/// <param name="instance">The instance.</param>
		/// <param name="fieldValue">The field value.</param>
		/// <returns>
		/// 	<c>true</c> if the specified instance is valid; otherwise, <c>false</c>.
		/// </returns>
		public override bool IsValid(object instance, object fieldValue)
		{
			ValidatorRunner runner = new ValidatorRunner(validationRegistry.BaseRegistry);
			bool valid =  runner.IsValid(instance);

			if (!valid)
				errorSummary = runner.GetErrorSummary(instance);

			return valid;
		}

		/// <errorSummary>
		/// Gets a value indicating whether the validator supports browser validation.
		/// </errorSummary>
		/// <value>
		/// 	<c>true</c> if the validator supports browser validation; otherwise, <c>false</c>.
		/// </value>
		public override bool SupportsBrowserValidation
		{
			get { return false; }
		}
	}
}
