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

using Castle.Facilities.MethodValidator.Validators;

namespace Castle.Facilities.MethodValidator
{

	using System;
	using System.Reflection;
	using Castle.Components.Validator;
	using Castle.Core.Interceptor;

	public class MethodValidationContributor : IValidationContributor
	{
		private readonly MethodValidatorMetaStore methodValidatorMetaStore;

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodValidationContributor"/> class.
		/// </summary>
		/// <param name="methodValidatorMetaStore">The method validator meta store.</param>
		public MethodValidationContributor(MethodValidatorMetaStore methodValidatorMetaStore)
		{
			this.methodValidatorMetaStore = methodValidatorMetaStore;
		}

		/// <summary>
		/// Determines whether the specified instance is valid.
		/// </summary>
		/// <param name="instance">The instance.</param>
		/// <param name="runWhen">The run when.</param>
		/// <returns></returns>
		public ErrorSummary IsValid(object instance, RunWhen runWhen)
		{
			ErrorSummary errors = new ErrorSummary();

			IInvocation invocation = instance as IInvocation;

			if (invocation == null)
				return errors;

			for (int i = 0; i < invocation.Arguments.Length; i++)
			{
				ValidateParameter(invocation, i, runWhen, errors);
			}

			return errors;
		}

		/// <summary>
		/// Validates the parameter.
		/// </summary>
		/// <param name="invocation">The invocation.</param>
		/// <param name="parameterPosition">The parameter position.</param>
		/// <param name="runWhen">The run when.</param>
		/// <param name="errors">The errors.</param>
		protected virtual void ValidateParameter(IInvocation invocation, int parameterPosition, RunWhen runWhen, ErrorSummary errors)
		{
			MethodInfo method = invocation.Method;


			ParameterInfoMeta parameterInfoMeta;
			IValidator[] validators = methodValidatorMetaStore.GetValidatorsFor(method, parameterPosition, null, runWhen, out parameterInfoMeta);

			foreach (IValidator validator in validators)
			{
				IPropertyAccessAware propertyAccessAware = (IPropertyAccessAware)validator;
				object value = invocation.Arguments[parameterPosition];

				if (parameterInfoMeta == ParameterInfoMeta.ParamsArray)
				{
					ValidateParamsArray(validator, propertyAccessAware, value, errors);
					continue;
				}

				propertyAccessAware.PropertyAccessor = delegate { return invocation.Arguments[parameterPosition]; };
					
				if (validator.IsValid(value)) 
					continue;

				AppendError(validator, errors);
			}
		}

		/// <summary>
		/// Validate method parameters that are decorated with the params keyword.
		/// </summary>
		/// <param name="validator">The validator.</param>
		/// <param name="propertyAccessAware">The property access aware.</param>
		/// <param name="value">The value.</param>
		/// <param name="errors">The errors.</param>
		private void ValidateParamsArray(IValidator validator, IPropertyAccessAware propertyAccessAware, object value, ErrorSummary errors)
		{
			object[] paramsValue = (object[]) value;
			foreach (object paramValue in paramsValue)
			{
				propertyAccessAware.PropertyAccessor = delegate { return paramValue; };
				if (validator.IsValid(paramValue)) continue;
				AppendError(validator, errors);
			}
		}

		/// <summary>
		/// Appends the error to the <see cref="ErrorSummary">ErrorSummary</see>.
		/// </summary>
		/// <param name="validator">The validator.</param>
		/// <param name="errors">The errors.</param>
		protected virtual void AppendError(IValidator validator, ErrorSummary errors)
		{
			string name = validator.FriendlyName ?? validator.Name;

			ObjectValidator objectValidator = validator as ObjectValidator;
			if (objectValidator != null)
				RegisterObjectValidatorErrorMessages(objectValidator, errors);
			else 
				errors.RegisterErrorMessage(name, validator.ErrorMessage);
		}

		/// <summary>
		/// Registers the error message returned from an object validator
		/// as an error message for each individual indexed property.
		/// </summary>
		/// <param name="validator">The validator.</param>
		/// <param name="errors">The errors.</param>
		private void RegisterObjectValidatorErrorMessages(ObjectValidator validator, ErrorSummary errors)
		{
			ErrorSummary objectErrors = validator.ErrorSummary;
			foreach (string property in objectErrors.InvalidProperties)
			{
				foreach (string message in objectErrors.GetErrorsForProperty(property))
				{
					errors.RegisterErrorMessage(validator.FriendlyName + "." + property, message);
				}
			}
		}
	}
}
