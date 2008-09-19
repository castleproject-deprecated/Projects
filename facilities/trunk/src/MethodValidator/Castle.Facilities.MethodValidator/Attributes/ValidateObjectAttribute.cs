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

namespace Castle.Facilities.MethodValidator.Attributes
{

	using System;
	using Castle.Components.Validator;
	using Castle.Facilities.MethodValidator.Validators;


	/// <summary>
	/// The <see cref="ValidateObjectAttribute">ValidateObjectAttribute</see> is used to indicate
	/// that a method parameter should be inspected and validated.
	/// </summary>
	[AttributeUsage(AttributeTargets.ReturnValue | AttributeTargets.Parameter)]
	public class ValidateObjectAttribute : AbstractValidationAttribute
	{
		/// <summary>
		/// Builds the <see cref="IValidator">IValidator</see> instance.
		/// </summary>
		/// <returns></returns>
		public override IValidator Build()
		{
			ObjectValidator objectValidator = new ObjectValidator();
			ConfigureValidatorMessage(objectValidator);
			return objectValidator;
		}
	}
}
