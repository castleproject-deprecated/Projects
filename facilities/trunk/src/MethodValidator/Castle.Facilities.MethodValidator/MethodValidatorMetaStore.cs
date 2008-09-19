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
	using Castle.MicroKernel.Facilities;

	/// <summary>
	/// Stores metadata about the method to be validated.
	/// </summary>
	public class MethodValidatorMetaStore
	{
		private readonly IValidatorRegistry registry;
		private readonly IDictionary<Type, MethodValidatorMetaInfo> type2MetaInfo = new Dictionary<Type, MethodValidatorMetaInfo>();
		private readonly BindingFlags DefaultBindingFlags = BindingFlags.Instance | BindingFlags.Public |
														   BindingFlags.NonPublic | BindingFlags.DeclaredOnly;

		/// <summary>
		/// Initializes a new instance of the <see cref="MethodValidatorMetaStore"/> class.
		/// </summary>
		/// <param name="registry">The registry.</param>
		public MethodValidatorMetaStore(IValidatorRegistry registry)
		{
			this.registry = registry;
		}

		public void CreateMetaFromType(Type service, Type implementation)
		{
			MethodValidatorMetaInfo info = CreateMetaInfo(service, implementation);
			Register(service, info);
		}

		private MethodValidatorMetaInfo CreateMetaInfo(Type service, Type implementation)
		{
			if (type2MetaInfo.Keys.Contains(service))
				return type2MetaInfo[service];

			MethodValidatorMetaInfo meta = new MethodValidatorMetaInfo();

			foreach (MethodInfo methodInfo in implementation.GetMethods(DefaultBindingFlags))
			{
				MethodInfo serviceMethodInfo = service == implementation
												   ? methodInfo
												   : GetMethodInfoFromService(service, implementation, methodInfo);

				ParameterInfo[] parameterInfo = methodInfo.GetParameters();

				for (int parameterPosition = 0; parameterPosition < parameterInfo.Length; parameterPosition++)
				{
					ParameterInfo parameter = parameterInfo[parameterPosition];

					object[] builders = parameter.GetCustomAttributes(typeof(IValidatorBuilder), true);

					foreach (IValidatorBuilder builder in builders)
					{
						builder.Initialize(registry, null);

						ParameterInfoMeta parameterMeta = GetParameterMeta(methodInfo);

						meta.AddBuilder(serviceMethodInfo, parameterPosition, builder, parameterMeta);
					}
				}
			}

			return meta;
		}

		/// <summary>
		/// Builds all of the validators for a given method, parameter position, with the defined RunWhen.
		/// </summary>
		/// <param name="method">The method.</param>
		/// <param name="parameterPosition">The parameter position.</param>
		/// <param name="runner">The runner.</param>
		/// <param name="runWhen">The run when.</param>
		/// <param name="parameterInfoMeta">Metadata about the parameters such as whether or not it is params</param>
		/// <returns></returns>
		public IValidator[] GetValidatorsFor(MethodInfo method, int parameterPosition, ValidatorRunner runner, RunWhen runWhen,
			out ParameterInfoMeta parameterInfoMeta)
		{
			MethodValidatorMetaInfo meta = type2MetaInfo[method.DeclaringType];
			List<IValidator> validators = new List<IValidator>();

			IValidatorBuilder[] builders = meta.GetBuilders(method, parameterPosition, out parameterInfoMeta);

			ParameterInfo[] parameters = method.GetParameters();

			foreach (IValidatorBuilder builder in builders)
			{
				IValidator validator = builder.Build(runner, typeof(MethodInfo));

				if (!IsValidatorOnPhase(validator, runWhen)) continue;

				if (string.IsNullOrEmpty(validator.FriendlyName))
					validator.FriendlyName = parameters[parameterPosition].Name;

				validator.Initialize(registry, null);
				validators.Add(validator);
			}

			return validators.ToArray();
		}

		public bool Contains(Type type)
		{
			return type2MetaInfo.Keys.Contains(type);
		}

		public MethodValidatorMetaInfo GetMetaFor(Type implementation)
		{
			return type2MetaInfo[implementation];
		}

		/// <summary>
		/// Gets the base method from the service interface.
		/// </summary>
		/// <param name="service">The service.</param>
		/// <param name="implementation">The implementation.</param>
		/// <param name="methodInfo">The method info.</param>
		/// <returns></returns>
		private MethodInfo GetMethodInfoFromService(Type service, Type implementation, MethodInfo methodInfo)
		{
			InterfaceMapping map = implementation.GetInterfaceMap(service);
			for (int i = 0; i < map.TargetMethods.Length; i++)
			{
				if (map.TargetMethods[i] == methodInfo)
				{
					return map.InterfaceMethods[i];
				}
			}
			throw new FacilityException(string.Format("Could not find method '{0}' from '{1}' on service '{2}'",
				methodInfo, implementation, service));
		}

		private ParameterInfoMeta GetParameterMeta(MethodInfo method)
		{
			bool isParams = false;

			ParameterInfo[] parameters = method.GetParameters();

			if (parameters != null && parameters.Length > 0)
			{
				ParameterInfo lastParameter = parameters[parameters.Length - 1];
				isParams = Attribute.IsDefined(lastParameter, typeof(ParamArrayAttribute));
			}

			return isParams ? ParameterInfoMeta.ParamsArray : ParameterInfoMeta.None;
		}

		private void Register(Type service, MethodValidatorMetaInfo validatorMetaInfo)
		{
			type2MetaInfo[service] = validatorMetaInfo;
		}

		private bool IsValidatorOnPhase(IValidator validator, RunWhen when)
		{
			if (validator.RunWhen == RunWhen.Everytime) return true;

			return ((validator.RunWhen & when) != 0);
		}
	}

	/// <summary>
	/// Metadata about the method parameters and the validators that should be invoked.
	/// </summary>
	public class MethodValidatorMetaInfo
	{

		private readonly List<ParameterValidatorInvocation> methodParameters = new List<ParameterValidatorInvocation>();
		private readonly IList<MethodInfo> methods = new List<MethodInfo>();

		public IList<MethodInfo> Methods
		{
			get { return methods; }
		}

		public bool HasBuilders(MethodInfo method)
		{
			return Methods.Contains(method);
		}

		public void AddBuilder(MethodInfo method, int parameterPosition, IValidatorBuilder builder, ParameterInfoMeta parameterMeta)
		{
			ParameterValidatorInvocation validatorInvocation = FindParameter(method, parameterPosition);

			if (validatorInvocation == null)
			{
				validatorInvocation = new ParameterValidatorInvocation(method, parameterPosition, parameterMeta);
				validatorInvocation.Builders.Add(builder);

				methodParameters.Add(validatorInvocation);
				Methods.Add(method);
			}
			else
			{
				validatorInvocation.Builders.Add(builder);
			}
		}

		public IValidatorBuilder[] GetBuilders(MethodInfo method, int parameterPosition, out ParameterInfoMeta parameterInfoMeta)
		{
			ParameterValidatorInvocation validatorInvocation = FindParameter(method, parameterPosition);
			if (validatorInvocation == null)
			{
				parameterInfoMeta = ParameterInfoMeta.None;
				return new IValidatorBuilder[] {};
			}
			
			parameterInfoMeta = validatorInvocation.ParameterInfoMeta;
			return validatorInvocation.Builders.ToArray();
		}

		private ParameterValidatorInvocation FindParameter(MethodInfo method, int parameterPosition)
		{
			return methodParameters.Find(
				delegate(ParameterValidatorInvocation candidate)
					{
						bool methodMatch;
						if (candidate.MethodInfo.IsGenericMethodDefinition && method.IsGenericMethod)
							methodMatch = candidate.MethodInfo == method.GetGenericMethodDefinition();
						else
							methodMatch = candidate.MethodInfo == method;

						bool parameterMatch = candidate.ParameterPosition == parameterPosition;
						return methodMatch && parameterMatch;
					});
		}

		private class ParameterValidatorInvocation
		{
			private readonly MethodInfo methodInfo;
			private readonly int parameterPosition;
			private readonly ParameterInfoMeta parameterInfoMeta;
			private readonly List<IValidatorBuilder> builders = new List<IValidatorBuilder>();

			public ParameterValidatorInvocation(MethodInfo methodInfo, int parameterPosition, ParameterInfoMeta parameterInfoMeta)
			{
				this.methodInfo = methodInfo;
				this.parameterPosition = parameterPosition;
				this.parameterInfoMeta = parameterInfoMeta;
			}

			public MethodInfo MethodInfo
			{
				get { return methodInfo; }
			}

			public int ParameterPosition
			{
				get { return parameterPosition; }
			}

			public List<IValidatorBuilder> Builders
			{
				get { return builders; }
			}

			public ParameterInfoMeta ParameterInfoMeta
			{
				get { return parameterInfoMeta; }
			}
		}

	}
}
