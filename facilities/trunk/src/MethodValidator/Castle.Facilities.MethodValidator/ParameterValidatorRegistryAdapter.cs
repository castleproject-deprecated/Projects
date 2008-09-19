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
	using Castle.Components.Validator;

	/// <summary>
	/// Provies an adapter for the registry so an existing registry can be used
	/// without gaining access to an accessor.
	/// </summary>
	public class ParameterValidatorRegistryAdapter : IValidatorRegistry
	{
		private readonly IValidatorRegistry baseRegistry;

		/// <summary>
		/// Initializes a new instance of the <see cref="ParameterValidatorRegistryAdapter"/> class.
		/// </summary>
		/// <param name="validatorRegistry">The validator registry.</param>
		public ParameterValidatorRegistryAdapter(IValidatorRegistry validatorRegistry)
		{
			this.baseRegistry = validatorRegistry;
		}

		/// <summary>
		/// Gets the base registry that this class adapts.
		/// </summary>
		/// <value>The base registry.</value>
		public IValidatorRegistry BaseRegistry
		{
			get { return baseRegistry; }
		}

		public IValidator[] GetValidators(ValidatorRunner validatorRunner, Type targetType, RunWhen runWhen)
		{
			throw new NotSupportedException();
		}

		public IValidator[] GetValidators(ValidatorRunner validatorRunner, Type targetType, PropertyInfo property, RunWhen runWhen)
		{
			throw new NotSupportedException();
		}

		public Accessor GetPropertyAccessor(PropertyInfo property)
		{
			return null;
		}

		public Accessor GetFieldOrPropertyAccessor(Type targetType, string path)
		{
			throw new NotSupportedException();
		}

		public string GetStringFromResource(string key)
		{
			return baseRegistry.GetStringFromResource(key);
		}
	}
}
