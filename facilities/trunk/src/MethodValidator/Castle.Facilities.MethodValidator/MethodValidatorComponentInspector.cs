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
	using System.Collections.Generic;
	using System.Reflection;
	using Castle.Components.Validator;
	using Castle.Core;
	using Castle.MicroKernel;
	using Castle.MicroKernel.Facilities;
	using Castle.MicroKernel.ModelBuilder.Inspectors;

	/// <summary>
	/// Inspects the <see cref="ComponentModel">ComponentModel</see> to see if the implementation
	/// has any methods that have validators.  If any methods have validators, then the type is added
	/// to the <see cref="MethodValidatorMetaStore">MethodValidatorMetaStore</see>.
	/// </summary>
	public class MethodValidatorComponentInspector : MethodMetaInspector
	{
		private static readonly string MethodValidatorNodeName = "MethodValidator";
		
		private MethodValidatorMetaStore metaStore;

		/// <summary>
		/// Processes the model.
		/// </summary>
		/// <param name="kernel">The kernel.</param>
		/// <param name="model">The model.</param>
		public override void ProcessModel(IKernel kernel, ComponentModel model)
		{
			if (metaStore == null)
				metaStore = (MethodValidatorMetaStore)kernel[typeof(MethodValidatorMetaStore)];

			Type service = GetServiceOrDefault(model);

			if (HasBuilders(model.Implementation))
				metaStore.CreateMetaFromType(service, model.Implementation);			

			Validate(model, metaStore);

			AddMethodInterceptorIfValidatable(model, metaStore);
		}

		/// <summary>
		/// Determines whether the specified implementation has any validator builders.
		/// </summary>
		/// <param name="implementation">The implementation.</param>
		/// <returns>
		/// 	<c>true</c> if the specified implementation has builders; otherwise, <c>false</c>.
		/// </returns>
		private bool HasBuilders(Type implementation)
		{
			foreach (MethodInfo methodInfo in implementation.GetMethods())
				foreach (ParameterInfo parameter in methodInfo.GetParameters())
					if (parameter.IsDefined(typeof(IValidatorBuilder), true))
						return true;
			
			return false;
		}

		/// <summary>
		/// Validates the specified model to make sure that any methods that are being intercepted
		/// are virtual and can be intercepted.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="store">The store.</param>
		private void Validate(ComponentModel model, MethodValidatorMetaStore store)
		{
			if (model.Service == null || model.Service.IsInterface)
				return;

			MethodValidatorMetaInfo meta = store.GetMetaFor(model.Implementation);

			if (meta == null)
				return;

			List<string> problematicMethods = new List<string>();

			foreach (MethodInfo method in meta.Methods)
			{
				if (!method.IsVirtual)
					problematicMethods.Add(method.Name);
			}

			if (problematicMethods.Count != 0)
			{
				String message = String.Format("The class {0} wants to use method validator interception, " +
											   "however the methods must be marked as virtual in order to do so. Please correct " +
											   "the following methods: {1}", model.Implementation.FullName,
											   String.Join(", ", problematicMethods.ToArray()));
				throw new FacilityException(message);
			}
		}

		/// <summary>
		/// Adds the method interceptor to the type if there are methods that have validators
		/// declared.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <param name="store">The store.</param>
		private static void AddMethodInterceptorIfValidatable(ComponentModel model, MethodValidatorMetaStore store)
		{
			MethodValidatorMetaInfo meta = store.GetMetaFor(GetServiceOrDefault(model));

			if (meta == null)
				return;

			model.Dependencies.Add(new DependencyModel(DependencyType.Service, null, typeof(MethodValidatorInterceptor), false));
			model.Interceptors.AddFirst(new InterceptorReference(typeof(MethodValidatorInterceptor)));
		}

		/// <summary>
		/// If the service is not set for the model, return the implementation.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		private static Type GetServiceOrDefault(ComponentModel model)
		{
			Type service = model.Service;
			if (service == null)
				service = model.Implementation;
			return service;
		}

		/// <summary>
		/// Obtains the name of the node.
		/// </summary>
		/// <returns></returns>
		protected override string ObtainNodeName()
		{
			return MethodValidatorNodeName;
		}
	}
}
